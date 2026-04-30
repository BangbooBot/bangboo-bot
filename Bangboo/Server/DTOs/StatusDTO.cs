namespace Bangboo.Server.DTOs;

public class StatusReponse
{
    public int ServerCount { get; set; }

    public int CommandsCount { get; set; }

    public StatusReponse(int serverCount, int commandsCount)
    {
        ServerCount = serverCount;
        CommandsCount = commandsCount;
    }
}

public class CommandReponse
{
    public string Name { get; set; }
    public string Description { get; set; }
}