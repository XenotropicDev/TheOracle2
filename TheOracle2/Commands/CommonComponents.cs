using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.GameObjects;
namespace TheOracle2;

public class CommonComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
  private readonly Random Random;
  public CommonComponents(Random random)
  {
    Random = random;
  }
  [ComponentInteraction("progress-mark:*")]
  public async Task MarkProgress(string tickString)
  {
    var interaction = Context.Interaction as SocketMessageComponent;
    int ticksToAdd = int.Parse(tickString);
    var progressTrack = IProgressTrack.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    await interaction.UpdateAsync(msg =>
    {
      progressTrack.Ticks += ticksToAdd;
      progressTrack.Ticks = Math.Max(0, Math.Min(progressTrack.Ticks, 40));
      msg.Components = progressTrack.MakeComponents().Build();
      msg.Embed = progressTrack.ToEmbed().Build();
    });
  }
  [ComponentInteraction("progress-clear:*")]
  public async Task ClearProgress(string tickString
  )
  {
    await MarkProgress("-" + tickString);
  }
  [ComponentInteraction("progress-recommit:*")]
  public async Task RecommitProgress()
  {
    var interaction = Context.Interaction as SocketMessageComponent;
    IProgressTrack.Recommit recommit = new(Random, interaction.Message.Embeds.FirstOrDefault());
    await interaction.UpdateAsync(msg =>
    {
      msg.Embed = recommit.NewTrack.ToEmbed().Build();
      msg.Components = recommit.NewTrack.MakeComponents().Build();
    });
    await interaction.FollowupAsync(embed: recommit.ToAlertEmbed().Build());
  }
  [ComponentInteraction("progress-roll:*")]
  public async Task RollProgress(string scoreString)
  {
    var interaction = Context.Interaction as SocketMessageComponent;
    var progressTrack = IProgressTrack.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    await interaction.RespondAsync(embed: progressTrack.Resolve(Random).ToEmbed().Build());
  }
  [ComponentInteraction("progress-menu")]
  public async Task ProgressMenu(string[] values)
  {
    string optionValue = values.FirstOrDefault();
    var interaction = Context.Interaction as SocketMessageComponent;
    IProgressTrack track = IProgressTrack.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    string operation = optionValue.Split(":")[0];
    string operationArg = optionValue.Split(":")[1];
    switch (operation)
    {
      case "progress-clear":
        await ClearProgress(operationArg);
        return;
      case "progress-mark":
        await MarkProgress(operationArg);
        return;
      case "progress-roll":
        await RollProgress(operationArg);
        return;
      case "progress-recommit":
        await RecommitProgress();
        return;
    }
    await interaction.UpdateAsync(msg =>
    {
      msg.Components = track.MakeComponents().Build();
      msg.Embed = track.ToEmbed().Build();
    });
    return;
  }
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
  public async Task ClockMenu(string[] values)
  {
    string optionValue = values.FirstOrDefault();
    var interaction = Context.Interaction as SocketMessageComponent;
    var clock = IClock.FromEmbed(interaction.Message.Embeds.FirstOrDefault());
    if (int.TryParse(optionValue.Replace("clock-advance-", ""), out int odds))
    {
      OracleAnswer answer = new(Random, odds, $"Does the clock *{clock.Title}* advance?");
      EmbedBuilder answerEmbed = answer.ToEmbed();
      if (answer.IsYes)
      {
        clock.Filled += answer.IsMatch ? 2 : 1;
        if (answer.IsMatch)
        {
          answerEmbed.WithFooter("You rolled a match! Envision how this situation or project gains dramatic support or inertia.");
        }
        string append = answer.IsMatch ? $"The clock advances **twice** to {clock.ToString()}." : $"The clock advances to {clock.ToString()}.";
        answerEmbed.Description += "\n" + append;
        answerEmbed = answerEmbed.WithThumbnailUrl(IClock.Images[clock.Segments][clock.Filled]);
      }
      if (!answer.IsYes)
      {
        if (answer.IsMatch)
        {
          answerEmbed = answerEmbed.WithFooter("You rolled a match! Envision a surprising turn of events which pits new factors or forces against the clock.");
        }
        answerEmbed.Description += "\n" + $"The clock remains at {clock.ToString()}";
      }
      answerEmbed = answerEmbed
        .WithThumbnailUrl(IClock.Images[clock.Segments][clock.Filled])
        .WithColor(IClock.ColorRamp[clock.Segments][clock.Filled]);
      await interaction.UpdateAsync(msg =>
      {
        msg.Components = clock.MakeComponents().Build();
        msg.Embed = clock.ToEmbed().Build();
      });
      await interaction.FollowupAsync(embed: answerEmbed.Build());
      return;
    }
    switch (optionValue)
    {
      case "clock-reset":
        clock.Filled = 0;
        break;
      case "clock-advance":
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
    return;
  }
  [ComponentInteraction("scene-challenge-menu")]
  public async Task SceneChallengeMenu(string[] values)
  {
    string optionValue = values.FirstOrDefault();
    if (optionValue.StartsWith("progress"))
    {
      await ProgressMenu(values);
      return;
    }
    if (optionValue.StartsWith("clock"))
    {
      await ClockMenu(values);
      return;
    }
  }
}
