﻿using System.Text.RegularExpressions;
using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.DiscordHelpers;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2;

[Group("roll", "Make an action roll (p. 28) or progress roll (p. 39). For oracle tables, use '/oracle'")]
public class RollCommandGroup : InteractionModuleBase
{
    public RollCommandGroup(Random random, EFContext efContext)
    {
        Random = random;
        EfContext = efContext;
    }

    public Random Random { get; }
    public EFContext EfContext { get; }
    public GuildPlayer GetGuildPlayer() => GuildPlayer.GetAndAddIfMissing(EfContext, Context);

    public override async Task AfterExecuteAsync(ICommandInfo command)
    {
        await EfContext.SaveChangesAsync().ConfigureAwait(false);
    }

    [SlashCommand("action-pc", "Make an action roll (p. 28) for a player character.")]
    public async Task RollPcAction(
        [Summary(description: "The stat value to use for the roll")] RollableStats stat,
        [Summary(description: "Any adds to the roll")][MinValue(0)] int adds,
        [Summary(description: "The character to use for the roll. Leave this blank to use the last PC you interacted with.")]
        // -1 is used to represent "last used character"
        [Autocomplete(typeof(CharacterAutocomplete))] string character = "-1",
        [Summary(description: "Any notes, fiction, or other text you'd like to include with the roll")] string description = "",
        [Summary(description: "A preset value for the Action Die (d6) to use instead of rolling.")][MinValue(1)][MaxValue(6)] int? actionDie = null,
        [Summary(description: "A preset value for the first Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie1 = null,
        [Summary(description: "A preset value for the second Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie2 = null)
    {
        PlayerCharacter pcData = null;
        GuildPlayer GuildPlayer = this.GetGuildPlayer();

        if (!int.TryParse(character, out var id))
        {
            await RespondAsync($"Unknown character ID: {character}", ephemeral: true);
            throw new ArgumentException($"Unable to parse PlayerCharacter ID from {character}.");
        }
        pcData = id == -1 ? GuildPlayer.LastUsedPc() : EfContext.PlayerCharacters.Find(id);
        if (pcData == null)
        {
            await OfferActionRollFallbackPcs(id, stat, adds, description, actionDie, challengeDie1, challengeDie2);
            return;
        }
        var pcEntity = new PlayerCharacterEntity(EfContext, pcData);
        var roll = await pcEntity.RollAction(Context, this.Random, stat, adds, description, actionDie, challengeDie1, challengeDie2);
        GuildPlayer.LastUsedPcId = pcData.Id;
        await RespondAsync(embed: roll.ToEmbed().Build(), components: roll.MakeComponents(pcData.Id)?.Build()).ConfigureAwait(false);
    }

    public async Task OfferActionRollFallbackPcs(
        int id,
        RollableStats stat,
        int adds,
        string description = "",
        int? actionDie = null,
        int? challengeDie1 = null,
        int? challengeDie2 = null
    )
    {
        string errorMessage = id == -1 ? "I couldn't find a recently used player character for you on this server." : $"I couldn't find a character with an Id of {id} on this server.";

        errorMessage += "If you want to create a character, use the `/player` command.";

        var fallbackPcs = GetGuildPlayer().GetPcs();
        ComponentBuilder components = null;
        if (fallbackPcs.Any())
        {
            components = new ComponentBuilder();
            errorMessage += $"\n\nOr select from one of your characters below to roll +{stat}";
            if (!string.IsNullOrEmpty(description))
            {
                errorMessage += $" ({description})";
            }
            errorMessage += ":";
            if (fallbackPcs.Count() <= 5)
            {
                foreach (PlayerCharacter fallbackPc in fallbackPcs)
                {
                    components.WithButton(fallbackPc.Name, $"finish-action-roll:{fallbackPc.Id},{stat},{adds},{actionDie},{challengeDie1},{challengeDie2}");
                }
            }
            if (fallbackPcs.Count() > 5)
            {
                var menu = new SelectMenuBuilder()
                .WithCustomId($"finish-action-roll-menu:{stat},{adds},{actionDie},{challengeDie1},{challengeDie2}")
                .WithOptions(
                    fallbackPcs.Take(25).Select(fallbackPc => new SelectMenuOptionBuilder(
                        fallbackPc.Name,
                        fallbackPc.Id.ToString()
                    )).ToList()
                );
            }
        }
        await RespondAsync(errorMessage, components: components?.Build(), ephemeral: true).ConfigureAwait(false);
    }
    [SlashCommand("action", "Make an action roll (p. 28) by setting a stat value.")]
    public async Task RollAction(
        [Summary(description: "The stat value to use for the roll")] int stat,
        [Summary(description: "Any adds to the roll")][MinValue(0)] int adds,
        [Summary(description: "The player character's momentum.")][MinValue(-6)][MaxValue(10)] int momentum,
        [Summary(description: "Any notes, fiction, or other text you'd like to include with the roll")] string description = "",
        [Summary(description: "A preset value for the Action Die (d6) to use instead of rolling.")][MinValue(1)][MaxValue(6)] int? actionDie = null,
        [Summary(description: "A preset value for the first Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie1 = null,
        [Summary(description: "A preset value for the second Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie2 = null)
    {
        var roll = new ActionRoll(Random, stat, adds, momentum, description, actionDie, challengeDie1, challengeDie2);
        await RespondAsync(embed: roll.ToEmbed().Build()).ConfigureAwait(false);
    }
    [SlashCommand("progress", "Roll with a set progress score (p. 39). For an interactive progress tracker, use /progress-track.")]
    public async Task RollProgress(
        [Summary(description: "The progress score.")] int progressScore,
        [Summary(description: "A preset value for the first Challenge Die to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie1 = null,
        [Summary(description: "A preset value for the second Challenge Die to use instead of rolling")][MinValue(1)][MaxValue(10)] int? challengeDie2 = null,
        [Summary(description: "Notes, fiction, or other text to include with the roll.")] string description = "")
    {
        var roll = new ProgressRoll(Random, progressScore, description, challengeDie1, challengeDie2);
        await RespondAsync(embed: roll.ToEmbed().Build()).ConfigureAwait(false);
    }
}

public class PcRollComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public PcRollComponents(Random random, EFContext efContext)
    {
        EfContext = efContext;
        Random = random;
    }
    public GuildPlayer GetGuildPlayer() => GuildPlayer.GetAndAddIfMissing(EfContext, Context);
    public Random Random { get; }
    public EFContext EfContext { get; }

    public override async Task AfterExecuteAsync(ICommandInfo command)
    {
        await EfContext.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Provides fallback options when the player character action roll command receives an invalid id.
    /// </summary>
    [ComponentInteraction("finish-action-roll:*,*,*,*,*,*")]
    public async Task FinishActionRoll
        (string pcIdString, string statString, string addsString, string actionDieString, string challengeDie1String, string challengeDie2String)
    {
        await DeferAsync();
        if (!int.TryParse(pcIdString, out var pcId))
        {
            throw new ArgumentException($"Unable to parse entity ID from {Context.Interaction.Data.CustomId}");
        }
        var pcData = await EfContext.PlayerCharacters.FindAsync(pcId);
        var pcEntity = new PlayerCharacterEntity(EfContext, pcData);

        var description = Context.Interaction.Message.Content;
        description = Regex.Match(description, @"\((.*)\):$")?.Groups[1].Value ?? "";
        var stat = Enum.Parse<RollableStats>(statString);
        var adds = int.Parse(addsString);

        int? actionDie = !string.IsNullOrEmpty(actionDieString) ? int.Parse(actionDieString) : null;
        int? challengeDie1 = !string.IsNullOrEmpty(challengeDie1String) ? int.Parse(challengeDie1String) : null;
        int? challengeDie2 = !string.IsNullOrEmpty(challengeDie2String) ? int.Parse(challengeDie2String) : null;

        var roll = await pcEntity.RollAction(Context, this.Random, stat, adds, description, actionDie, challengeDie1, challengeDie2);

        var rollEmbed = roll.ToEmbed();

        GetGuildPlayer().LastUsedPcId = pcEntity.Pc.Id;

        await FollowupAsync(embed: rollEmbed.Build(), components: roll.MakeComponents().Build()).ConfigureAwait(false);
        return;
    }

    /// <summary>
    /// Provides fallback options when the player character action roll command receives an invalid id.
    /// </summary>
    [ComponentInteraction("finish-action-roll-menu:*,*,*,*,*")]
    public async Task FinishActionRoll(string statString, string addsString, string actionDieString, string challengeDie1String, string challengeDie2String)
    {
        var selectedValue = Context.Interaction.Data.Values.FirstOrDefault();
        await FinishActionRoll(selectedValue, statString, addsString, actionDieString, challengeDie1String, challengeDie2String);
    }

    // TODO: for compatibility with earlier alpha buttons. remove by first release.
    [ComponentInteraction("burn-roll-*,*,*")]
    public async Task BurnFromRollLegacy(string Die1, string Die2, string pcId)
    {
        await BurnFromRoll(Die1, Die2, pcId);
    }

    [ComponentInteraction("burn-roll:*,*,*")]
    public async Task BurnFromRoll(string Die1, string Die2, string pcId)
    {
        await DeferAsync();

        var embed = Context.Interaction.Message.Embeds?.FirstOrDefault();
        if (embed == null || !int.TryParse(pcId, out var Id) || !int.TryParse(Die1, out var die1Val) || !int.TryParse(Die2, out var die2Val))
        {
            await FollowupAsync($"I couldn't burn your momentum, please try doing it from the character card.", ephemeral: true);
            return;
        }

        var pcData = EfContext.PlayerCharacters.Find(Id);

        var roll = new ActionRoll(Random, embed, pcData.Momentum);

        pcData.BurnMomentum(roll);

        GetGuildPlayer().LastUsedPcId = pcData.Id;
        await EfContext.SaveChangesAsync();

        if (pcData.ChannelId == 0 || pcData.MessageId == 0)
        {
            //Modify the message, but don't use FollowupAsync so we can reply with the ephemeral message
            await Context.Interaction.ModifyOriginalResponseAsync(msg =>
            {
                msg.Embed = roll.ToEmbed().Build();
                msg.Components = roll.MakeComponents().Build();
            });
            await FollowupAsync($"I couldn't find the character card to update, but it should update the next time you click a button on that card", ephemeral: true);
            return;
        }

        var pcEntity = new PlayerCharacterEntity(EfContext, pcData);

        IMessageChannel channel = (pcData.ChannelId == Context.Channel.Id) ? Context.Channel : await Context.Client.Rest.GetChannelAsync(pcData.ChannelId) as IMessageChannel;
        await channel.ModifyMessageAsync(pcData.MessageId, msg => msg.Embeds = pcEntity.GetEmbeds());

        await Context.Interaction.ModifyOriginalResponseAsync(msg =>
        {
            msg.Embed = roll.ToEmbed().Build();
            msg.Components = roll.MakeComponents().Build();
        });
    }
}
