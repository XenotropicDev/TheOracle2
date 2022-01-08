using Discord.Interactions;
using TheOracle2.GameObjects;
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
        [Choice("Sure thing", 90),
        Choice("Likely", 75),
        Choice("Fifty-fifty", 50),
        Choice("Unlikely", 25),
        Choice("Small chance", 10)]
        int chance,
      string question = "")
  {
    await RespondAsync(embed: new OracleAnswer(random, chance, question).ToEmbed().Build()).ConfigureAwait(false);
  }

  [SlashCommand("with-chance", "Ask the oracle based on a percentage")]
  public async Task Numeric([Summary(description: "Ask the oracle based on a given percent chance of something happening")]
      [MaxValue(99)]
      [MinValue(1)] int chance,
      string question = "")
  {
    await RespondAsync(embed: new OracleAnswer(random, chance, question).ToEmbed().Build()).ConfigureAwait(false);
  }
}

