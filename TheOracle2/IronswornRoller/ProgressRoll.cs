namespace TheOracle2;

public class ProgressRoll : IronswornRoll
{
  /// <summary>
  /// Makes a Progress Roll.
  /// </summary>
  /// <param name="progressScore">The player character's progress score.</param>
  /// <param name="description">A user-provided text string.</param>
  /// <param name="challengeDie1">A preset value for the first challenge die.</param>
  /// <param name="challengeDie2">A preset value for the second challenge die.</param>
  public ProgressRoll(Random random, int progressScore, string description = "", int? challengeDie1 = null, int? challengeDie2 = null, string moveName = "") : base(random, description, challengeDie1, challengeDie2, moveName)
  {
    ProgressScore = progressScore;
  }

  /// <inheritdoc/>
  public override string EmbedCategory { get => "Progress Roll"; }

  public override string ScoreLabel { get => "Progress Score"; }

  public int ProgressScore { get; set; }

  /// <inheritdoc/>
  public override int RawScore { get => ProgressScore; }
}