using Discord.WebSocket;

namespace TheOracle2;

internal interface ISlashCommand
{
    public void SetCommandContext(SocketSlashCommand slashCommandContext);

    IList<SlashCommandBuilder> GetCommandBuilders();
}
