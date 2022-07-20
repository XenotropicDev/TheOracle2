using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Server.DiscordServer;
using Server.Interactions.Helpers;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2;

[Group("player-character", "Create and manage player characters.")]
public class PlayerCharacterCommandGroup : InteractionModuleBase
{
    public PlayerCharacterCommandGroup(ApplicationContext dbContext)
    {
        DbContext = dbContext;
    }

    public ApplicationContext DbContext { get; }

    public override async Task AfterExecuteAsync(ICommandInfo command)
    {
        await DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    [SlashCommand("create", "Create a player character.")]
    public async Task BuildPlayerCard(string name, [MaxValue(4)][MinValue(1)] int edge, [MaxValue(4)][MinValue(1)] int heart, [MaxValue(4)][MinValue(1)] int iron, [MaxValue(4)][MinValue(1)] int shadow, [MaxValue(4)][MinValue(1)] int wits, string? avatarImageURL = null)
    {
        await DeferAsync();
        var message = await Context.Interaction.GetOriginalResponseAsync();

        var pcData = new PlayerCharacter(name, edge, heart, iron, shadow, wits, avatarImageURL, Context.Interaction.User.Id, Context.Interaction.GuildId ?? Context.Interaction.ChannelId, message.Id);
        DbContext.PlayerCharacters.Add(pcData);
        await DbContext.SaveChangesAsync();

        var pcEntity = new PlayerCharacterEntity(pcData);
        var characterSheet = await pcEntity.EntityAsResponse(FollowupAsync).ConfigureAwait(false);
        pcData.MessageId = characterSheet.Id;

        await DbContext.SaveChangesAsync();
        return;
    }

    [SlashCommand("impacts", "Manage a player character's Impacts (SF p. 46). [equivalent to debilities]")]
    public async Task SetImpacts([Autocomplete(typeof(CharacterAutocomplete))] string character, string impact)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);

        if (pc == null) throw new ArgumentException($"Unknown character: {id}");

        pc.Impacts.Add(impact);

        await RespondAsync($"Impacts will update next time you trigger an interaction on that character card", ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("debilities", "Manage a player character's Debilities (IS p. 36). [equivalent to impacts]")]
    public async Task DebilitiesAlias([Autocomplete(typeof(CharacterAutocomplete))] string character, string impact)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);

        pc.Impacts.Add(impact);

        await RespondAsync($"Debilities will update next time you trigger an interaction on that character card", ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("earned-xp", "Set a player character's earned xp.")]
    public async Task SetEarnedXp([Autocomplete(typeof(CharacterAutocomplete))] string character,
                                [Summary(description: "The total amount of Xp this character has earned")][MinValue(0)] int earnedXp)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = DbContext.PlayerCharacters.Find(id);
        if (pc == null) throw new ArgumentException($"Unknown character: {id}");

        pc.XpGained = earnedXp;
        await RespondAsync($"{pc.Name}'s XP was set to **{earnedXp}**. Their card will be updated the next time you click a button on the card.", ephemeral: true).ConfigureAwait(false);
        return;
    }

    [SlashCommand("delete", "Delete a player character, removing them from the database and search results.")]
    public async Task DeleteCharacter([Autocomplete(typeof(CharacterAutocomplete))] string character)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);

        if (pc == null) throw new ArgumentException($"Unknown character: {id}");

        if (pc.DiscordGuildId != Context.Guild.Id || (pc.UserId != Context.User.Id && Context.Guild.OwnerId != Context.User.Id))
        {
            await RespondAsync($"You are not allowed to delete this player character.", ephemeral: true);
        }

        await RespondAsync($"Are you sure you want to delete {pc.Name}?\nMomentum: {pc.Momentum}, xp: {pc.XpGained}\nPlayer id: {pc.Id}, last known message id: {pc.MessageId}",
            components: new ComponentBuilder()
            .WithButton(GenericComponents.CancelButton())
            .WithButton("Delete", $"delete-player-{pc.Id}", style: ButtonStyle.Danger)
            .Build());
    }
}

/// <summary>
/// Message interactions for player cards.
/// </summary>
public class PcCardComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly ILogger<PcCardComponents> logger;

    public PcCardComponents(ApplicationContext dbContext, ILogger<PcCardComponents> logger)
    {
        DbContext = dbContext;
        this.logger = logger;
    }

    //public GuildPlayer GuildPlayer => GuildPlayer.GetAndAddIfMissing(DbContext, Context);
    public ApplicationContext DbContext { get; }

    public override async Task AfterExecuteAsync(ICommandInfo command)
    {
        await DbContext.SaveChangesAsync().ConfigureAwait(false);
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

                components.WithButton(ButtonBuilder.CreateSuccessButton("+Xp", $"add-xp-{pcId}"), 2);
                components.WithButton(ButtonBuilder.CreateSecondaryButton("-Xp", $"lose-xp-{pcId}"), 2);

                components.ReplaceComponentById($"player-more-{pcId}", ButtonBuilder.CreatePrimaryButton("less", $"player-less-{pcId}").Build());

                msg.Components = components.Build();
            }).ConfigureAwait(false);
        }
        catch (HttpException httpEx)
        {
            string json = JsonConvert.SerializeObject(httpEx.Errors, Formatting.Indented);

            logger.LogError(json);
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

            logger.LogError(json);
            throw;
        }
    }

    [ComponentInteraction("delete-player-*")]
    public async Task deletePc(string pcId)
    {
        if (!int.TryParse(pcId, out var id))
        {
            throw new ArgumentException($"Unable to parse integer from {pcId}");
        }
        var pc = await DbContext.PlayerCharacters.FindAsync(id);
        if (pc == null) throw new ArgumentException($"Unknown character: {id}");

        if (pc.UserId != Context.User.Id)
        {
            await RespondAsync($"You can't delete that character", ephemeral: true);
            return;
        }

        if (pc.MessageId > 0 && pc.ChannelId > 0)
        {
            try
            {
                var channel = await Context.Client.GetChannelAsync(pc.ChannelId);
                if (channel is IMessageChannel messageChannel)
                {
                    var message = await messageChannel.GetMessageAsync(pc.MessageId);
                    await message.DeleteAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Couldn't delete character card message for PcId:{0}", pcId);
            }
        }

        DbContext.PlayerCharacters.Remove(pc);

        await DeleteOriginalResponseAsync();
    }

    private async Task UpdatePCValue(string pcId, Action<PlayerCharacter> change)
    {
        if (!int.TryParse(pcId, out var id))
        {
            throw new ArgumentException($"Unable to parse integer from {pcId}");
        }
        var pc = await DbContext.PlayerCharacters.FindAsync(id);
        if (pc == null) throw new ArgumentException($"Unknown character: {id}");

        if (pc.MessageId != Context.Interaction.Message.Id)
        {
            pc.MessageId = Context.Interaction.Message.Id;
            pc.ChannelId = Context.Interaction.Channel.Id;
        }

        change(pc);
        //GuildPlayer.LastUsedPcId = Id;

        var entity = new PlayerCharacterEntity(pc);
        await Context.Interaction.UpdateAsync(msg =>
        {
            msg.Embeds = entity.AsEmbedArray();
        }).ConfigureAwait(false);
    }
}
