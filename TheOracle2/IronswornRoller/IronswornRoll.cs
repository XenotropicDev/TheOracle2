using TheOracle2.GameObjects;
using TheOracle2.IronswornRoller;

namespace TheOracle2;

public abstract class IronswornRoll
{
  /// <summary>
  /// Represents an Ironsworn roll, where a Score of 0-10 is compared to two Challenge Dice (d10).
  /// </summary>
  /// <param name="text">A user-provided text annotation to the roll.</param>
  /// <param name="challengeDieValue1">A preset value for the first challenge die.</param>
  /// <param name="challengeDieValue2">A preset value for the second challenge die.</param>
  public IronswornRoll(Random random, string text = "", int? challengeDieValue1 = null, int? challengeDieValue2 = null)
  {
    Random = random;
    Text = text;
    ChallengeDie1 = new Die(Random, 10, challengeDieValue1 ?? null);
    ChallengeDie2 = new Die(Random, 10, challengeDieValue2 ?? null);
  }

  public Random Random { get; }

  /// <summary>A user-provided text annotation to the roll.</summary>
  public string Text { get; }

  /// <summary>The first Challenge Die.</summary>
  public Die ChallengeDie1 { get; set; }

  /// <summary>The second Challenge Die.</summary>
  public Die ChallengeDie2 { get; set; }

  /// <summary>Whether the Value properties of the Challenge Dice match.</summary>
  public bool IsMatch { get => ChallengeDie1.Value == ChallengeDie2.Value; }

  /// <summary>The roll's score before it's reduced by the score cap.</summary>
  public abstract int RawScore { get; }

  /// <summary>The Score to be compared to the Challenge Dice, capped at 10.</summary>
  public int Score { get => Math.Min(10, RawScore); }

  /// <summary>A Markdown string representation of the Challenge Dice values for use in text output.</summary>
  public string ToChallengeString() => $"{ChallengeDie1.Value}, {ChallengeDie2.Value}";

  /// <summary>A Markdown string representation of the Score (and any relevant arithmetic) for use in text output.</summary>
  public virtual string ToScoreString() => $"**{Score}**";

  /// <summary>A Markdown string representation of the roll and any user-provided text, for use in text output.</summary>
  public override string ToString()
  {
    var baseString = $"{ToScoreString()} vs. {ToChallengeString()}";
    if (Text.Length > 0)
    {
      return $"{Text}\n{baseString}";
    }
    return baseString;
  }

  /// <summary>The outcome of the roll - a Strong Hit, Weak Hit, or Miss.</summary>
  public IronswornRollOutcome Outcome
  {
    get
    {
      if (Score > ChallengeDie1.Value && Score > ChallengeDie2.Value)
      {
        return IronswornRollOutcome.StrongHit;
      }

      if (Score <= ChallengeDie1.Value && Score <= ChallengeDie2.Value)
      {
        return IronswornRollOutcome.Miss;
      }

      return IronswornRollOutcome.WeakHit;
    }
  }

  /// <summary>The string representation of the roll outcome - a Strong Hit, Weak Hit, or Miss - and whether or not it's a Match.</summary>
  public string OutcomeText()
  {
    if (IsMatch)
    {
      return Outcome switch
      {
        IronswornRollOutcome.Miss => IronswornRollResources.MissWithAMatch,
        IronswornRollOutcome.WeakHit => IronswornRollResources.WeakHitWithAMatch,
        IronswornRollOutcome.StrongHit => IronswornRollResources.StrongHitWithAMatch,
        _ => "ERROR",
      };
    }

    return Outcome switch
    {
      IronswornRollOutcome.Miss => IronswornRollResources.Miss,
      IronswornRollOutcome.WeakHit => IronswornRollResources.Weak_Hit,
      IronswornRollOutcome.StrongHit => IronswornRollResources.Strong_Hit,
      _ => "ERROR",
    };
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
    return new EmbedBuilder()
        .WithColor(OutcomeColor())
        .WithThumbnailUrl(OutcomeIcon())
        .WithTitle(OutcomeText())
        .WithDescription(ToString())
        .WithFooter(FooterText())
        .WithAuthor(RollTypeLabel);
  }

  public string OverMaxMessage() => RawScore > 10 ? string.Format(IronswornRollResources.OverMaxMessage, Score) : "";

  public virtual string FooterText() => OverMaxMessage();

  // <summary>A string description of the roll type (e.g. "Progress Roll", "Action Roll"), for use in labelling output.</summary>
  public virtual string RollTypeLabel { get => "Roll"; }
}