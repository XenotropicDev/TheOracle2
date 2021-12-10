using Discord.WebSocket;

namespace TheOracle2;

interface ISlashCommand
{
    public SocketSlashCommand SlashCommandContext { get; set; }

    IList<SlashCommandBuilder> GetCommandBuilders();
}
