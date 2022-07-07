using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Server.DiscordServer;
using Server.Interactions.Helpers;
using TheOracle2.Commands;
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
        var characterSheet = await FollowupAsync(embeds: pcEntity.AsEmbedArray(), components: pcEntity.GetComponents()?.Build()).ConfigureAwait(false);
        pcData.MessageId = characterSheet.Id;
        return;
    }

    [SlashCommand("impacts", "Manage a player character's Impacts (SF p. 46). [equivalent to debilities]")]
    public async Task SetImpacts([Autocomplete(typeof(CharacterAutocomplete))] string character, string impact)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);
    
        pc.Impacts.Add(impact);
    
        await RespondAsync($"Impacts will update next time you trigger an interaction on that character card", ephemeral: true).ConfigureAwait(false);
    
        //await pc.RedrawCard(Context.Client);
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
        if (!int.TryParse(character, out var Id)) return;
        var pc = DbContext.PlayerCharacters.Find(Id);
        pc.XpGained = earnedXp;
        await RespondAsync($"{pc.Name}'s XP was set to **{earnedXp}**. Their card will be updated the next time you click a button on the card.", ephemeral: true).ConfigureAwait(false);
        return;
    }
    
    [SlashCommand("delete", "Delete a player character, removing them from the database and search results.")]
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
            .WithButton(GenericComponents.CancelButton())
            .WithButton("Delete", $"delete-player-{pc.Id}", style: ButtonStyle.Danger)
            .Build());
    }
}
