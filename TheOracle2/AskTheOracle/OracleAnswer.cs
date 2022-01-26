namespace TheOracle2.GameObjects;

public class OracleAnswer : Die, IMatchable
{
    public OracleAnswer(Random random, AskOption odds, string question) : base(random, 100)
    {
        Question = question;
        Odds = odds;
    }

    public string Question { get; set; }
    public AskOption Odds { get; }
    public bool IsYes => Value <= (int)Odds;
    public bool IsMatch => Value == 100 || Value % 11 == 0;

    public override string ToString()
    {
        return $"Rolled {Value} vs. {(int)Odds}: **{AnswerString}**";
    }

    public Color OutcomeColor() => IsYes ? new Color(0x47AEDD) : new Color(0xC50933);

    private string MatchMessage => (IsMatch) ? "You rolled a match! Envision an extreme result or twist." : String.Empty;

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

    public static Dictionary<AskOption, string> OddsString => new()
    {
        { AskOption.SmallChance, "Small chance" },
        { AskOption.Unlikely, "Unlikely" },
        { AskOption.FiftyFifty, "50/50" },
        { AskOption.Likely, "Likely" },
        { AskOption.SureThing, "Sure thing" }
    };

    public EmbedBuilder ToEmbed()
    {
        string authorString = $"Ask the Oracle: {OddsString[Odds]}";
        string footerString = MatchMessage;
        return new EmbedBuilder()
        .WithAuthor(authorString)
        .WithDescription(ToString())
        .WithTitle(Question.Length > 0 ? Question : "Ask the Oracle")
        .WithFooter(footerString)
        .WithColor(OutcomeColor())
        ;
    }
}
