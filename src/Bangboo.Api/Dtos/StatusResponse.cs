namespace Bangboo.Api.Dtos;

/// <summary>Estado atual do bot.</summary>
/// <param name="Online">O bot enviou heartbeat recentemente.</param>
/// <param name="GuildCount">Quantidade de servidores em que o bot está.</param>
/// <param name="LatencyMs">Latência do gateway no último heartbeat.</param>
/// <param name="CommandCount">Quantidade de slash commands registrados.</param>
/// <param name="StartedAt">Quando o processo do bot iniciou (null se nunca reportou).</param>
/// <param name="UpdatedAt">Último heartbeat (null se nunca reportou).</param>
public sealed record StatusResponse(
    bool Online,
    int GuildCount,
    int LatencyMs,
    int CommandCount,
    DateTimeOffset? StartedAt,
    DateTimeOffset? UpdatedAt);

/// <summary>Slash command do bot.</summary>
/// <param name="Name">Nome do comando (sem a barra).</param>
/// <param name="Description">Descrição no idioma pedido, ou a padrão (inglês) se não houver tradução.</param>
/// <param name="GuildOnly">O comando só funciona dentro de servidores (não em DMs).</param>
/// <param name="Options">Parâmetros e subcomandos, na ordem do Discord.</param>
public sealed record CommandResponse(
    string Name,
    string Description,
    bool GuildOnly,
    IReadOnlyList<CommandOptionResponse> Options);

/// <summary>Parâmetro (ou subcomando) de um slash command.</summary>
/// <param name="Name">Nome do parâmetro.</param>
/// <param name="Description">Descrição no idioma pedido, ou a padrão.</param>
/// <param name="Type">Tipo: subcommand, subcommand-group, string, integer, boolean, user, channel, role, mentionable, number, attachment.</param>
/// <param name="Required">Obrigatório ao usar o comando.</param>
/// <param name="Choices">Valores fixos aceitos (vazio = livre).</param>
/// <param name="Options">Parâmetros do subcomando (vazio para parâmetros comuns).</param>
public sealed record CommandOptionResponse(
    string Name,
    string Description,
    string Type,
    bool Required,
    IReadOnlyList<string> Choices,
    IReadOnlyList<CommandOptionResponse> Options);
