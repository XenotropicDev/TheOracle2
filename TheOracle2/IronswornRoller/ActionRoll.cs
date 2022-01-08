using TheOracle2.GameObjects;
using TheOracle2.IronswornRoller;

namespace TheOracle2;

public class ActionRoll : IronswornRoll
{
  /// <summary>
  /// Makes an Action Roll.
  /// </summary>
  /// <param name="stat">The stat value for the action roll.</param>
  /// <param name="adds">Any adds for the action roll.</param>
  /// <param name="momentum">The current Momentum of the player character.</param>
  /// <param name="text">A user-provided text string.</param>
  /// <param name="actionDieValue">A preset value for the action die.</param>
  /// <param name="challengeDieValue1">A preset value for the first challenge die.</param>
  /// <param name="challengeDieValue2">A preset value for the second challenge die.</param>
  public ActionRoll(Random random, int stat, int adds, int? momentum = null, string text = "", int? actionDieValue = null, int? challengeDieValue1 = null, int? challengeDieValue2 = null) : base(random, text, challengeDieValue1, challengeDieValue2)
  {
    Stat = stat;
    Adds = adds;
    Momentum = momentum ?? 0;
    ActionDie = new Die(Random, 6, actionDieValue);
  }

  public int Stat { get; set; }
  public int Adds { get; set; }
  public Die ActionDie { get; set; }
  public int Momentum { get; set; }

  public bool IsActionDieCanceled
  {
    get
    {
      return Momentum < 0 && ActionDie.Value == Math.Abs(Momentum);
    }
  }

  public bool IsBurnable { get => Momentum > Score && Outcome != IronswornRollOutcome.StrongHit && Outcome != MomentumBurnOutcome; }

  public IronswornRollOutcome MomentumBurnOutcome
  {
    get
    {
      if (Momentum > ChallengeDie1.Value && Momentum > ChallengeDie2.Value)
      {
        return IronswornRollOutcome.StrongHit;
      }
      if (Momentum <= ChallengeDie1.Value && Momentum <= ChallengeDie2.Value)
      {
        return IronswornRollOutcome.Miss;
      }
      return IronswornRollOutcome.WeakHit;
    }
  }

  public string MomentumText()
  {
    if (IsActionDieCanceled)
    {
      return "Your action die was canceled by your negative momentum (see p. 34).";
    }
    if (IsBurnable)
    {
      var momentumResultString = MomentumBurnOutcome == IronswornRollOutcome.WeakHit ? IronswornRollResources.Weak_Hit : MomentumBurnOutcome == IronswornRollOutcome.StrongHit ? IronswornRollResources.Strong_Hit : "ERROR";
      return $"You may burn +{Momentum} momentum for a {momentumResultString} (see p. 32).";
    }
    return "";
  }

  /// <inheritdoc/>
  public override string FooterText()
  {
    return $"{base.FooterText()}\n{MomentumText()}";
  }

  /// <inheritdoc/>
  public override string RollTypeLabel { get => "Action Roll"; }

  /// <inheritdoc/>
  public override string ToScoreString()
  {
    string scoreString = (Stat + Adds > 0) ? $"{ActionDie.Value} + {Stat} + {Adds} = {base.ToScoreString()}" : base.ToScoreString();

    return IsActionDieCanceled
        ? $"~~{ActionDie.Value}~~ + {Stat} + {Adds} = {base.ToScoreString()}"
        : scoreString;
  }

  /// <inheritdoc/>
  public override int RawScore
  {
    get
    {
      return IsActionDieCanceled
          ? Stat + Adds
          : Stat + Adds + ActionDie.Value;
    }
  }
}