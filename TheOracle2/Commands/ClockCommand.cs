using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using TheOracle2.GameObjects;
namespace TheOracle2;
public class ClockCommand : InteractionModuleBase
{
  [SlashCommand("clock", "Creates a clock or scene challenge (see p. 230).")]
  public async Task BuildClock(
    [Summary(description: "The type of clock.")]
    [Choice("Tension clock", "tension-clock"),
    Choice("Campaign clock", "campaign-clock"),
    // Choice("Scene challenge", "scene-challenge")
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
    Embed embed = interaction.Message.Embeds.FirstOrDefault();
    Clock clock = Clock.FromEmbed(embed);
    clock.Reset();
    await interaction.UpdateAsync(msg =>
  {
    msg.Components = clock.MakeComponents().Build();
    msg.Embed = clock.ToEmbed().Build();
  });

  }
  [ComponentInteraction("advance-clock")]
  public async Task AdvanceClock()
  {
    SocketMessageComponent interaction = Context.Interaction as SocketMessageComponent;
    // SocketMessageComponentData data = interaction.Data;
    Embed embed = interaction.Message.Embeds.FirstOrDefault();
    Clock clock = Clock.FromEmbed(embed);
    clock.Advance();
    await interaction.UpdateAsync(msg =>
    {
      msg.Components = clock.MakeComponents().Build();
      msg.Embed = clock.ToEmbed().Build();
    });
  }


  [ComponentInteraction("advance-clock-menu")]
  public async Task AdvanceClockMenu(string customId, string[] values)
  {

    SocketMessageComponent interaction = Context.Interaction as SocketMessageComponent;
    string optionValue = values.FirstOrDefault();
    Embed embed = interaction.Message.Embeds.FirstOrDefault();
    Clock clock = Clock.FromEmbed(embed);

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
        clock.Advance(answer.IsMatch ? 2 : 1);
        await interaction.UpdateAsync(msg =>
        {
          msg.Embed = clock.ToEmbed().Build();
          msg.Components = clock.MakeComponents().Build();
        });
        answerEmbed = answerEmbed.WithThumbnailUrl(clock.GetImage());
        await interaction.FollowupAsync(embed: answerEmbed.Build());
        return;
      }

      if (answer.IsMatch)
      {
        answerEmbed.WithFooter("You rolled a match! Envision a surprising turn of events which pits new factors or forces against the clock.");
      }
      answerEmbed = answerEmbed.WithThumbnailUrl(clock.GetImage());
      await interaction.RespondAsync(
        embed: answerEmbed.Build());
      return;
    }

    switch (optionValue)
    {
      case "reset":
        clock.Reset();
        break;
      case "advance":
        clock.Advance();
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(optionValue), "Could not parse integer or valid string from select menu option value.");
        break;
    }

    await interaction.UpdateAsync(msg =>
  {
    msg.Components = clock.MakeComponents().Build();
    msg.Embed = clock.ToEmbed().Build();
  });
  }



  // [ComponentInteraction("advance-clock-*")]
  // public async Task AdvanceCampaignClock(string oddsString)
  // {
  //   int odds = int.Parse(oddsString);
  //   SocketMessageComponent interaction = Context.Interaction as SocketMessageComponent;
  //   Console.WriteLine($"interaction.Data.Values.FirstOrDefault(): {interaction.Data.Values.FirstOrDefault()}");
  //   Embed originalEmbed = interaction.Message.Embeds.FirstOrDefault();
  //   string originalEmbedTitle = originalEmbed.Title;
  //   string clockTitle = originalEmbed.Title.Length > 0 ? originalEmbed.Title : originalEmbed.Author.ToString();
  //   OracleAnswer answer = new(odds, $"Does *{clockTitle}* advance?");
  //   switch (answer.IsYes)
  //   {
  //     case true:
  //       CampaignClock clock = new(interaction.Message.Embeds.FirstOrDefault());
  //       int wedgesFilled = answer.IsMatch ? 2 : 1;
  //       clock.Advance(wedgesFilled);
  //       await interaction.UpdateAsync(msg =>
  //         {
  //           msg.Components = clock.MakeComponents().Build();
  //           msg.Embed = clock.ToEmbed().Build();
  //         });
  //       await interaction.FollowupAsync(embed: answer.ToEmbed().Build());
  //       break;
  //     case false:
  //       await interaction.RespondAsync(embed: answer.ToEmbed().Build());
  //       break;
  //   }
  // }
}

