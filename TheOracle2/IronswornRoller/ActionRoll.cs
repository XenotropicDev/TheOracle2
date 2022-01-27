using System.Text.RegularExpressions;
using TheOracle2.DiscordHelpers;
using TheOracle2.GameObjects;
using TheOracle2.IronswornRoller;

namespace TheOracle2;

/// <inheritdoc/>
public class ActionRoll : IronswornRoll
{
    /// <inheritdoc/>
    /// <param name="stat">The stat value for the action roll.</param>
    /// <param name="adds">Any adds for the action roll.</param>
    /// <param name="momentum">The current Momentum of the player character.</param>
    /// <param name="actionDie">A preset value for the action die.</param>
    /// <param name="description">A user-provided text annotation to the roll.</param>
    /// <param name="challengeDie1">A preset value for the first challenge die.</param>
    /// <param name="challengeDie2">A preset value for the second challenge die.</param>
    public ActionRoll(Random random, int stat, int adds, int? momentum = null, string description = "", int? actionDie = null, int? challengeDie1 = null, int? challengeDie2 = null, string moveName = "", string pcName = "", string statName = "") : base(random, description, challengeDie1, challengeDie2, moveName, pcName)
    {
        ActionDie = new Die(random, 6, actionDie);
        Stat = stat;
        Adds = adds;
        Momentum = momentum ?? 0;
        if (!string.IsNullOrEmpty(statName))
        {
            EmbedCategory += $" +{statName}";
        }
    }

    public ActionRoll(Random random, Embed embed, int? momentum = null) : base(random, embed)
    {
        Momentum = momentum ?? 0;
        var actionScore = ParseActionScore(embed);
        ActionDie = new Die(random, 6, actionScore[0]);
        Stat = actionScore[1];
        Adds = actionScore[2];
    }

    public int Stat { get; set; }
    public int Adds { get; set; }

    /// <summary>
    /// The action die (d6) for this action roll.
    /// </summary>
    public Die ActionDie { get; set; }

    /// <summary>
    /// The current momentum of the PC rolling.
    /// </summary>
    public int Momentum { get; set; }

    /// <summary>
    /// Whether the action die is cancelled to 0 due to negative momentum.
    /// </summary>
    public bool IsActionDieCanceled => Momentum < 0 && Math.Abs(Momentum) == ActionDie;

    /// <summary>
    /// Whether burning momentum is possible (and would improve the outcome).
    /// </summary>
    public bool IsBurnable => MomentumBurnOutcome > Outcome;

    private bool _burnt;
    public bool IsBurnt { get => _burnt; set => _burnt = ((value == true & !IsBurnable) ? false : value); }

    /// <summary>
    /// The outcome that would result if Momentum were used in place of the score.
    /// </summary>
    private IronswornRollOutcome MomentumBurnOutcome => Resolve(Momentum, ChallengeDice);

    public string MomentumText()
    {
        if (IsActionDieCanceled)
        {
            return "Your action die was canceled by your negative momentum (see p. 34).";
        }

        var momentumOutcomeString = IronswornRoll.ToOutcomeString(MomentumBurnOutcome, IsMatch);
        if (IsBurnable && !IsBurnt)
        {
            return $"You may burn +{Momentum} momentum to score a {momentumOutcomeString} instead (see p. 32).";
        }
        if (IsBurnt)
        {
            var oldOutcome = IronswornRoll.Resolve(Math.Min(ActionDie.Value + Stat + Adds, 10), ChallengeDice);
            var oldOutcomeString = IronswornRoll.ToOutcomeString(oldOutcome, IsMatch);
            return $"You burned +{Momentum} momentum to improve this roll's outcome from a {oldOutcomeString} to a {OutcomeText()} (see p. 32).";
        }
        return "";
    }

    /// <inheritdoc/>
    public override string Footer => $"{base.Footer}\n{MomentumText()}";

    /// <inheritdoc/>
    public override string EmbedCategory { get; set; } = "Action Roll";

    private string ActionDieString => IsActionDieCanceled ? $"~~{ActionDie}~~" : $"{ActionDie}";

    /// <inheritdoc/>
    public EmbedFieldBuilder MomentumBurnScoreField()
    {
        return new EmbedFieldBuilder().WithName("Action Score").WithValue($"**{Momentum}**");
    }
    private string MomentumOldScoreTotalString => $"**{Math.Min(10, Stat + Adds + ActionDie.Value)}**";
    public override string ToScoreString()
    {
        string arithmetic = $"{ActionDieString} + {Stat} + {Adds}";
        if (IsBurnt)
        {
            return $"{arithmetic} = {MomentumOldScoreTotalString}";
        }
        return $"{arithmetic} = {base.ToScoreString()}";
    }

    public override EmbedFieldBuilder ScoreField()
    {
        if (IsBurnt)
        {
            return base.ScoreField().Strike();
        }
        return base.ScoreField();
    }
    /// <inheritdoc/>
    public override int RawScore
    {
        get
        {
            if (IsBurnt)
            { return Momentum; }
            if (IsActionDieCanceled)
            { return Stat + Adds; }
            return Stat + Adds + ActionDie;
        }
    }

    private const string ActionScorePattern = @"([0-9]+)";

    public static int[] ParseActionScore(string actionScoreString)
    {
        if (Regex.IsMatch(actionScoreString, ActionScorePattern))
        {
            var matchValues = Regex.Matches(actionScoreString, ActionScorePattern).Select(match =>
            {
                if (!int.TryParse(match.Value, out int value))
                { throw new Exception($"Unable to parse an integer from {match.Value}"); }
                return value;
            }).ToArray();
            return matchValues;
        }
        throw new Exception($"Unable to parse string '{actionScoreString}' in to action score.");
    }

    public static int[] ParseActionScore(EmbedField embedField)
    {
        return ParseActionScore(embedField.Value);
    }

    public override string ScoreLabel { get => _scoreLabel; }
    private const string _scoreLabel = "Action Score";

    public static int[] ParseActionScore(Embed embed)
    {
        EmbedField embedField = embed.Fields.FirstOrDefault(field => field.Name == _scoreLabel);
        return ParseActionScore(embedField);
    }

    public ButtonBuilder MomentumBurnButton(int pcId)
    {
        return new ButtonBuilder()
          .WithLabel("Burn")
          .WithCustomId($"burn-roll:{ChallengeDice[0]},{ChallengeDice[1]},{pcId}")
          .WithEmote(ActionRollEmoji["burn"])
          .WithStyle(ButtonStyle.Danger)
          ;
    }

    public ComponentBuilder MakeComponents(int pcId)
    {
        var components = base.MakeComponents();
        if (IsBurnable && !IsBurnt)
        {
            components = components.WithButton(MomentumBurnButton(pcId));
        }
        return components;
    }

    public static readonly Dictionary<string, IEmote> ActionRollEmoji = new() { { "burn", new Emoji("🔥") } };
    /// <summary>
    /// Attempts to burn the  momentum on this ActionRoll; sets momentum on the PlayerCharacter if it succeeds, and returns the PlayerCharacter.
    /// </summary>
    public PlayerCharacter BurnMomentum(PlayerCharacter pcData)
    {
        Momentum = pcData.Momentum;
        if (!IsBurnable)
        {
            throw new Exception($"Unable to burn {Momentum} momentum because it does not beat any challenge dice values ({ChallengeDice})");
        }
        // this shouldn't happen normally, but if something goes wrong it might make it easier to diagnose where the math is incorrect.
        if (!pcData.ResetMomentum())
        {
            throw new Exception($"Unable to burn {Momentum} momentum. Momentum of less than {PlayerCharacter.MinMomentumToBurn} can't cancel any challenge die result.");
        }
        IsBurnt = true;
        return pcData;
    }
    public override EmbedBuilder ToEmbed()
    {
        if (IsBurnt)
        {
            var embed = base.ToEmbed();
            embed.Fields.Clear();
            embed
            .AddField(
                ScoreField()
                .WithIsInline(true))
            .AddField(
                MomentumBurnScoreField()
                .WithIsInline(true))
            .AddField(
                ChallengeDice.ToEmbedField()
            );
            return embed;
        }
        return base.ToEmbed();
    }
}
