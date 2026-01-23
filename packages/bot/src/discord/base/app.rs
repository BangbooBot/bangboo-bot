use crate::discord::HANDLERS;
use crate::functions::{log, success, warn};
use crate::{env::ENV, functions::error};
use std::sync::Arc;
use tokio::task::JoinSet;
use tokio::time::{interval, sleep, Duration};
use twilight_cache_inmemory::{DefaultInMemoryCache, InMemoryCache, ResourceType};
use twilight_gateway::{
    Config, ConfigBuilder, Event, EventTypeFlags, Intents, MessageSender, Shard, ShardId,
    StreamExt as _,
};
use twilight_http::Client;
use twilight_http_ratelimiting::RateLimiter;
use twilight_model::application::interaction::InteractionData;

pub struct App {
    pub http: Arc<Client>,
    pub cache: Arc<InMemoryCache>,
    pub shards: Vec<Shard>,
    pub intents: Intents,
}

#[derive(Clone)]
pub struct Context {
    pub http: Arc<Client>,
    pub cache: Arc<InMemoryCache>,
    pub sender: MessageSender,
}

impl App {
    pub async fn bootstrap(intents: Intents) -> Self {
        let token = &ENV.BOT_TOKEN;

        // Create HTTP client with rate limiting to prevent 429 errors
        let http = Client::builder()
            .token(token.clone())
            .ratelimiter(Some(RateLimiter::default()))
            .build();
        let http = Arc::new(http);

        let config = Config::new(token.clone(), intents);
        let config_callback = |_, builder: ConfigBuilder| builder.build();

        let shards = match twilight_gateway::create_recommended(
            &http,
            config.clone(),
            config_callback,
        )
        .await
        {
            Ok(shards) => shards,
            Err(err) => {
                error(&format!("Error trying to create shards\n└ {:?}", err));
                panic!();
            }
        }.collect::<Vec<_>>();

        let cache = Arc::new(
            DefaultInMemoryCache::builder()
                .resource_types(ResourceType::all())
                .build(),
        );

        Self {
            http,
            cache,
            shards,
            intents,
        }
    }

    pub async fn run(&mut self) {
        log("Starting bot with reshard system...");
        
        // Spawn reshard timer task (60 minutes)
        let http_clone = self.http.clone();
        let cache_clone = self.cache.clone();
        let intents = self.intents;
        
        tokio::spawn(async move {
            App::reshard_timer(http_clone, cache_clone, intents).await;
        });

        // Start all shards with recovery system
        let mut set = JoinSet::new();
        for shard in self.shards.drain(..) {
            let shard_id = shard.id();
            set.spawn(App::shard_runner_with_recovery(
                self.http.clone(),
                self.cache.clone(),
                self.intents,
                shard_id,
            ));
        }

        // Monitor shard tasks
        while let Some(res) = set.join_next().await {
            match res {
                Ok(_) => log("Shard runner finished."),
                Err(e) => error(&format!("Shard runner task error: {:?}", e)),
            }
        }
    }

    /// Timer que executa reshard a cada 60 minutos
    async fn reshard_timer(http: Arc<Client>, cache: Arc<InMemoryCache>, intents: Intents) {
        let mut timer = interval(Duration::from_secs(60 * 60 * 6)); // 6 hours
        timer.tick().await; // Skip first tick

        loop {
            timer.tick().await;
            warn("🕰️ Executing scheduled reshard (6 hours interval)...");
            
            if let Err(e) = App::perform_reshard(http.clone(), cache.clone(), intents).await {
                error(&format!("Failed to perform scheduled reshard: {:?}", e));
            }
        }
    }

    /// Executa o processo de reshard
    async fn perform_reshard(
        http: Arc<Client>,
        cache: Arc<InMemoryCache>,
        intents: Intents,
    ) -> Result<(), Box<dyn std::error::Error + Send + Sync>> {
        log("Creating new shards for reshard...");
        
        let token = &ENV.BOT_TOKEN;
        let config = Config::new(token.clone(), intents);
        let config_callback = |_, builder: ConfigBuilder| builder.build();

        let new_shards = twilight_gateway::create_recommended(&http, config, config_callback)
            .await?
            .collect::<Vec<_>>();

        success(&format!("Reshard completed: {} shards created", new_shards.len()));

        // Start new shards
        for shard in new_shards {
            let shard_id = shard.id();
            tokio::spawn(App::shard_runner_with_recovery(
                http.clone(),
                cache.clone(),
                intents,
                shard_id,
            ));
        }

        Ok(())
    }

    /// Runner de shard com sistema de recuperação automática
    async fn shard_runner_with_recovery(
        http: Arc<Client>,
        cache: Arc<InMemoryCache>,
        intents: Intents,
        shard_id: ShardId,
    ) {
        loop {
            log(&format!("Starting shard {} runner...", shard_id.number()));
            
            match App::create_and_run_shard(http.clone(), cache.clone(), intents, shard_id).await {
                Ok(_) => {
                    log(&format!("Shard {} finished normally", shard_id.number()));
                    break;
                }
                Err(e) => {
                    error(&format!(
                        "Shard {} crashed: {:?}. Attempting recovery in 60 seconds...",
                        shard_id.number(),
                        e
                    ));
                    
                    // Wait 60 seconds before retry
                    sleep(Duration::from_secs(60)).await;
                    log(&format!("Attempting to recreate shard {}...", shard_id.number()));
                }
            }
        }
    }

    /// Cria e executa uma shard específica
    async fn create_and_run_shard(
        http: Arc<Client>,
        cache: Arc<InMemoryCache>,
        intents: Intents,
        shard_id: ShardId,
    ) -> Result<(), Box<dyn std::error::Error + Send + Sync>> {
        let token = &ENV.BOT_TOKEN;
        let config = Config::new(token.clone(), intents);

        let shard = Shard::with_config(shard_id, config);
        
        success(&format!("Shard {} created successfully", shard_id.number()));
        
        App::shard_handle(http, cache, shard).await;
        
        Ok(())
    }

    pub async fn shard_handle(
        http: Arc<Client>,
        cache: Arc<InMemoryCache>,
        mut shard: Shard,
    ) {
        let ctx = Context {
            http,
            cache,
            sender: shard.sender(),
        };

        while let Some(item) = shard.next_event(EventTypeFlags::all()).await {
            let Ok(event) = item else {
                error(&format!("Error receiving event\n└ {:?}", item.unwrap_err()));
                continue;
            };
            
            // Update the cache with the event.
            ctx.cache.update(&event);

            tokio::spawn(App::handle_event(ctx.clone(), event));
        }
    }

    async fn handle_event(ctx: Context, event: Event) {
        if let Some(callback) = HANDLERS.event_handlers.get(&event.kind()) {
            if let Err(err) = callback.run(ctx, event).await {
                error(&format!("Event error!\n└ {:?}", err));
            }
            return;
        }
        
        match event {
            Event::InteractionCreate(interaction) => {
                if let Some(data) = &interaction.data {
                    match data {
                        InteractionData::ApplicationCommand(command) => {
                            if let Some(callback) =
                                HANDLERS.app_command_handlers.get(command.name.as_str())
                            {
                                if let Err(err) = callback.run(ctx, &interaction).await {
                                    error(&format!("Application command error!\n└ {:?}", err));
                                }
                            }
                        }
                        InteractionData::ModalSubmit(modal_data) => {
                            if let Some(callback) =
                                HANDLERS.modal_handlers.get(modal_data.custom_id.as_str())
                            {
                                if let Err(err) = callback.run(ctx, &interaction).await {
                                    error(&format!("Modal submit error!\n└ {:?}", err));
                                }
                            }
                        }
                        InteractionData::MessageComponent(message_component) => {
                            if let Some(callback) =
                                HANDLERS.message_component_handlers.get(message_component.custom_id.as_str())
                            {
                                if let Err(err) = callback.run(ctx, &interaction).await {
                                    error(&format!("Modal submit error!\n└ {:?}", err));
                                }
                            }
                        }
                        _ => {}
                    }
                }
            }
            _ => {}
        }
    }
}
