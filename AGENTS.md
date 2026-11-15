# Bangboo — Backend (.NET 10)

Guia para agentes e pessoas que trabalham neste repositório. Ele descreve o **estado alvo**: se o código atual divergir, siga este guia e consulte "Dívidas conhecidas" no fim. Visão geral do sistema: `../AGENTS.md`.

## 1. Estrutura da solução

```
Bangboo.slnx
Directory.Build.props        net10.0, Nullable, ImplicitUsings para todos os projetos
Directory.Packages.props     versões de TODOS os pacotes (Central Package Management)
global.json                  dotnet test usa Microsoft.Testing.Platform
├─ src/
│  ├─ Bangboo.Core/          classlib — ÚNICO dono do schema
│  │  ├─ Entities/           User, Guild, Member, GuildMemberEvents, Mousetrap, ModeratorRole, BotStatus
│  │  ├─ Data/AppDbContext.cs (+ AppDbContextDesignTimeFactory para o dotnet ef)
│  │  ├─ Data/Configurations/ <Entidade>Configuration.cs (IEntityTypeConfiguration<T>)
│  │  ├─ Migrations/
│  │  └─ Extensions/          AddBangbooDatabase(this IServiceCollection, IConfiguration)
│  ├─ Bangboo.Api/           ASP.NET Core Web API
│  │  ├─ Controllers/        <Recurso>Controller.cs (AuthController, GuildsController, StatusController)
│  │  ├─ Services/           <Recurso>Service.cs (implementações)
│  │  │  └─ Interfaces/      I<Recurso>Service.cs (contratos usados pelos controllers e testes)
│  │  ├─ Dtos/               <Nome>Request.cs / <Nome>Response.cs (+ modelos internos de entrada, ex.: DiscordProfile)
│  │  ├─ Extensions/         ServiceCollectionExtensions.AddApplicationServices() (registro dos services)
│  │  ├─ Common/Errors/       ApiError, AppException + derivadas, GlobalExceptionHandler, ApiErrorWriter
│  │  ├─ Common/Auth/         Discord OAuth + cookie, policies, ICurrentUser
│  │  ├─ Common/Discord/      IDiscordApiClient (REST) + cache, permissões, CDN
│  │  ├─ Common/OpenApi/      transformers do documento
│  │  └─ Program.cs          só composição: chama extensões, nada de lógica
│  └─ Bangboo.Bot/           NetCord (exe)
│     ├─ Discord/{Commands,Events,Responders}/  Services/  Utils/
│     └─ Program.cs
├─ tests/
│  ├─ Bangboo.Api.Tests/     Unit/ e Integration/
│  └─ Bangboo.Bot.Tests/
└─ openapi/Bangboo.Api.json  gerado no build — commitado
```

- Organização **em camadas** (padrão MVC/ASP.NET):
  - `Controllers/` fala HTTP.
  - `Services/` tem a regra de negócio.
  - `Services/Interfaces/` guarda as abstrações.
  - `Dtos/` define o formato de entrada/saída.

  Recurso novo (ex.: `Members`):
  1. `Dtos/MemberResponse.cs`
  2. `Services/Interfaces/IMemberService.cs`
  3. `Services/MemberService.cs`
  4. `Controllers/MembersController.cs`
  5. uma linha em `AddApplicationServices()`
- Namespaces seguem as pastas: `Bangboo.Api.Controllers`, `Bangboo.Api.Services`, `Bangboo.Api.Services.Interfaces`, `Bangboo.Api.Dtos`.
- Infra compartilhada (não é regra de negócio) fica em `Common/<Assunto>/` com sua própria extensão (`AddBangbooAuth`, `AddDiscordApi`, `AddApiErrorHandling`, `AddApiDocs`).
- Pacote novo: adicione a versão em `Directory.Packages.props` e a referência **sem** `Version` no `.csproj`.
- IDs do Discord (snowflakes) saem na API como **string**: não cabem em `number` do JavaScript.
- Dependências: `Api → Core`, `Bot → Core`. **Api e Bot nunca se referenciam.**

## 2. Regras de código (SOLID / código limpo)

- **Controller fino**: recebe o request, chama um service e retorna o DTO. Não acessa `DbContext`, `HttpClient` ou `RestClient` diretamente e não contém regra de negócio.
- **Service com uma responsabilidade**, sempre atrás de uma interface (`IGuildService`), para depender de abstração e permitir mock nos testes.
- **Registro explícito de DI** em `Extensions/ServiceCollectionExtensions.cs`:
  ```csharp
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
      services.AddScoped<IGuildService, GuildService>();
      // ...
      return services;
  }
  ```
- Controllers dependem **só da interface** (`IGuildService`), nunca da classe concreta.
  Proibido: registro por varredura de reflexão, classes-base que resolvem serviços via `IHost`/`IServiceProvider` (service locator) e `new` de serviços.
- **Construtor primário** para injeção: `public sealed class GuildService(AppDbContext db, IDiscordGateway discord) : IGuildService`.
- **Configuração** via Options pattern, nunca `IConfiguration["..."]` espalhado:
  ```csharp
  services.AddOptions<DiscordOptions>().BindConfiguration("Discord").ValidateDataAnnotations().ValidateOnStart();
  ```
- **Nomenclatura**:
  - Entidades no singular, sem sufixo (`Guild`, não `GuildsModel`).
  - DTOs são `record` com sufixo `Request` ou `Response`. **Nunca exponha entidade na API.**
  - Métodos async terminam em `Async` e recebem `CancellationToken ct`.
- `Nullable` ligado e sem `!` para calar o compilador. Prefira `sealed` em classes concretas.
- Mapeamento do EF em `IEntityTypeConfiguration<T>` (nada de `OnModelCreating` gigante). Leituras com `AsNoTracking()` e projeção direta para DTO (`Select(g => new GuildResponse(...))`).
- Rotas sempre sob `api/`, com substantivo no plural e kebab-case: `[Route("api/guilds")]`, `api/guilds/{guildId}/member-events`.
- Métodos curtos e nomes que explicam. Comentário explica o *porquê*, não o *quê*.

## 3. Contrato de erro (obrigatório)

Toda resposta de erro tem **exatamente** `{ status, message }`:

```csharp
public sealed record ApiError(int Status, string Message);

public abstract class AppException(int status, string message) : Exception(message)
{
    public int Status { get; } = status;
}
// Common/Errors/AppException.cs
ValidationException       // 400
UnauthorizedException     // 401 (ex.: token do Discord expirou → front refaz login)
ForbiddenException        // 403
NotFoundException         // 404
ConflictException         // 409
ExternalServiceException  // 502 (Discord fora/erro)
```

Precisa de outro status? Crie uma derivada nova em `AppException.cs`, não lance `AppException` genérica.

- **Services lançam `AppException`.** Controllers não montam erro à mão (`return NotFound("...")` é proibido).
- `GlobalExceptionHandler : IExceptionHandler` converte `AppException` em `ApiError(ex.Status, ex.Message)`. Qualquer outra exceção é logada e vira `ApiError(500, "Internal server error")`, sem vazar stack ou detalhes.
- Validação de model: `ApiBehaviorOptions.InvalidModelStateResponseFactory` retorna `ApiError(400, <primeira mensagem de erro>)`. `SuppressMapClientErrors = true` (nada de ProblemDetails).
- Respostas sem corpo (404 de rota inexistente, 405...): `UseStatusCodePages` escreve `ApiError` via `ApiErrorWriter`. Os eventos do cookie auth `OnRedirectToLogin` → 401 e `OnRedirectToAccessDenied` → 403 **não redirecionam**, retornam `ApiError`.
- Fora do MVC (middleware, eventos de auth), escreva erro **só** com `ApiErrorWriter.WriteAsync(context, status, message)`.
- Obs.: para usuário anônimo, rota inexistente responde 401 (a `FallbackPolicy` vale também sem endpoint), o que não revela quais rotas existem.
- Sucesso retorna o DTO direto, sem envelope.
- Todo endpoint declara os tipos de resposta para o OpenAPI:
  ```csharp
  [ProducesResponseType<GuildResponse>(StatusCodes.Status200OK)]
  [ProducesResponseType<ApiError>(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType<ApiError>(StatusCodes.Status404NotFound)]
  ```

## 4. Rotas públicas vs. privadas

- `FallbackPolicy` exige usuário autenticado: **toda rota é privada por padrão**.
- Rota pública é explícita, com `[AllowAnonymous]` (ex.: `api/status`, `api/status/commands`, `api/auth/login`).
- Acesso a uma guild: policy `AuthPolicies.ManageGuild` (`ManageGuildRequirement` + `ManageGuildAuthorizationHandler`): o usuário é dono, Administrator ou tem Manage Server na guild do parâmetro de rota `{guildId}`. Toda rota `api/guilds/{guildId}/...` usa `[Authorize(Policy = AuthPolicies.ManageGuild)]`. A regra de permissão fica só em `DiscordPermissions.CanManage`.
- Dados do usuário logado: injete `ICurrentUser` (id, username, avatar, `GetDiscordAccessTokenAsync()`). Services nunca usam `HttpContext`.
- Proibido checar cookie/sessão na mão em middleware ou controller.

## 5. Autenticação (Discord OAuth)

Pacotes: `AspNet.Security.OAuth.Discord` (aspnet-contrib) + Cookie authentication do ASP.NET Core. Tudo configurado em `Common/Auth/AuthExtensions.cs` (`AddBangbooAuth`):

- `DefaultScheme` **e** `DefaultChallengeScheme` = Cookie: rota privada sem sessão responde 401 `ApiError` (nunca redireciona). O Discord só é acionado explicitamente por `Challenge(..., DiscordAuthenticationDefaults.AuthenticationScheme)` em `GET api/auth/login`.
- Cookie `bangboo.auth`: `HttpOnly`, `SameSite=Lax`, `Secure` fora de Development, 7 dias (a validade do token do Discord), sliding.
- Discord: scopes `identify guilds`, `CallbackPath = /api/auth/callback/discord`, `SaveTokens = true` (o access token fica criptografado no cookie e é usado em `/users/@me/guilds`). O correlation cookie é `SameSite=Lax` para funcionar em http no dev.
- `OnCreatingTicket` faz o upsert do usuário (`IUserService.UpsertAsync`). `OnRemoteFailure` (login cancelado) redireciona para `/?login=failed`.
- ClientId/ClientSecret/BotToken vêm de `DiscordOptions` (seção `Discord`, validada no start).

- O **redirect URI cadastrado no Discord aponta para a origem do dashboard** (`http://localhost:3000/api/auth/callback/discord`). O proxy do dashboard repassa para a API. Use `UseForwardedHeaders` (X-Forwarded-Host/Proto) para o `redirect_uri` sair correto.
- Data Protection keys persistidas no Postgres (`PersistKeysToDbContext<AppDbContext>`), para que restart ou deploy não desloguem ninguém.
- Endpoints em `Controllers/AuthController.cs`:
  - `GET api/auth/login?returnUrl=` (`[AllowAnonymous]`): `Challenge` com `RedirectUri = returnUrl`. Aceite só caminhos relativos (`Url.IsLocalUrl`), senão use `/dashboard`.
  - `POST api/auth/logout`: `SignOut` do cookie e 204.
  - `GET api/auth/me`: o `UserResponse` do usuário logado (401 `ApiError` se não houver).
- Segredos (token do bot, client secret, connection string) ficam em user-secrets (`dotnet user-secrets set "Discord:ClientSecret" "..." --project src/Bangboo.Api`), variáveis de ambiente (`Discord__ClientSecret`) ou `appsettings.json` local (ignorado pelo git). Modelo em `appsettings.json.example`.
- Chamadas ao Discord passam por `IDiscordApiClient` (`Common/Discord/`), com cache em memória (`CachedDiscordApiClient`) para respeitar o rate limit. Nunca use `HttpClient` direto num service.

## 6. Bot (`Bangboo.Bot`)

- Handlers (comandos, eventos, interações) são finos e delegam para services em `Services/`, com a mesma regra de interface + DI explícita.
- A configuração por guild vem do banco via `Bangboo.Core`, não de JSON. Ela é salva pelo dashboard (API) e lida pelo bot com cache curto (ex.: `Services/Mousetrap/MousetrapSettingsProvider`, 30 s), sem consultar o banco a cada mensagem.
- **Mousetrap**: `AutomodService` + regra pura em `MousetrapPolicy` (testada). `kick` = ban + unban na hora (apaga 7 dias de mensagens); `byTenure` expulsa quem está há `MinTenureDays` ou mais e bane os mais novos; a DM vai **antes** da punição; o dono nunca é punido.
- **`/moderate`**: pode usar quem é dono, tem Administrador ou tem um dos cargos salvos no dashboard (tabela `moderator_roles`, página Moderação). Regra pura em `Services/Moderation/ModerationPolicy`; `IModeratorRolesProvider.EnsureCanModerateAsync` checa no comando **e** em cada modal (o cargo pode ser removido entre abrir e enviar). Cache de 30 s.
- `Services/BotStatus/BotStatusHeartbeat` (BackgroundService) grava latência, início e guild count via `IBotStatusWriter` a cada 30 s. A API considera o bot offline sem heartbeat por 90 s.
- Serviços singleton/background usam `IDbContextFactory<AppDbContext>`, nunca um `AppDbContext` injetado.
- O bot **não expõe HTTP**. Se precisar receber sinal do site (ex.: "recarregar config"), use `LISTEN/NOTIFY` do Postgres.
- Erros de comando passam pelo handler central de resultado (`ErrorCommandResultHandler`) e respondem com embed padronizado, nunca com stack trace.
- **Traduções dos slash commands**:
  - Os textos padrão (em inglês) ficam nos atributos: `[SlashCommand("nome", "Descrição")]` e `[SlashCommandParameter(Name = "...", Description = "...")]`. Todo parâmetro tem `Description`.
  - As traduções ficam em `Localizations/<locale>.json` (hoje `pt-BR.json`), lidas pelo `JsonLocalizationsProvider` do NetCord (`Services/Localization/CommandLocalizations.cs`).
  - Formato: `commands.<comando>.description`, `commands.<comando>.parameters.<parâmetro>.description` e, para escolhas de enum, `enums.<NomeDoEnum>.<Campo>`.
  - Nomes de comandos e parâmetros **não** são traduzidos (`/profile` é igual em qualquer idioma). Traduzimos só descrições e escolhas.
  - Comando novo = descrição em `pt-BR.json` no mesmo PR. O teste `CommandLocalizationsTests.EverySlashCommand_HasPortugueseDescription` falha se faltar.
  - As traduções chegam ao Discord quando o bot sobe (registro dos comandos), e o dashboard as exibe via `GET api/status/commands?locale=pt-BR`.

## 7. Documentação da API (OpenAPI, Scalar, Swagger)

- Fonte única: o `AddOpenApi()` nativo do .NET. As UIs leem `/openapi/v1.json`:
  - Scalar em `/scalar` (`MapScalarApiReference()`)
  - Swagger UI em `/swagger` (`UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "Bangboo API"))`)
  - Ficam ativas em `Development` ou com `Docs:Enabled=true`.
- `<GenerateDocumentationFile>true</GenerateDocumentationFile>`: **todo endpoint tem `/// <summary>`** (e `<remarks>` quando necessário) e `ProducesResponseType` de sucesso e de cada `ApiError` possível.
- Transformers em `Common/OpenApi/`: `CookieAuthDocumentTransformer` (título, descrição e security scheme do cookie `bangboo.auth`) e `AnonymousOperationTransformer` (tira o cadeado das rotas `[AllowAnonymous]`).
- Respostas só em `application/json` e números estritos (`JsonNumberHandling.Strict` no MVC **e** no `ConfigureHttpJsonOptions`), para o front receber `number` e não `number | string`.
- Geração no build (`Microsoft.Extensions.ApiDescription.Server` + `OpenApiGenerateDocumentsOnBuild` no `Bangboo.Api.csproj`) gera `openapi/Bangboo.Api.json`, que **é commitado**. Nesse modo o app sobe sem configuração: use `BuildContext.IsGeneratingOpenApi` para pular validações de start, e não acesse banco/Discord durante o startup. No dashboard, `bun run api:sync` copia esse arquivo e gera os tipos do `openapi-fetch`.
- Mudou contrato (rota, DTO, status)? Faça o build, commite o `openapi/` e avise que o dashboard precisa de `bun run api:sync`.

## 8. Testes

- **Endpoint/service novo ou bug corrigido vem com teste.**
- xUnit v3 rodando no **Microsoft.Testing.Platform** (`global.json`). Use `TestContext.Current.CancellationToken` nas chamadas async.
- `tests/Bangboo.Api.Tests`:
  - `Unit/`: services e helpers com **NSubstitute** para as interfaces.
  - `Integration/`: `BangbooApiFactory` (`WebApplicationFactory<Program>` + **Testcontainers** PostgreSQL). Herde de `ApiTestBase`, que limpa o banco e o substitute entre testes. Auth via `TestAuthHandler`: `Factory.CreateClientAs(userId)` autentica e `CreateClientAs(null)` é anônimo. O Discord é `Factory.Discord` (substitute de `IDiscordApiClient`), **nunca o Discord real**.
  - **Testes de contrato obrigatórios** (`ErrorContractTests`): use `AssertApiErrorAsync(response, status)`, que verifica o corpo exatamente `{ status, message }`.
- `tests/Bangboo.Bot.Tests`: services do bot, com Postgres em container quando tocam o banco, sem gateway real.
- Testes de integração precisam do **Docker** rodando.
- Nome dos testes: `Metodo_Cenario_ResultadoEsperado`. Um comportamento por teste, no padrão Arrange/Act/Assert.
- O CI (`.github/workflows/ci.yml`) roda build, testes e verifica se `openapi/` está atualizado.

## 9. Comandos

```bash
dotnet build
dotnet test --solution Bangboo.slnx
dotnet run --project src/Bangboo.Api        # http://localhost:3001 (Scalar em /scalar, Swagger em /swagger)
dotnet run --project src/Bangboo.Bot
dotnet ef migrations add <Nome> --project src/Bangboo.Core
dotnet ef database update --project src/Bangboo.Core   # conexão: env ConnectionStrings__DbConnection
```

As migrations não precisam de projeto de startup (`AppDbContextDesignTimeFactory`). Em dev, `Database:MigrateOnStartup=true` faz a API aplicar as migrations ao subir.

## 10. Checklist antes de concluir uma tarefa

- [ ] `dotnet build` sem warnings novos e `dotnet test --solution Bangboo.slnx` verde.
- [ ] Erros saem como `ApiError` via `AppException`, sem `return NotFound("...")`.
- [ ] Endpoint novo tem summary, `ProducesResponseType` e a policy correta (público só com `[AllowAnonymous]`).
- [ ] `openapi/Bangboo.Api.json` regenerado e commitado, se o contrato mudou.
- [ ] Migration criada no Core, se o schema mudou.

## 11. Dívidas conhecidas (código atual → alvo)

A API e o Core já seguem este guia. O que falta está no bot (`src/Bangboo.Bot`). Ao tocar nesses pontos, migre para o padrão acima:

- Config por guild em JSON (`Data/guild.json`, `fab.json`, `social.json`, canais/cargos fixos). Mover para o banco (`Guild`, `GuildMemberEvents`, `Mousetrap`) quando o dashboard passar a editá-las.
- `Utils/Constants/Emojis.cs` (~40 KB, com warnings de nullability) e o `ErrorCommandResultHandler` criado por reflexão no `Program.cs`.
- Namespaces inconsistentes (`Bot.*`, `Services`, `Bangboo.Utils`). Padronizar em `Bot.*` ao editar o arquivo.
- Warnings de nullability (CS8602/CS8604) em `Discord/Responders/Modal/Moderation.cs` e `Discord/Commands/Moderation.cs`.
- Deploy Discloud (`src/Bangboo.Bot/discloud.config`): o bot agora referencia `../Bangboo.Core`, então o upload precisa incluir o Core.
