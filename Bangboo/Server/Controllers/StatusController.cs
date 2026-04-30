using System.Collections;
using Bangboo.Server.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Bangboo.Server.Controllers;

[ApiController]
[Route("/status")]
public class Status : ControllerBase
{
    private readonly DatabaseService _databaseService;
    private readonly DiscordService _discordService;
    
    public Status(DatabaseService databaseService, DiscordService discordService)
    {
        _databaseService = databaseService;
        _discordService = discordService;
    }

    /// <summary>
    /// Get discord bot status
    /// </summary>
    /// <remarks>
    /// Return discord bot guild count and slash command count
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(StatusReponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Get()
    {
        /*
         * var commandsCount = Assembly.GetExecutingAssembly().GetTypes().SelectMany(type => type.GetMethods())
            .Count(method => method.GetCustomAttribute<SlashCommandAttribute>() is not null);
         */
        var res = new StatusReponse(
            _discordService.Gateway?.Cache.Guilds.Count ?? 0, 
            _discordService.SlashCommands.Count
        );
        return Ok(res);
    }
    
    /// <summary>
    /// Get discord bot commands
    /// </summary>
    /// <remarks>
    /// Return a list of slash commands
    /// </remarks>
    [HttpGet("commands")]
    [ProducesResponseType(typeof(IAsyncEnumerable<CommandReponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.None, NoStore = true)]
    public async IAsyncEnumerable<CommandReponse> GetCommands()
    {
        foreach (var command in _discordService.SlashCommands)
        {
            yield return new CommandReponse
            {
                Name = command?.Name,
                Description = command.Description
            };
        }
    }
}