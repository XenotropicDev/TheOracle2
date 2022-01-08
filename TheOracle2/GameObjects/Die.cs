namespace TheOracle2.GameObjects;

public class Die
{
  private readonly Random random;
  public readonly int Sides;
  public int Value { get; set; }

  private int Roll()
  {
    return random.Next(1, Sides + 1);
  }

  public void Reroll()
  {
    Value = Roll();
  }

  /// <summary>
  ///
  /// </summary>
  /// <param name="sides">The number of sides the die has (minimum 2)</param>
  /// <param name="value">A preset value for the die</param>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public Die(Random random, int sides, int? value)
  {
    if (sides < 2) { throw new ArgumentOutOfRangeException("Die must have at least 2 sides."); }
    if (value != null && (value > sides || value < 1)) { throw new ArgumentOutOfRangeException("Face must be null, or a positive integer less than the number of sides on the die."); }

    this.random = random;
    Sides = sides;
    Value = value ?? Roll();
  }
}