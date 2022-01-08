using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.GameObjects;
namespace TheOracle2;

[Group("clock", "Set a campaign clock, tension clock, or scene challenge (p. 230)")]
public class ClockCommand : InteractionModuleBase
{
  [SlashCommand("campaign", "Set a campaign clock to resolve objectives and actions in the background of your campaign.")]
  public async Task BuildCampaignClock(
    [Summary(description: "A name that makes it clear what project is complete or event triggered when the clock is filled.")]
    string text,
    [Summary(description: "The number of clock segments.")]
    [Choice("4 segments", 4),
    Choice("6 segments", 6),
    Choice("8 segments", 8),
    Choice("10 segments", 10)]
    int segments
  )
  {
    CampaignClock campaignClock = new((ClockSize)segments, 0, text);
    await RespondAsync(
      embed: campaignClock.ToEmbed().Build(),
      components: campaignClock.MakeComponents().Build()
      );
  }



  [SlashCommand("tension", "Set a tension clock: a smaller-scope clock to fill as you suffer setbacks or fail to act.")]
  public async Task BuildTensionClock(
    [Summary(description: "A name for the tension clock.")]
    string text,
    [Summary(description: "The number of clock segments. Imminent danger or deadline: 4-6. Longer term threat: 8-10.")]
    [Choice("4 segments", 4),
    Choice("6 segments", 6),
    Choice("8 segments", 8),
    Choice("10 segments", 10)]
    int segments
      )
  {
    TensionClock tensionClock = new((ClockSize)segments, 0, text);
    await RespondAsync(
      embed: tensionClock.ToEmbed().Build(),
      components: tensionClock.MakeComponents().Build());
  }

  // [SlashCommand("scene-challenge", "Set a clock for a scene challenge")]
  // public async Task BuildSceneChallenge(
  //   [Summary(description: "The scene challenge's objective.")]
  //   string text,
  //   [Summary(description: "The number of clock segments. Default = 6, severe disadvantage = 4, strong advantage = 8.")]
  //   [Choice("4 segments", 4),
  //   Choice("6 segments", 6),
  //   Choice("8 segments", 8)]
  //   int segments=6)
  // {
  //   await RespondAsync("NYI");
  // }
}

public class ClockComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
  [ComponentInteraction("clock-reset")]
  public async Task ResetClock()
  {
    var interaction = Context.Interaction as SocketMessageComponent;
    var clock = IClock.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    await interaction.UpdateAsync(msg =>
  {
    clock.Filled = 0;
    msg.Components = clock.MakeComponents().Build();
    msg.Embed = clock.ToEmbed().Build();
  });
  }
  [ComponentInteraction("clock-advance")]
  public async Task AdvanceClock()
  {
    var interaction = Context.Interaction as SocketMessageComponent;
    var clock = IClock.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    await interaction.UpdateAsync(msg =>
    {
      clock.Filled++;
      msg.Components = clock.MakeComponents().Build();
      msg.Embed = clock.ToEmbed().Build();
    });
    await interaction.FollowupAsync(embed: clock.AlertEmbed().Build());
  }
  [ComponentInteraction("clock-menu")]
  public async Task AdvanceClockMenu(string[] values)
  {
    var interaction = Context.Interaction as SocketMessageComponent;
    var clock = IClock.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    string optionValue = values.FirstOrDefault();
    if (int.TryParse(optionValue.Replace("advance-", ""), out int odds))
    {
      OracleAnswer answer = new(odds, $"Does the clock *{clock.Text}* advance?");
      EmbedBuilder answerEmbed = answer.ToEmbed();
      if (answer.IsYes)
      {
        if (answer.IsMatch)
        {
          answerEmbed.WithFooter("You rolled a match! Envision how this situation or project gains dramatic support or inertia.");
        }
        await interaction.UpdateAsync(msg =>
        {
          clock.Filled += answer.IsMatch ? 2 : 1;
          msg.Embed = clock.ToEmbed().Build();
          msg.Components = clock.MakeComponents().Build();
        });
        string append = answer.IsMatch ? $"The clock advances twice to {clock.ToString()}." : $"The clock advances to {clock.ToString()}.";
        answerEmbed.Description = answerEmbed.Description + "\n" + append;
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