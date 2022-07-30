using Discord.Interactions;
using Discord.WebSocket;
using Server.DiscordServer;
using Server.Interactions;
using TheOracle2.UserContent;

namespace TheOracle2.Commands;

public class RightClickCommands : InteractionModuleBase<SocketInteractionContext<SocketMessageCommand>>
{
    public RightClickCommands(ApplicationContext dbContext)
    {
        DbContext = dbContext;
    }

    public ApplicationContext DbContext { get; }

    [MessageCommand("Recreate Message")]
    public async Task MoveToBottom(IMessage msg)
    {
        if (msg.Author.Id != Context.Client.CurrentUser.Id) await RespondAsync($"I can't recreate that message", ephemeral: true);

        var builder = ComponentBuilder.FromMessage(msg);
        var content = msg.Content?.Length > 0 ? msg.Content : null;
        await RespondAsync(content, embeds: msg.Embeds.OfType<Embed>().ToArray(), components: builder.Build()).ConfigureAwait(false);

        var pc = DbContext.PlayerCharacters.FirstOrDefault(pc => pc.MessageId == msg.Id);
        if (pc != null)
        {
            pc.MessageId = msg.Id;
            await DbContext.SaveChangesAsync();
        }

        await msg.DeleteAsync().ConfigureAwait(false);
    }
}
