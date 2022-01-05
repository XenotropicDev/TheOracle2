using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using TheOracle2.GameObjects;
namespace TheOracle2;
public class ClockCommand : InteractionModuleBase
{
  [SlashCommand("clock", "Creates a clock.")]
  public async Task BuildClock(
    [Summary(description: "A label for the clock.")]
    string text,
    [Summary(description: "The number of segments for the clock")]
    [Choice("4 segments", 4),
    Choice("6 segments", 6),
    Choice("8 segments", 8),
    Choice("10 segments", 10)]
    int segments = 6
  )
  {
    Clock clock = new((ClockSize)segments, 0, text);
    ComponentBuilder components = new ComponentBuilder().WithButton(clock.AdvanceButton());
    await RespondAsync(embed: clock.ToEmbed().Build(), components: components.Build());
  }
  [ComponentInteraction("advance-clock")]
  public async Task AdvanceClock()
  {
    var interaction = Context.Interaction as SocketMessageComponent;
    var oldEmbed = interaction.Message.Embeds.FirstOrDefault();
    Clock clock = new(oldEmbed);
    clock.Advance();
    ComponentBuilder components = new ComponentBuilder().WithButton(clock.AdvanceButton());
    await interaction.UpdateAsync(msg =>
    {
      msg.Components = components.Build();
      msg.Embed = clock.ToEmbed().Build();
    });
  }
}

