namespace TheOracle2.GameObjects;

/// <summary>
/// Represents a die with an arbitrary number of sides and its current facing.
/// </summary>
public class Die
{
  private readonly Random random;
  public int Sides { get; }
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
  public Die(int sides, int? value)
  {
    random = new System.Random();
    if (sides < 2) { throw new ArgumentOutOfRangeException(nameof(sides), "Die must have at least 2 sides."); }
    if (value != null && (value > sides || value < 1)) { throw new ArgumentOutOfRangeException(nameof(value), "Face must be null, or a positive integer less than the number of sides on the die."); }
    Sides = sides;
    Value = value ?? Roll();
  }
}