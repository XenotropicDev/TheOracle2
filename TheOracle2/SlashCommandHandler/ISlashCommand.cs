using Discord.WebSocket;

namespace TheOracle2;

interface ISlashCommand
{
    public void SetCommandContext(SocketSlashCommand slashCommandContext);

    IList<SlashCommandBuilder> GetCommandBuilders();
}
