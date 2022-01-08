using Discord.Interactions;

namespace TheOracle2;

public class ProgressRollCommand : InteractionModuleBase
{
  public ProgressRollCommand(Random random)
  {
    Random = random;
  }

  public Random Random { get; }

  [SlashCommand("progress-roll", "Make an Ironsworn progress roll.")]
  public async Task ProgressRoll(
    [Summary(description: "The progress score.")] int progressScore,
    [Summary(description: "A preset value for the first Challenge Die to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie1 = null,
    [Summary(description: "A preset value for the second Challenge Die to use instead of rolling")][MinValue(1)][MaxValue(10)] int? challengeDie2 = null,
    [Summary(description: "Notes, fiction, or other text to include with the roll.")] string text = "")
  {
    var roll = new ProgressRoll(Random, progressScore, text, challengeDie1, challengeDie2);
    await RespondAsync(embed: roll.ToEmbed().Build()).ConfigureAwait(false);
  }
}