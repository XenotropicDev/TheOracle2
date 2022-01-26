namespace TheOracle2.GameObjects;

public class ChallengeDice : Dice, IMatchable
{
    private const int Sides = 10;
    private const int NumberOfDice = 2;
    public ChallengeDice(Random random) : base(random, 2, Sides) { }
    public ChallengeDice(Random random, int? die1 = null, int? die2 = null)
    {
        Add(new Die(random, Sides, die1 ?? null));
        Add(new Die(random, Sides, die2 ?? null));
    }
    public ChallengeDice(IEnumerable<Die> dice) : base(dice)
    {
        if (dice.Count() > NumberOfDice)
        {
            throw new ArgumentOutOfRangeException(nameof(dice), "No more than 2 challenge dice allowed.");
        }
    }
    public ChallengeDice(params Die[] dice) : base(dice)
    {
        if (dice.Length > NumberOfDice)
        {
            throw new ArgumentOutOfRangeException(nameof(dice), "No more than 2 challenge dice allowed.");
        }
    }
    public ChallengeDice(Random random, IEnumerable<int> values)
    {
        if (values.Count() > NumberOfDice)
        {
            throw new ArgumentOutOfRangeException(nameof(values), "No more than 2 challenge dice values allowed.");
        }
        AddRange(values.Select(item => new Die(random, Sides, item)));
    }
    public ChallengeDice(Random random, Embed embed)
    {
        var values = ParseChallengeDice(embed);
        AddRange(values.Select(item => new Die(random, Sides, item)));
    }
    /// <summary>Whether the Challenge Dice values match.</summary>
    public bool IsMatch => this.All(die => die.Value == this[0].Value);
    public static List<int> ParseChallengeDice(string challengeString)
    {
        var dieValueStrings = challengeString.Split(DieSeparator).ToList();
        var dieValues = new List<int>();
        dieValueStrings.ForEach(item =>
        {
            if (!int.TryParse(item, out int dieValue)) { throw new Exception($"Unable to parse value from challenge dice string: {challengeString}"); }
            dieValues.Add(dieValue);
        });
        return dieValues;
    }
    public static List<int> ParseChallengeDice(EmbedField challengeField)
    {
        var diceString = challengeField.Value;
        return ParseChallengeDice(diceString);
    }
    public static List<int> ParseChallengeDice(Embed rollEmbed)
    {
        var fields = rollEmbed.Fields.ToList();

        // var challengeField = fields.First(field => field.ToString().Equals(Label, StringComparison.Ordinal));
        // fields.ForEach(item => Console.WriteLine($"{item.Name}: {item.Value}"));
        var challengeField = fields.Find(field => !field.Name.Contains("Score") || field.ToString().Equals(Label, StringComparison.Ordinal));
        return ParseChallengeDice(challengeField);
    }
    private const string Label = "Challenge Dice";
    public EmbedFieldBuilder ToEmbedField()
    {
        return new EmbedFieldBuilder().WithName(Label).WithValue(ToString());
    }
}
