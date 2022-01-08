using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.GameObjects;
namespace TheOracle2;
public class ClockCommand : InteractionModuleBase
{

  [SlashCommand("clock", "Set a tension clock, campaign clock, or scene challenge (p. 230)")]
  public async Task BuildClock(
    [Summary(description: "The type of clock.")]
    [Choice("Tension clock", "tension-clock"),
    Choice("Campaign clock", "campaign-clock"),
    Choice("Scene challenge", "scene-challenge")
    ] string type,
    [Summary(description: "The number of segments for the clock")]
    [Choice("4 segments", 4),
    Choice("6 segments", 6),
    Choice("8 segments", 8),
    Choice("10 segments", 10)]
    int segments,
    [Summary(description: "A label for the clock.")]
    string text
  )
  {
    switch (type)
    {
      case "tension-clock":
        TensionClock tensionClock = new((ClockSize)segments, 0, text);
        await RespondAsync(
          embed: tensionClock.ToEmbed().Build(),
          components: tensionClock.MakeComponents().Build());
        break;
      case "campaign-clock":
        CampaignClock campaignClock = new((ClockSize)segments, 0, text);
        await RespondAsync(
          embed: campaignClock.ToEmbed().Build(),
          components: campaignClock.MakeComponents().Build()
          );
        break;
      case "scene-challenge":
        await RespondAsync("NYI");
        break;
      default:
        break;
    }
  }

  [ComponentInteraction("reset-clock")]
  public async Task ResetClock()
  {
    SocketMessageComponent interaction = Context.Interaction as SocketMessageComponent;
    IClock clock = IClock.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    await interaction.UpdateAsync(msg =>
  {
    clock.Filled = 0;
    msg.Components = clock.MakeComponents().Build();
    msg.Embed = clock.ToEmbed().Build();
  });
  }
  [ComponentInteraction("advance-clock")]
  public async Task AdvanceClock()
  {
    SocketMessageComponent interaction = Context.Interaction as SocketMessageComponent;
    IClock clock = IClock.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    await interaction.UpdateAsync(msg =>
    {
      clock.Filled++;
      msg.Components = clock.MakeComponents().Build();
      msg.Embed = clock.ToEmbed().Build();
    });
    await interaction.FollowupAsync(embed: clock.AlertEmbed().Build());
  }

  [ComponentInteraction("advance-clock-menu")]
  public async Task AdvanceClockMenu(string[] values)
  {
    SocketMessageComponent interaction = Context.Interaction as SocketMessageComponent;
    IClock clock = IClock.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    string optionValue = values.FirstOrDefault();
    if (int.TryParse(optionValue.Replace("advance-", ""), out int odds))
    {
      OracleAnswer answer = new(odds, $"Does the clock *{clock.Text}* advance?");
      EmbedBuilder answerEmbed = answer.ToEmbed();
      if (answer.IsYes)
      {
        if (answer.IsMatch)
        {
          answerEmbed.WithFooter("You rolled a match! Two segments have been marked on the clock. Envision how this situation or project gains dramatic support or inertia.");
        }
        await interaction.UpdateAsync(msg =>
        {
          clock.Filled += answer.IsMatch ? 2 : 1;
          msg.Embed = clock.ToEmbed().Build();
          msg.Components = clock.MakeComponents().Build();
        });
        answerEmbed = answerEmbed.WithThumbnailUrl(IClock.Images[clock.Segments].ElementAt(clock.Filled));
        await interaction.FollowupAsync(embed: answerEmbed.Build());
        return;
      }
      if (answer.IsMatch)
      {
        answerEmbed.WithFooter("You rolled a match! Envision a surprising turn of events which pits new factors or forces against the clock.");
      }
      answerEmbed = answerEmbed.WithThumbnailUrl(IClock.Images[clock.Segments].ElementAt(clock.Filled));
      await interaction.RespondAsync(
        embed: answerEmbed.Build());
      return;
    }
    switch (optionValue)
    {
      case "reset":
        clock.Filled = 0;
        break;
      case "advance":
        clock.Filled++;
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(optionValue), "Could not parse integer or valid string from select menu option value.");
    }
    await interaction.UpdateAsync(msg =>
    {
      msg.Components = clock.MakeComponents().Build();
      msg.Embed = clock.ToEmbed().Build();
    });
    await interaction.FollowupAsync(embed: clock.AlertEmbed().Build());
  }
}

