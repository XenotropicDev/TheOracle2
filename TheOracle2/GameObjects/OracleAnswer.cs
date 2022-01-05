
namespace TheOracle2.GameObjects;

public class OracleAnswer : Die
{
  public OracleAnswer(AskOption askOption, string question) : base(100)
  {
    Ask = askOption;
    Question = question;
  }
  public OracleAnswer(int chance, string question) : base(100)
  {
    if (chance < 1 || chance > 99) { throw new ArgumentOutOfRangeException(nameof(chance), "Chance must be from 1 to 99, inclusive."); }
    RawChance = chance;
    Question = question;
  }
  public string Question { get; }

  public int Chance
  {
    get => Ask != null ? (int)Ask : RawChance;
  }
  private readonly int RawChance;
  public AskOption? Ask { get; }
  public bool IsYes { get => Value <= Chance; }

  public bool IsMatch { get => MatchResults.Contains(Value); }
  private readonly static List<int> MatchResults = new() { 11, 22, 33, 44, 55, 66, 77, 88, 99, 100 };

  public override string ToString()
  {
    return $"Rolled {Value} vs. {Chance}: **{(IsYes ? "Yes" : "No")}**";
  }

  public Color OutcomeColor() => IsYes switch
  {
    false => new Color(0x47AEDD),
    true => new Color(0xC50933)
  };
  private string MatchMessage { get => "On a match, envision an extreme result or twist."; }
  public EmbedBuilder ToEmbed()
  {
    string chanceString = (Ask != null) ? $"{Ask.GetType().GetField(Ask.ToString())}" : Chance + "%";
    string authorString = $"Ask the Oracle: {chanceString}";
    string footerString = IsMatch ? MatchMessage : "";
    return new EmbedBuilder()
    .WithAuthor(authorString)
    .WithDescription(ToString())
    .WithTitle(Question)
    .WithFooter(footerString)
    .WithColor(OutcomeColor())
    ;
  }
}