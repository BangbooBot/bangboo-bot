use std::mem;

use twilight_http::request::AuditLogReason;
use twilight_model::channel::Message;
use chrono::Utc;
use twilight_util::builder::embed::EmbedBuilder;
use crate::constants::*;
use crate::discord::*;
use crate::functions::*;

pub async fn mousetrap(ctx: &Context, message: &Message) {
    if message.channel_id.get() != GUILD.channels.mousetrap {
        return;
    }

    let guild_id = match message.guild_id.as_ref() {
        Some(guild_id) => guild_id,
        None => {
            return;
        }
    };

    let id = message.author.id;
    let reason = "Captured by mousetrap.";

    if let Err(err) = ctx.http
        .create_ban(guild_id.clone(), id)
        .delete_message_seconds(604_800)
        .reason(reason)
        .await {
        error(&format!("Failed to ban user\n└ {:?}", err));
        return;
    }

    if let Some(member) = &message.member {
        if let Some(joined_at) = member.joined_at {
            let now = Utc::now().timestamp();
            let joined = joined_at.as_secs();
            
            if now - joined > (60 * 60 * 24 * 30) {
                if let Err(err) = ctx.http.delete_ban(guild_id.clone(), id).await {
                    error(&format!("Failed to unban {} after being captured by mousetrap\n└ {:?}", &message.author.name, err));
                } else {
                    if let Ok(dm_channel) = ctx.http.create_private_channel(id).await {
                        if let Ok(channel) = dm_channel.model().await {
                            let dm_content = "Hello! Bangboo has identified that your account sent a series of suspicious messages on the server's channels. As you are a long-time member of the server, your ban has been automatically removed since your account was likely hacked. I recommend that you review your account security!";
                            let embed = EmbedBuilder::new()
                                .description(dm_content)
                                .color(COLORS.warning)
                                .build();
                            if let Err(err) = ctx.http.create_message(channel.id)
                                .embeds(&[embed])
                                .await {
                                    error(&format!("Failed to send DM to {} after being captured by mousetrap\n└ {:?}", &message.author.name, err));
                                }
                        }
                    }
                }
            }
        }
    }
}

/*
static REGEXSTEAM: Lazy<Regex> =
    Lazy::new(|| Regex::new(r"\[\s*(?i:steam)[^\]]*]\((https?://[^)]+)\)").unwrap());
static REGEXATTACHMENT: Lazy<Regex> = Lazy::new(|| {
    Regex::new(r"(?:(?:https?:\/\/)?(?:cdn|media)\.discordapp\.(?:net|com)\/attachments\/\d+\/\d+\/[^\s]+\.(?:png|jpg|jpeg|webp)(?:\?[^\s]*)?\s*){3,}").unwrap()
});
 */

/*
pub async fn filter_attachment(ctx: &Context, message: &Message) {
    let guild_id = match message.guild_id.as_ref() {
        Some(guild_id) => guild_id,
        None => {
            return;
        }
    };

    let attachments = &message.attachments;
    if attachments.len() < 4 {
        return;
    }
    let hasher = average::AverageHasher {
        width: 32,
        height: 32,
        color_space: ColorSpace::REC709,
    };
    let image_attachments = attachments
        .iter()
        .map(|attachment| {
            if let Some(content_type) = attachment.content_type.as_ref() {
                if !content_type.starts_with("image/") && content_type.starts_with("image/gif") {
                    return None;
                }
                return Some(attachment);
            }
            None
        })
        .flatten()
        .collect::<Vec<_>>();

    if image_attachments.len() < 4 {
        return;
    }

    if let Some(attachment) = image_attachments.last() {
        let image_buffer = match attachment.download().await {
            Ok(buf) => buf,
            Err(_) => return,
        };

        let image = match image::load_from_memory(&image_buffer) {
            Ok(img) => img,
            Err(_) => return,
        };

        let image_hash = hasher.hash_from_img(&image);

        let mut distance = 2;
        for hash in HASHES.iter() {
            let dist = match hash.distance(&image_hash) {
                Ok(size) => size,
                Err(_) => continue,
            };

            if dist >= distance {
                continue;
            }
            distance = dist;
            break;
        }

        if distance < 2 {
            let channel_id = &message.channel_id;
            let user_id = message.author.id.get();

            let warning = format!(
                "<@{}> sent a image that was identified as a scam. Bangboo (me) will presume his/her account has been compromised, leading to a server kick!",
                user_id
            );

            let server_embed = CreateEmbed::new()
                .color(COLORS.warning)
                .description(warning);
            let warning_msg = CreateMessage::new().embed(server_embed);
            if let Err(err) = channel_id.send_message(ctx.http(), warning_msg).await {
                error(&format!("Failed to send warning message\nʟ {:?}", err));
            }
            if let Err(err) = message
                .delete(ctx.http(), Some("Sent a message flagged as a scam"))
                .await
            {
                error(&format!("Failed to delete scan message\nʟ {:?}", err));
            }

            let guild = match guild_id.to_guild_cached(&ctx.cache) {
                Some(guild) => guild.clone(),
                None => {
                    error("Guild is none.");
                    return;
                }
            };

            if guild.owner_id == message.author.id {
                error("Tried to kick the owner of the guild.");
                return;
            }

            if guild
                .id
                .kick(
                    ctx.http(),
                    message.author.id,
                    Some("Sent a message that was flagged as a scam."),
                )
                .await
                .is_err()
            {
                error("Failed to kick the owner of the scam message.");
                return;
            }

            let private_channel = match message.author.id.create_dm_channel(ctx.http()).await {
                Ok(channel) => channel,
                Err(err) => {
                    error(&format!("Failed to create DM channel.\nʟ {:?}", err));
                    return;
                }
            };

            let dm_warning = "It look like you probably got hacked and sent a scam image. You were just kicked from the server, but feel free to come back as soon as you resolve the issue with your account.";
            let dm_embed = CreateEmbed::new()
                .color(COLORS.warning)
                .description(dm_warning);

            if let Err(err) = private_channel
                .id
                .widen()
                .send_message(ctx.http(), CreateMessage::new().embed(dm_embed))
                .await
            {
                error(&format!("Failed to send DM warning.\nʟ {:?}", err));
                return;
            }
        }
    }
}
 */

/*
pub async fn filter_message(ctx: &Context, message: &Message) -> bool {
    let guild_id = match message.guild_id.as_ref() {
        Some(guild_id) => guild_id,
        None => {
            return true;
        }
    };

    let content = &message.content;

    let steam_test = REGEXSTEAM.is_match(&content);
    let attatchment_test = REGEXATTACHMENT.is_match(&content);
    if !steam_test && !attatchment_test {
        return false;
    }

    let channel_id = &message.channel_id;
    let user_id = message.author.id.get();

    let warning = format!(
        "<@{}> sent a message that was flagged as a scam. Messages containing **[text](hyperlink)** or **more than 2 image attachments** are strictly prohibited. Bangboo (me) will presume his/her account has been compromised, leading to a server kick!",
        user_id
    );

    let server_embed = CreateEmbed::new()
        .color(COLORS.warning)
        .description(warning);
    let mut warning_msg = CreateMessage::new().embed(server_embed);
    if let Err(err) = channel_id.send_message(ctx.http(), warning_msg).await {
        error(&format!("Failed to send warning message\nʟ {:?}", err));
    }
    if let Err(err) = message
        .delete(ctx.http(), Some("Sent a message flagged as a scam"))
        .await
    {
        error(&format!("Failed to delete scan message\nʟ {:?}", err));
    }

    let guild = match guild_id.to_guild_cached(&ctx.cache) {
        Some(guild) => guild.clone(),
        None => {
            error("Guild is none.");
            return true;
        }
    };

    if guild.owner_id == message.author.id {
        error("Tried to kick the owner of the guild.");
        return true;
    }

    if guild
        .id
        .kick(
            ctx.http(),
            message.author.id,
            Some("Sent a message that was flagged as a scam."),
        )
        .await
        .is_err()
    {
        error("Failed to kick the owner of the scam message.");
        return true;
    }

    let private_channel = match message.author.id.create_dm_channel(ctx.http()).await {
        Ok(channel) => channel,
        Err(err) => {
            error(&format!("Failed to create DM channel.\nʟ {:?}", err));
            return true;
        }
    };

    let dm_warning = "It look like you probably got hacked and sent a message that was flagged as scam containing **[text](hyperlink)** or **more than 2 image attachments**. You were just kicked from the server, but feel free to come back as soon as you resolve the issue with your account.";
    let dm_embed = CreateEmbed::new()
        .color(COLORS.warning)
        .description(dm_warning);

    if let Err(err) = private_channel
        .id
        .widen()
        .send_message(ctx.http(), CreateMessage::new().embed(dm_embed))
        .await
    {
        error(&format!("Failed to send DM warning.\nʟ {:?}", err));
        return true;
    }

    true
}
*/
