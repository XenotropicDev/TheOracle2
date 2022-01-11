using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.GameObjects;
namespace TheOracle2;

public class ProgressTrackerCommand : InteractionModuleBase
{
  private readonly Random Random;
  public ProgressTrackerCommand(Random random)
  {
    Random = random;
  }
  [SlashCommand("track", "Creates a generic tracker for things like vows, expeditions, and combat")]
  public async Task BuildProgressTrack(
    [Summary(description: "A title for the progress track.")]
    string title,
    [Summary(description: "The challenge rank of the progress track.")]
    ChallengeRank rank,
    [Summary(description: "An optional description.")]
    string description=""
  )
  {
    GenericTrack track = new(rank: rank, ticks: 0, title: title, description: description);
    await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
  }
}

