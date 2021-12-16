using Discord.Interactions;

namespace TheOracle2;

[Group("ask", "Ask the oracle")]
public class OracleAskPaths : InteractionModuleBase
{
    [SlashCommand("with-likelihood", "Ask the oracle based on the predefined likelihoods")]
    public async Task Named([Summary(description: "Ask the oracle based on the predefined likelihoods")] AskOption keyword, string fluff = "")
    {
        var rnd = BotRandom.Instance;
        var roll = rnd.Next(101);
        string result = (roll >= 100 - (int)keyword) ? "Yes" : "No";

        if (fluff?.Length > 0) fluff += "\n";
        await RespondAsync($"{fluff}You rolled {roll} VS. {keyword} ({(int)keyword}%)\n**{result}**.").ConfigureAwait(false);
    }

    [SlashCommand("with-chance", "Ask the oracle based on a percentage")]
    public async Task Numeric([Summary(description: "Ask the oracle based on a given percent chance of something happening")]
                                [MaxValue(99)]
                                [MinValue(1)] int number, 
        string fluff = null)
    {
        var rnd = BotRandom.Instance;
        var roll = rnd.Next(101);
        string result = (roll >= 100 - number) ? "Yes" : "No";

        if (fluff?.Length > 0) fluff += "\n";
        await RespondAsync($"{fluff}You rolled {roll} VS. {number}%\n**{result}**.").ConfigureAwait(false);
    }
}

public enum AskOption
{
    AlmostCertain = 10,
    Likely = 25,
    FiftyFifty = 50,
    Unlikely = 75,
    SmallChance = 90
}