using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using TheOracle2.Commands;
using TheOracle2.UserContent;

namespace TheOracle2;

[Group("edit-player", "Edits player stats")]
public class EditPlayerPaths : InteractionModuleBase
{
    public EFContext DbContext { get; }

    public EditPlayerPaths(EFContext dbContext)
    {
        DbContext = dbContext;
    }

    [SlashCommand("add-impact", "Changes the impacts on a character")]
    public async Task SetImpacts([Autocomplete(typeof(CharacterAutocomplete))] string character, string impact)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);

        pc.Impacts.Add(impact);
        await DbContext.SaveChangesAsync();

        await RespondAsync($"Impacts will update next time you trigger an interaction on that character card", ephemeral: true);

        //await pc.RedrawCard(Context.Client);
    }

    [SlashCommand("earned-xp", "Changes the earned xp on a character")]
    public async Task SetEarnedXp([Autocomplete(typeof(CharacterAutocomplete))] string character,
                                [Summary(description: "The total amount of Xp this character has earned")][MinValue(0)] int earnedXp)
    {
    }

    [SlashCommand("delete-character", "Removes the character from the character search results")]
    public async Task DeleteCharacter([Autocomplete(typeof(CharacterAutocomplete))] string character)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);

        if (pc.DiscordGuildId != Context.Guild.Id || (pc.UserId != Context.User.Id && Context.Guild.OwnerId != Context.User.Id))
        {
            await RespondAsync($"You are not allowed to delete this player character.", ephemeral: true);
        }

        await RespondAsync($"Are you sure you want to delete {pc.Name}?",
            components: new ComponentBuilder()
            .WithButton(GenericComponentHandlers.CancelButton())
            .WithButton("Delete", $"delete-player-{pc.Id}", style: ButtonStyle.Danger)
            .Build());
    }
}

public class EditPlayerComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly ILogger<EditPlayerComponents> logger;

    public EditPlayerComponents(EFContext efContext, ILogger<EditPlayerComponents> logger)
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
        if (pc.MessageId > 0)
        {
            try
            {
                var msg = await ((await Context.Client.GetChannelAsync(pc.ChannelId)) as IMessageChannel).GetMessageAsync(pc.MessageId);
                await msg.DeleteAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Unable to delete player card post for player id {pcId}:\n{ex}");
                errors.Add($"`Unable to delete player card post (this does not mean the character wasn't removed from command search results)`");
            }
        }
        else
        {
            errors.Add("`Unable to delete player card post (this does not mean the character wasn't removed from command search results)`");
        }

        try
        {
            EfContext.PlayerCharacters.Remove(pc);
            await EfContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError($"Unable to delete player from database. Id: {pcId}\n{ex}");
            errors.Add($"`Unable to player from database, please try again, and post an issue on discord/github if it continues.`");
        }

        await Context.Interaction.Message.DeleteAsync();

        string message = (errors.Count == 0) ? $"Deleted {pc.Name}" : $"Finished with error(s):\n{string.Join('\n', errors)}";
        await FollowupAsync(message, ephemeral: true);
    }
}