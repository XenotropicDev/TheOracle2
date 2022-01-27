using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using TheOracle2.Commands;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2;

public class EditPcComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly ILogger<EditPcComponents> logger;

    public EditPcComponents(EFContext efContext, ILogger<EditPcComponents> logger)
    {
        EfContext = efContext;
        this.logger = logger;
    }

    public EFContext EfContext { get; }

    [ComponentInteraction("delete-player-*")]
    public async Task DeletePlayer(string pcId)
    {
        await DeferAsync();

        if (!int.TryParse(pcId, out var id)) return;
        var pc = await EfContext.PlayerCharacters.FindAsync(id);

        if (pc == null)
        {
            await RespondAsync($"I couldn't find that character, is it maybe already deleted?", ephemeral: true);
            return;
        }

        if (pc.DiscordGuildId != Context.Guild.Id || (pc.UserId != Context.User.Id && Context.Guild.OwnerId != Context.User.Id))
        {
            await RespondAsync($"You are not allowed to delete this player character.", ephemeral: true);
            return;
        }

        var errors = new List<string>();
        try
        {
            EfContext.PlayerCharacters.Remove(pc);
            await EfContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError($"Unable to delete player from database. Id: {pcId}\n{ex}");
            await FollowupAsync($"Unable to delete player {pc.Name} from the player database. Please try again, then make an issue on github, or post on the Ironsworn discord.", ephemeral: true);
            return;
        }

        if (pc.MessageId > 0)
        {
            try
            {
                var msg = await ((await Context.Client.GetChannelAsync(pc.ChannelId)) as IMessageChannel)?.GetMessageAsync(pc.MessageId);
                await msg?.DeleteAsync(); //The message could've been deleted by the user.
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Unable to delete player card post for player id {pcId}:\n{ex}");
                errors.Add($"`The player was deleted from the database, but I was unable to delete player card post. If you already deleted the post you can ignore this message, otherwise you might want to delete the post yourself.`");
            }
        }
        else
        {
            errors.Add("`Unable to delete player card post (this does not mean the character wasn't removed from command search results)`");
        }

        await Context.Interaction.Message.DeleteAsync();

        string message = (errors.Count == 0) ? $"Deleted {pc.Name}" : $"Finished with error(s):\n{string.Join('\n', errors)}";
        await FollowupAsync(message, ephemeral: true);
    }
}
