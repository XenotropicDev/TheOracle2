using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using TheOracle2.Commands;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2;

[Group("player-character", "Create and manage player characters.")]
public class EditPlayerPaths : InteractionModuleBase
{
    public EditPlayerPaths(EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public EFContext DbContext { get; }

    public GuildPlayer GuildPlayer => GuildPlayer.AddIfMissing(Context, DbContext);

    public override void AfterExecute(ICommandInfo command)
    {
        DbContext.SaveChanges();
    }


    [SlashCommand("create", "Create a player character.")]
    public async Task BuildPlayerCard(string name, [MaxValue(4)][MinValue(1)] int edge, [MaxValue(4)][MinValue(1)] int heart, [MaxValue(4)][MinValue(1)] int iron, [MaxValue(4)][MinValue(1)] int shadow, [MaxValue(4)][MinValue(1)] int wits)
    {
        await DeferAsync();
        var pc = new PlayerCharacter(Context as SocketInteractionContext, name, edge, heart, iron, shadow, wits);
        DbContext.PlayerCharacters.Add(pc);
        await DbContext.SaveChangesAsync();

        // AfterExecute does SaveChanges, but the PC has to be saved to the DB to get an Id.
        GuildPlayer.LastUsedPcId = pcData.Id;

        var pc = new PlayerCharacterEntity(pcData);
        await FollowupAsync(embeds: pc.GetEmbeds(), components: pc.GetComponents()).ConfigureAwait(false);
        return;
    }
    [SlashCommand("impacts", "Manage a player character's Impacts (p. 46)")]
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

    [SlashCommand("delete", "Removes the character from the character search results")]
    public async Task DeleteCharacter([Autocomplete(typeof(CharacterAutocomplete))] string character)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);

        if (pc.DiscordGuildId != Context.Guild.Id || (pc.UserId != Context.User.Id && Context.Guild.OwnerId != Context.User.Id))
        {
            await RespondAsync($"You are not allowed to delete this player character.", ephemeral: true);
        }

        await RespondAsync($"Are you sure you want to delete {pc.Name}?\nMomentum: {pc.Momentum}, xp: {pc.XpGained}\nPlayer id: {pc.Id}, last known message id: {pc.MessageId}",
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


public class PlayerCardComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public PlayerCardComponents(EFContext dbContext)
    {
        DbContext = dbContext;
    }
    public GuildPlayer GuildPlayer => GuildPlayer.AddIfMissing(Context, DbContext);
    public EFContext DbContext { get; }
    public override void AfterExecute(ICommandInfo command)
    {
        DbContext.SaveChanges();
    }
    [ComponentInteraction("add-momentum-*")]
    public async Task AddMomentum(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Momentum++).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-momentum-*")]
    public async Task LoseMomentum(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Momentum--).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-supply-*")]
    public async Task loseSupply(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Supply--).ConfigureAwait(false);
    }

    [ComponentInteraction("add-supply-*")]
    public async Task addSupply(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Supply++).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-health-*")]
    public async Task loseHealth(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Health--).ConfigureAwait(false);
    }

    [ComponentInteraction("add-health-*")]
    public async Task addHealth(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Health++).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-spirit-*")]
    public async Task loseSpirit(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Spirit--).ConfigureAwait(false);
    }

    [ComponentInteraction("add-spirit-*")]
    public async Task addSpirit(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Spirit++).ConfigureAwait(false);
    }

    [ComponentInteraction("add-xp-*")]
    public async Task addXp(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.XpGained++).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-xp-*")]
    public async Task loseXp(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.XpGained--).ConfigureAwait(false);
    }

    [ComponentInteraction("burn-momentum-*")]
    public async Task burn(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.BurnMomentum()).ConfigureAwait(false);
    }

    [ComponentInteraction("player-more-*")]
    public async Task toggleMore(string pcId)
    {
        try
        {
            await Context.Interaction.UpdateAsync(msg =>
            {
                ComponentBuilder components = ComponentBuilder.FromMessage(Context.Interaction.Message);

                components.TryAdd(ButtonBuilder.CreateSuccessButton("+Xp", $"add-xp-{pcId}").Build(), 2);
                components.TryAdd(ButtonBuilder.CreateSecondaryButton("-Xp", $"lose-xp-{pcId}").Build(), 2);

                components.ReplaceComponentById($"player-more-{pcId}", ButtonBuilder.CreatePrimaryButton("less", $"player-less-{pcId}").Build());

                msg.Components = components.Build();
            }).ConfigureAwait(false);
        }
        catch (HttpException hex)
        {
            string json = JsonConvert.SerializeObject(hex.Errors, Formatting.Indented);

            Console.WriteLine(json);
            throw;
        }
    }

    [ComponentInteraction("player-less-*")]
    public async Task toggleLess(string pcId)
    {
        try
        {
            await Context.Interaction.UpdateAsync(msg =>
            {
                ComponentBuilder components = ComponentBuilder.FromMessage(Context.Interaction.Message)
                .RemoveComponentById($"add-xp-{pcId}")
                .RemoveComponentById($"lose-xp-{pcId}");

                components.ReplaceComponentById($"player-less-{pcId}", ButtonBuilder.CreatePrimaryButton("...", $"player-more-{pcId}").Build());

                msg.Components = components.Build();
            }).ConfigureAwait(false);
        }
        catch (HttpException hex)
        {
            string json = JsonConvert.SerializeObject(hex.Errors, Formatting.Indented);
            Console.WriteLine(json);
            throw;
        }
    }

    private async Task UpdatePCValue(string pcId, Action<PlayerCharacter> change)
    {
        if (!int.TryParse(pcId, out var Id)) return;

        var pc = await DbContext.PlayerCharacters.FindAsync(Id);
        if (pc.MessageId != Context.Interaction.Message.Id)
        {
            pc.MessageId = Context.Interaction.Message.Id;
            pc.ChannelId = Context.Interaction.Channel.Id;
        }
        change(pc);
        GuildPlayer.LastUsedPcId = Id;
        // await DbContext.SaveChangesAsync();
        // TODO: commenting out the above to see what breaks.

        var entity = new PlayerCharacterEntity(pc);
        await Context.Interaction.UpdateAsync(msg =>
        {
            msg.Embeds = entity.GetEmbeds();
        }).ConfigureAwait(false);
    }
}
