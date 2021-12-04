using Discord.WebSocket;

namespace TheOracle2;

interface ISlashCommand
{
    public SocketSlashCommand Context { get; set; }

    IList<SlashCommandBuilder> GetCommandBuilders();
}
