using System.Text.RegularExpressions;
using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.DiscordHelpers;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2;

[Group("roll", "Make an action roll (p. 28) or progress roll (p. 39). For oracle tables, use '/oracle'")]

public class RollCommand : InteractionModuleBase
{
    public RollCommand(Random random, EFContext efContext)
    {
        Random = random;
        EfContext = efContext;
    }
    public Random Random { get; }
    public EFContext EfContext { get; }
    public GuildPlayer GuildPlayer => GuildPlayer.AddIfMissing(Context, EfContext);

    public override async Task AfterExecuteAsync(ICommandInfo command)
    {
        await EfContext.SaveChangesAsync().ConfigureAwait(false);
    }

    [SlashCommand("action-pc", "Make an Ironsworn action roll (p. 28) for a player character.")]
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

        if (!int.TryParse(character, out var id))
        {
            await RespondAsync("Unknown character ID", ephemeral: true);
            return;
        }
        pcData = id == -1 ? GuildPlayer.LastUsedPc(EfContext) : EfContext.PlayerCharacters.Find(id);
        if (pcData == null)
        {
            string errorMessage = id == -1 ? "I couldn't find a recently used player character for you on this server." : $"I couldn't find a character with an Id of {id} on this server.";

            errorMessage += "If you want to create a character, use the `/player` command.";

            var fallbackPcs = GuildPlayer.GetPcs(EfContext);
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
                    )
                    ;
                }
            }
            await RespondAsync(errorMessage, components: components?.Build(), ephemeral: true).ConfigureAwait(false);
            return;
        }

        var pc = new PlayerCharacterEntity(pcData);
        var roll = pc.RollAction(this.Random, stat, adds, description, actionDie, challengeDie1, challengeDie2);
        var embed = roll.ToEmbed();
        if (pcData.MessageId > 0)
        {
            var characterSheet = await Task.Run(() => pc.GetDiscordMessage(Context));
            embed.Author.Url = characterSheet.GetJumpUrl();
        }
        GuildPlayer.LastUsedPcId = pcData.Id;
        await RespondAsync(embed: roll.ToEmbed().Build(), components: roll.MakeComponents(pcData.Id)?.Build()).ConfigureAwait(false);
    }

    private int GetStatValue(RollableStats stat, PlayerCharacter pc)
    {
        return stat switch
        {
            RollableStats.Edge => pc.Edge,
            RollableStats.Heart => pc.Heart,
            RollableStats.Iron => pc.Iron,
            RollableStats.Shadow => pc.Shadow,
            RollableStats.Wits => pc.Wits,
            RollableStats.Health => pc.Health,
            RollableStats.Spirit => pc.Spirit,
            RollableStats.Supply => pc.Supply,
            _ => throw new NotImplementedException(),
        };
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
    [SlashCommand("progress", "Roll with a set progress score (p. 39). For an interactive progress tracker, use /progress-track")]
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


public class PCRollComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public PCRollComponents(Random random, EFContext efContext)
    {
        EfContext = efContext;
        Random = random;
    }
    public GuildPlayer GuildPlayer => GuildPlayer.AddIfMissing(Context, EfContext);
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
            throw new Exception($"Unable to parse entity ID from {Context.Interaction.Data.CustomId}");
        }
        var pcData = await EfContext.PlayerCharacters.FindAsync(pcId);
        var pcEntity = new PlayerCharacterEntity(pcData);

        var description = Context.Interaction.Message.Content;
        description = Regex.Match(description, @"\((.*)\):$")?.Groups[1].Value ?? "";
        var stat = Enum.Parse<RollableStats>(statString);
        var adds = int.Parse(addsString);

        int? actionDie = !string.IsNullOrEmpty(actionDieString) ? int.Parse(actionDieString) : null;
        int? challengeDie1 = !string.IsNullOrEmpty(challengeDie1String) ? int.Parse(challengeDie1String) : null;
        int? challengeDie2 = !string.IsNullOrEmpty(challengeDie2String) ? int.Parse(challengeDie2String) : null;


        var roll = pcEntity.RollAction(this.Random, stat, adds, description, actionDie, challengeDie1, challengeDie2);

        var embed = roll.ToEmbed();
        var characterSheet = await Task.Run(() => pcEntity.GetDiscordMessage(Context));

        embed.Author.Url = characterSheet.GetJumpUrl();

        GuildPlayer.LastUsedPcId = pcEntity.Pc.Id;

        await FollowupAsync(embed: roll.ToEmbed().Build(), components: roll.MakeComponents().Build()).ConfigureAwait(false);
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

        var pc = EfContext.PlayerCharacters.Find(Id);

        var roll = new ActionRoll(Random, 0, 0, 0, $"{embed.Description}\n{pc.Name} burned {pc.Momentum} momentum to change this roll result", 1, die1Val, die2Val)
        {
            ActionDie = new Die(Random, 10, pc.Momentum)
        };

        pc.BurnMomentum();
        GuildPlayer.LastUsedPcId = pc.Id;
        await EfContext.SaveChangesAsync();

        if (pc.ChannelId == 0 || pc.MessageId == 0)
        {
            //Modify the message, but don't use FollowupAsync so we can reply with the ephemeral message
            await Context.Interaction.ModifyOriginalResponseAsync(msg =>
            {
                msg.Embed = roll.ToEmbed().WithAuthor(embed.Author?.ToEmbedAuthorBuilder()).Build();
                msg.Components = new ComponentBuilder().Build();
            });
            await FollowupAsync($"I couldn't find the character card to update, but it should update the next time you click a button on that card", ephemeral: true);
            return;
        }

        var entity = new PlayerCharacterEntity(pc);

        IMessageChannel channel = (pc.ChannelId == Context.Channel.Id) ? Context.Channel : await Context.Client.Rest.GetChannelAsync(pc.ChannelId) as IMessageChannel;
        await channel.ModifyMessageAsync(pc.MessageId, msg => msg.Embeds = entity.GetEmbeds());

        await FollowupAsync(
            embed: roll.ToEmbed().WithAuthor(embed.Author?.ToEmbedAuthorBuilder()).Build(),
            components: new ComponentBuilder().Build()
        );
    }
}

public enum RollableStats
{
    Edge,
    Heart,
    Iron,
    Shadow,
    Wits,
    Health,
    Spirit,
    Supply
}
