using Discord.Interactions;
using System.ComponentModel.DataAnnotations;

namespace TheOracle2;

[Group("ask", "Ask the oracle")]
public class OracleAskPaths : InteractionModuleBase
{
  private readonly Random random;

  public OracleAskPaths(Random random)
  {
    this.random = random;
  }

  [SlashCommand("with-likelihood", "Ask the oracle based on the predefined likelihoods")]
  public async Task Named(
      [Summary(description: "Ask the oracle based on the predefined likelihoods")]
        [Choice("Sure Thing", "SureThing"),
        Choice("Likely", "Likely"),
        Choice("Fifty-fifty", "FiftyFifty"),
        Choice("Unlikely", "Unlikely"),
        Choice("Small Chance", "SmallChance")]
        string keyword,
      string fluff = "")
  {
    if (!Enum.TryParse<AskOption>(keyword, out var value)) throw new ArgumentException($"Unknown value {keyword}");
    var roll = random.Next(101);
    string result = (roll <= (int)value) ? "Yes" : "No";

    var displayValue = GetAttributeOfType<DisplayAttribute>(value)?.Name ?? keyword;
    if (fluff?.Length > 0) fluff = $"You asked: {fluff}\n";
    await RespondAsync($"{fluff}You rolled: {roll} VS. {displayValue} ({(int)value} or less)\n**{result}**.").ConfigureAwait(false);
  }

  [SlashCommand("with-chance", "Ask the oracle based on a percentage")]
  public async Task Numeric([Summary(description: "Ask the oracle based on a given percent chance of something happening")]
                                [MaxValue(99)]
                                [MinValue(1)] int number,
      string fluff = null)
  {
    var roll = random.Next(101);
    string result = (roll <= number) ? "Yes" : "No";

    if (fluff?.Length > 0) fluff += "\n";
    await RespondAsync($"{fluff}You rolled {roll} VS. {number} or less\n**{result}**.").ConfigureAwait(false);
  }

  public static T GetAttributeOfType<T>(Enum enumVal) where T : Attribute
  {
    var type = enumVal.GetType();
    var memInfo = type.GetMember(enumVal.ToString());
    var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
    return (attributes.Length > 0) ? (T)attributes[0] : null;
  }
}

