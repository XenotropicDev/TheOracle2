using TheOracle2.GameObjects;
using TheOracle2.IronswornRoller;
using System.Text.RegularExpressions;

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
  public ActionRoll(Random random, int stat, int adds, int? momentum = null, string description = "", int? actionDie = null, int? challengeDie1 = null, int? challengeDie2 = null, string moveName = "") : base(random, description, challengeDie1, challengeDie2, moveName)
  {
    ActionDie = new Die(random, 6, actionDie);
    Stat = stat;
    Adds = adds;
    Momentum = momentum ?? 0;
  }
  public ActionRoll(Random random, Embed embed, int? momentum = null) : base(random, embed)
  {
    if (embed.Author.ToString() != null || !embed.Author.ToString().StartsWith("Action Roll"))
    {
      throw new Exception("Embed is not an Action Roll.");
    }
    EmbedCategory = embed.Author.ToString();
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
    var momentumResultString = MomentumBurnOutcome == IronswornRollOutcome.WeakHit ? IronswornRollResources.Weak_Hit : MomentumBurnOutcome == IronswornRollOutcome.StrongHit ? IronswornRollResources.Strong_Hit : "ERROR";
    if (IsBurnable && !IsBurnt)
    {
      return $"You may burn +{Momentum} momentum for a {momentumResultString} (see p. 32).";
    }
    if (IsBurnt)
    {
      return $"You burned momentum to improve this roll to a {momentumResultString}";
    }
    return "";
  }
  /// <inheritdoc/>
  public override string Footer => $"{base.Footer}\n{MomentumText()}";
  /// <inheritdoc/>
  public override string EmbedCategory { get; set; } = "Action Roll";
  private string ActionDieString => IsActionDieCanceled ? $"~~{ActionDie}~~" : $"{ActionDie}";
  /// <inheritdoc/>
  public override string ToScoreString()
  {
    string arithmetic = $"{ActionDieString} + {Stat} + {Adds}";
    if (IsBurnt)
    {
      return $"~~{arithmetic}~~ {Momentum} = {base.ToScoreString()}";
    }
    return $"{arithmetic} = {base.ToScoreString()}";
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
  private const string ActionScorePattern = @"^(?:~~)([1-6])(?:~~) \+ ([1-9]|10) \+ ([1-9]|10) = \*\*([1-9]|10)\*\*$";
  private const string BurntActionScorePattern = @"^~~([1-6]) \+ ([1-9]|10) \+ ([1-9]|10) = ([1-9]|10)~~ \*\*([1-9]|10)\*\*$";
  public static int[] ParseActionScore(string actionScoreString)
  {
    if (Regex.IsMatch(actionScoreString, BurntActionScorePattern))
    {
      return Regex.Matches(actionScoreString, BurntActionScorePattern).Select(match => int.Parse(match.ToString())) as int[];
    }
    if (Regex.IsMatch(actionScoreString, ActionScorePattern))
    {
      return Regex.Matches(actionScoreString, ActionScorePattern).Select(match => int.Parse(match.ToString())) as int[];
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
  public static readonly Dictionary<string, IEmote> ActionRollEmoji = new() { { "burn", new Emoji("🔥") } };
}