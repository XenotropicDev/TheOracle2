using System.Text.RegularExpressions;
using TheOracle2.GameObjects;
using TheOracle2.IronswornRoller;

namespace TheOracle2;

/// <summary>
/// Represents an Ironsworn roll, where a Score of 0-10 is compared to two Challenge Dice (d10).
/// </summary>
public abstract class IronswornRoll : IWidget, IMatchable
{
    /// <summary>
    /// Make an Ironsworn roll, where a Score of 0-10 is compared to two Challenge Dice (d10).
    /// </summary>
    /// <param name="description">A user-provided text annotation to the roll.</param>
    /// <param name="challengeDie1">A preset value for the first challenge die.</param>
    /// <param name="challengeDie2">A preset value for the second challenge die.</param>
    /// <param name="moveName">A move name to be appended to the EmbedCategory</param>
    protected IronswornRoll(Random random, string description = "", int? challengeDie1 = null, int? challengeDie2 = null, string moveName = "", string pcName = "")
    {
        Description = description;
        ChallengeDice = new ChallengeDice(random, challengeDie1, challengeDie2);

        if (!string.IsNullOrEmpty(pcName) && !string.IsNullOrEmpty(moveName))
        {
            EmbedCategory = $"{pcName} rolls {moveName}";
        }
        if (!string.IsNullOrEmpty(pcName) && string.IsNullOrEmpty(moveName))
        {
            EmbedCategory = $"{pcName} rolls";
        }
        if (!string.IsNullOrEmpty(moveName) && string.IsNullOrEmpty(pcName))
        {
            EmbedCategory = $"{EmbedCategory}: {moveName}";
        }
    }

    /// <summary>
    /// Rebuild an IronswornRoll object from an Ironsworn roll embed.
    /// </summary>
    /// <param name="embed">The embed to rebuild the roll from.</param>
    protected IronswornRoll(Random random, Embed embed)
    {
        ChallengeDice = new ChallengeDice(random, embed);
        Description = embed.Description;
        EmbedCategory = embed.Author.ToString();
    }

    public ChallengeDice ChallengeDice { get; set; }

    /// <summary>A user-provided text annotation to the roll.</summary>
    public bool IsMatch => ChallengeDice.IsMatch;

    public string Description { get; set; }

    /// <summary>The roll's score before it's reduced by the score cap.</summary>
    public abstract int RawScore { get; }

    /// <summary>The Score to be compared to the Challenge Dice, capped at 10.</summary>
    public int Score => Math.Min(10, RawScore);

    /// <summary>A Markdown string representation of the Score (and any relevant arithmetic) for use in text output.</summary>
    public virtual string ToScoreString() => $"**{Score}**";

    public static int ParseScore(string scoreFieldString)
    {
        string pattern = Regex.Escape("**") + "([0-9]|10)" + Regex.Escape("**");
        string scoreString = Regex.Match(scoreFieldString, pattern).ToString();
        if (!int.TryParse(scoreString, out int score))
        {
            throw new Exception($"Unable to parse {nameof(score)} from {scoreString}");
        }
        return score;
    }

    public abstract string ScoreLabel { get; }

    public virtual EmbedFieldBuilder ScoreField()
    {
        return new EmbedFieldBuilder()
          .WithName(ScoreLabel)
          .WithValue(ToScoreString());
    }

    /// <summary>A Markdown string representation of the roll and any user-provided text, for use in text output.</summary>
    public override string ToString()
    {
        var baseString = $"{ToScoreString()} vs. {ChallengeDice}";
        if (Description.Length > 0)
        {
            return $"{Description}\n{baseString}";
        }
        return baseString;
    }

    /// <summary>
    /// Resolve a score and two challenge dice to a roll outcome.
    /// </summary>
    public static IronswornRollOutcome Resolve(int score, ChallengeDice challengeDice)
    {
        int diceBeaten = challengeDice.ValuesLessThan(score);
        if (Enum.IsDefined(typeof(IronswornRollOutcome), diceBeaten))
        {
            return (IronswornRollOutcome)diceBeaten;
        }
        throw new Exception("Unable to resolve challenge dice into Ironsworn roll outcome.");
    }

    public static IronswornRollOutcome Resolve(int score, int challengeDie1, int challengeDie2)
    {
        if (score > challengeDie1 && score > challengeDie2)
        {
            return IronswornRollOutcome.StrongHit;
        }
        if (score <= challengeDie1 && score <= challengeDie2)
        {
            return IronswornRollOutcome.Miss;
        }
        return IronswornRollOutcome.WeakHit;
    }

    /// <summary>The outcome of the roll - a Strong Hit, Weak Hit, or Miss.</summary>
    public IronswornRollOutcome Outcome => Resolve(Score, ChallengeDice);

    /// <summary>The string representation of the roll outcome - a Strong Hit, Weak Hit, or Miss - and whether or not it's a Match.</summary>
    public string OutcomeText()
    {
        return ToOutcomeString(Outcome, IsMatch);
    }

    public Color OutcomeColor() => Outcome switch
    {
        IronswornRollOutcome.Miss => new Color(0xC50933),
        IronswornRollOutcome.WeakHit => new Color(0x842A8C),
        IronswornRollOutcome.StrongHit => new Color(0x47AEDD),
        _ => new Color(0x842A8C),
    };

    public string OutcomeIcon() => Outcome switch
    {
        IronswornRollOutcome.Miss => IronswornRollResources.MissImageURL,
        IronswornRollOutcome.WeakHit => IronswornRollResources.WeakHitImageURL,
        IronswornRollOutcome.StrongHit => IronswornRollResources.StrongHitImageURL,
        _ => IronswornRollResources.MissImageURL,
    };

    public EmbedBuilder ToEmbed()
    {
        return IWidget.EmbedStub(this)
          .WithColor(OutcomeColor())
          .WithThumbnailUrl(OutcomeIcon())
          .AddField(ScoreField())
          .AddField(ChallengeDice.ToEmbedField())
          ;
    }

    public string OverMaxMessage() => RawScore > 10 ? string.Format(IronswornRollResources.OverMaxMessage, Score) : "";

    public virtual ComponentBuilder MakeComponents()
    {
        return new ComponentBuilder()
        // .WithButton("Dummy button", "dummy", ButtonStyle.Secondary)
        ;
    }

    public string Title => OutcomeText();
    public virtual string Footer => OverMaxMessage();

    // <summary>A string description of the roll type (e.g. "Progress Roll", "Action Roll"), for use in labelling embed output.</summary>
    public virtual string EmbedCategory { get; set; } = "Roll";

    public static string ToOutcomeString(IronswornRollOutcome outcome, bool isMatch = false)
    {
        if (isMatch)
        {
            return outcome switch
            {
                IronswornRollOutcome.Miss => IronswornRollResources.MissWithAMatch,
                IronswornRollOutcome.WeakHit => IronswornRollResources.WeakHitWithAMatch,
                IronswornRollOutcome.StrongHit => IronswornRollResources.StrongHitWithAMatch,
                _ => "ERROR",
            };
        }
        return outcome switch
        {
            IronswornRollOutcome.Miss => IronswornRollResources.Miss,
            IronswornRollOutcome.WeakHit => IronswornRollResources.Weak_Hit,
            IronswornRollOutcome.StrongHit => IronswornRollResources.Strong_Hit,
            _ => "ERROR",
        };
    }

    public static readonly Dictionary<string, IEmote> Emoji = new() { { "roll", new Emoji("🎲") } };
}
