using TheOracle2.IronswornRoller;
namespace TheOracle2.GameObjects;

/// <summary>
/// Represents a group of dice.
/// </summary>
public class Dice : List<Die>
{
  /// <summary>
  /// Roll multiple dice with the same number of sides.
  /// </summary>
  /// <param name="random"></param>
  /// <param name="number">The number of dice to roll.</param>
  /// <param name="sides">The number of sides on each die.</param>
  ///
  public Dice(Random random, int number, int sides)
  {
    for (int i = 0; i < number; i++)
    {
      Add(new Die(random, sides));
    }
  }
  /// <summary>
  /// Create a group of dice with the same number of size using preset values.
  /// </summary>
  /// <param name="random"></param>
  /// <param name="sides">The number of sides on each die.</param>
  /// <param name="values">An enumerable of values to set the dice to.</param>
  ///
  public Dice(Random random, int sides, IEnumerable<int> values)
  {
    this.AddRange(
      values.Select(value => new Die(random, sides, value)));
  }
  /// <summary>
  /// Group multiple Die objects into a Dice list.
  /// </summary>
  /// <param name="dice">An enumerable of Die objects.</param>
  public Dice(IEnumerable<Die> dice) : base(dice) { }
  /// <summary>
  /// Group multiple Die objects into a Dice list.
  /// </summary>
  /// <param name="dice">At least one Die object.</param>
  public Dice(params Die[] dice) : base(dice) { }

  /// <summary>The default separator to use between dice when rendering them as a string.</summary>
  public const string DieSeparator = ", ";

  /// <summary>A string representation of the dice values for use in text output.</summary>
  public override string ToString() { return ToString(DieSeparator); }
  public string ToString(string joiner)
  {
    return string.Join(joiner, this);
  }
  /// <summary>
  /// Render the dice as an embed field for display.
  /// </summary>
  public EmbedFieldBuilder ToEmbedField(string fieldTitle = "Dice")
  {
    return new EmbedFieldBuilder()
      .WithName(fieldTitle)
      .WithValue(ToString());
  }
  /// <summary>
  /// Gets the highest value of the two challenge dice.
  /// </summary>
  public int Highest()
  {
    return this.Max(die => die.Value);
  }
  /// <summary>
  /// Gets the lowest value of the two challenge dice.
  /// </summary>
  public int Lowest()
  {
    return this.Min(die => die.Value);
  }
  /// <summary>
  /// Returns the number of dice showing a value lower than the number.
  /// </summary>
  public int ValuesLessThan(int number)
  {
    return this.Count(die => die.Value < number);
  }
  /// <summary>
  /// Returns the number of dice showing a value greater than the number.
  /// </summary>
  public int ValuesGreaterThan(int number)
  {
    return this.Count(die => die.Value > number);
  }
}