using Discord.Interactions;
using TheOracle2.GameObjects;

namespace TheOracle2;

public class AskTheOracleCommand : InteractionModuleBase
{
    private readonly Random random;

    public AskTheOracleCommand(Random random)
    {
        this.random = random;
    }

    [SlashCommand("ask", "Ask the Oracle a yes/no question (p. 225). To roll on a specific oracle table, use /oracle.")]
    public async Task AskTheOracle(
        [Summary(description: "The question to ask the oracle.")]
        string question,
        [Summary(description: "The odds of receiving a 'yes' answer.")]
        [Choice("Small chance (10 or less)", 10),
        Choice("Unlikely (25 or less)", 25),
        Choice("50/50 (50 or less)", 50),
        Choice("Likely (75 or less)", 75),
        Choice("Sure thing (90 or less)", 90)]
        int odds
    )
    {
        /// TODO: once discord display string attributes are available for enums, this can use the AskOption enum directly
        await RespondAsync(embed: new OracleAnswer(random, (AskOption)odds, question).ToEmbed().Build()).ConfigureAwait(false);
    }
}
