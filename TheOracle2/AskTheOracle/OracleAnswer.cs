namespace TheOracle2.GameObjects;

public class OracleAnswer : Die
{
  public OracleAnswer(Random random, int odds, string question) : base(random, 100)
  {
    if (odds is < 1 or > 99) { throw new ArgumentOutOfRangeException(nameof(odds), "Chance must be from 1 to 99, inclusive."); }
    Question = question;
    Odds = odds;
  }
  public string Question { get; set; }
  public int Odds { get; }
  public bool IsYes => Value <= Odds;
  public bool IsMatch => MatchResults.Contains(Value);
  private static readonly List<int> MatchResults = new() { 11, 22, 33, 44, 55, 66, 77, 88, 99, 100 };

  public override string ToString()
  {
    return $"Rolled {Value} vs. {Odds}: **{AnswerString}**";
  }
  public Color OutcomeColor()
  {
    return IsYes switch
    {
      true => new Color(0x47AEDD),
      false => new Color(0xC50933)
    };
  }
  private static string MatchMessage => "You rolled a match! Envision an extreme result or twist.";
  private string AnswerString
  {
    get
    {
      string str = IsYes ? "Yes" : "No";
      if (IsMatch)
      {
        str = $"hell {str}!".ToUpperInvariant();
      }
      return str;
    }
  }
  public static Dictionary<int, string> OddsString => new()
  {
    { 10, "Small chance" },
    { 25, "Unlikely" },
    { 50, "50/50" },
    { 75, "Likely" },
    { 90, "Sure thing" }
  };

  public EmbedBuilder ToEmbed()
  {
    string authorString = $"Ask the Oracle: {OddsString[Odds]}";
    string footerString = IsMatch ? MatchMessage : "";
    return new EmbedBuilder()
    .WithAuthor(authorString)
    .WithDescription(ToString())
    .WithTitle(Question.Length > 0 ? Question : "Ask the Oracle")
    .WithFooter(footerString)
    .WithColor(OutcomeColor())
    ;
  }
}