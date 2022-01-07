using Discord.Interactions;
using TheOracle2.UserContent;

namespace TheOracle2.Commands;

public class RightClickCommands : InteractionModuleBase<SocketInteractionContext>
{
    public RightClickCommands(EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public EFContext DbContext { get; }

    [MessageCommand("Recreate Message")]
    public async Task MoveToBottom(IMessage msg)
    {
        if (msg.Author.Id != Context.Client.CurrentUser.Id) await RespondAsync($"I can't recreate that message", ephemeral: true);

        await DeferAsync();

        var builder = ComponentBuilder.FromMessage(msg);
        var content = msg.Content?.Length > 0 ? msg.Content : null;
        await FollowupAsync(content, embeds: msg.Embeds.OfType<Embed>().ToArray(), components: builder.Build()).ConfigureAwait(false);

        var pc = DbContext.PlayerCharacters.FirstOrDefault(pc => pc.MessageId == msg.Id);
        if (pc != null)
        {
            pc.MessageId = msg.Id;
            await DbContext.SaveChangesAsync();
        }

        await msg.DeleteAsync().ConfigureAwait(false);
    }
}
