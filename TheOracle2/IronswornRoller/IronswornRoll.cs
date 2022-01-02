using TheOracle2.IronswornRoller;
using TheOracle2.GameObjects;

namespace TheOracle2;

public abstract class IronswornRoll {
  public string Text { get; }
  public Die ChallengeDie1 { get; set; }
  public Die ChallengeDie2 { get; set; }
  public Boolean IsMatch { get => ChallengeDie1.Value == ChallengeDie2.Value; }
  public abstract int RawScore { get; }
  public int Score { get => Math.Min(10, RawScore); }
  public string ToChallengeString() { return $"{ChallengeDie1.Value}, {ChallengeDie2.Value}"; }
  public virtual string ToScoreString() { return $"**{Score}**"; }

  public override string ToString() {
    var baseString = $"{ToScoreString()} vs. {ToChallengeString()}";
    if (Text.Length > 0) {
      return $"{Text}\n{baseString}";
    } else { return baseString; }
  }

  public IronswornRollOutcome Outcome {
    get {
      if (Score > ChallengeDie1.Value && Score > ChallengeDie2.Value) {
        return IronswornRollOutcome.StrongHit;
      }
      if (Score <= ChallengeDie1.Value && Score <= ChallengeDie2.Value) {
        return IronswornRollOutcome.Miss;
      }
      return IronswornRollOutcome.WeakHit;
    }
  }

  /// <summary>
  ///
  /// </summary>
  /// <param name="text">A user-provided text string.</param>
  /// <param name="challengeDieValue1">A preset value for the first challenge die.</param>
  /// <param name="challengeDieValue2">A preset value for the second challenge die.</param>
  public IronswornRoll(string text = "", int? challengeDieValue1 = null, int? challengeDieValue2 = null) {
    Text = text;
    ChallengeDie1 = new Die(10, challengeDieValue1 ?? null);
    ChallengeDie2 = new Die(10, challengeDieValue2 ?? null);
  }

  public string OutcomeText() {
    if (IsMatch) {
      return Outcome switch {
        IronswornRollOutcome.Miss => IronswornRollResources.MissWithAMatch,
        IronswornRollOutcome.WeakHit => IronswornRollResources.WeakHitWithAMatch,
        IronswornRollOutcome.StrongHit => IronswornRollResources.StrongHitWithAMatch,
        _ => "ERROR",
      };
    } else {
      return Outcome switch {
        IronswornRollOutcome.Miss => IronswornRollResources.Miss,
        IronswornRollOutcome.WeakHit => IronswornRollResources.Weak_Hit,
        IronswornRollOutcome.StrongHit => IronswornRollResources.Strong_Hit,
        _ => "ERROR",
      };
    }
  }

  public Color OutcomeColor() {
    return Outcome switch {
      IronswornRollOutcome.Miss => new Color(0xC50933),
      IronswornRollOutcome.WeakHit => new Color(0x842A8C),
      IronswornRollOutcome.StrongHit => new Color(0x47AEDD),
      _ => new Color(0x842A8C),
    };
  }

  public string OutcomeIcon() {
    return Outcome switch {
      IronswornRollOutcome.Miss => IronswornRollResources.MissImageURL,
      IronswornRollOutcome.WeakHit => IronswornRollResources.WeakHitImageURL,
      IronswornRollOutcome.StrongHit => IronswornRollResources.StrongHitImageURL,
      _ => IronswornRollResources.MissImageURL,
    };
  }

  public EmbedBuilder ToEmbed() {
    return new EmbedBuilder()
        .WithColor(OutcomeColor())
        .WithThumbnailUrl(OutcomeIcon())
        .WithTitle(OutcomeText())
        .WithDescription(ToString())
        .WithFooter(FooterText())
        .WithAuthor(AuthorText);
  }

  public string OverMaxMessage() {
    return RawScore > 10 ? string.Format(IronswornRollResources.OverMaxMessage, Score) : "";
  }
  public virtual string FooterText() {
    return OverMaxMessage();
  }
  public virtual string AuthorText {
    get => "Roll";
  }
}
