namespace TheOracle2.GameObjects;
using System.ComponentModel.DataAnnotations;

public class OracleAnswer : Die
{
  public OracleAnswer(int odds, string question) : base(100)
  {
    if (odds < 1) { throw new ArgumentOutOfRangeException(nameof(odds), "Chance must be from 1 to 99, inclusive."); }
    if (odds > 99) { throw new ArgumentOutOfRangeException(nameof(odds), "Chance must be from 1 to 99, inclusive."); }
    Question = question;
    Odds = odds;
  }
  public string Question { get; set; }
  public int Odds { get; }
  public bool IsYes { get => Value <= Odds; }

  public bool IsMatch { get => MatchResults.Contains(Value); }
  private readonly static List<int> MatchResults = new() { 11, 22, 33, 44, 55, 66, 77, 88, 99, 100 };

  public override string ToString()
  {
    return $"Rolled {Value} vs. {Odds}: **{AnswerString}**";
  }

  public Color OutcomeColor() => IsYes switch
  {
    false => new Color(0xC50933),
    true => new Color(0x47AEDD)
  };
  // TODO: put this in a resource file instead
  private string MatchMessage { get => "You rolled a match! Envision an extreme result or twist."; }

  private string AnswerString
  {
    get
    {
      var str = IsYes ? "Yes" : "No";
      if (IsMatch)
      {
        str = $"hell {str}!".ToUpperInvariant();
      }
      return str;
    }
  }

  public static Dictionary<int, string> OddsString
  {
    get => new Dictionary<int, string>(){
      {10, "Small chance"},
      {25, "Unlikely"},
      {50, "50/50"},
      {75, "Likely"},
      {90, "Sure thing"}
    };
  }

  // public static string OddsString(int odds)
  // {
  //   switch (AskOption.IsDefined(typeof(AskOption), odds))
  //   {
  //     case true:
  //       // return $"{new CultureInfo("en-US", false).TextInfo.ToTitleCase(Defs.ByNumber[odds])}";
  //       return GetAttributeOfType<DisplayAttribute>((AskOption)odds).Name;

  //     case false:
  //       return odds + "%";
  //   }
  // }

  public EmbedBuilder ToEmbed()
  {
    string authorString = $"Ask the Oracle: {OracleAnswer.OddsString[Odds]}";
    string footerString = IsMatch ? MatchMessage : "";
    return new EmbedBuilder()
    .WithAuthor(authorString)
    .WithDescription(ToString())
    .WithTitle(Question.Length > 0 ? Question : "Ask the Oracle")
    .WithFooter(footerString)
    .WithColor(OutcomeColor())
    ;
  }
  public static T GetAttributeOfType<T>(Enum enumVal) where T : Attribute
  {
    var type = enumVal.GetType();
    var memInfo = type.GetMember(enumVal.ToString());
    var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
    return (attributes.Length > 0) ? (T)attributes[0] : null;
  }
}