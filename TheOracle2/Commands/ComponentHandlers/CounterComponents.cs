using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2;

/// <summary>
/// Progress and clock message components that are used by multiple slash commands.
/// </summary>
public class CounterComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public CounterComponents(EFContext dbContext, Random random)
    {
        Random = random;
        DbContext = dbContext;
    }

    private readonly Random Random;
    public EFContext DbContext { get; set; }

    [ComponentInteraction("progress-mark:*,*")]
    public async Task MarkProgress(string addTicksString, string currentTicksString)
    {
        SocketMessageComponent interaction = Context.Interaction as SocketMessageComponent;

        if (!int.TryParse(currentTicksString, out int currentTicks)) { throw new ArgumentException($"Unable to parse current ticks from {currentTicksString}"); }

        if (!int.TryParse(addTicksString, out int addedTicks)) { throw new ArgumentException($"Unable to parse added ticks from {addedTicks}"); }
        var alerts = ILogWidget.ParseAlertStatus(interaction.Message.Components);
        ProgressTrack progressTrack = IProgressTrack.FromEmbed(DbContext, interaction.Message.Embeds.FirstOrDefault(), currentTicks, alerts: alerts) as ProgressTrack;

        EmbedBuilder alert = progressTrack.Mark(addedTicks);

        await interaction.UpdateAsync(msg =>
        {
            msg.Components = progressTrack.MakeComponents().Build();
            msg.Embed = progressTrack.ToEmbed().Build();
        });

        if (ILogWidget.ParseAlertStatus(interaction.Message.Components))
        {
            await interaction.FollowupAsync(embed: alert.Build()).ConfigureAwait(false);
        }
    }

    [ComponentInteraction("progress-clear:*,*")]
    public async Task ClearProgress(string subtractedTickString, string currentTicksString
    )
    {
        if (!int.TryParse(subtractedTickString, out int subtractTicks))
        {
            throw new Exception($"Unable to parse {nameof(subtractedTickString)} from {subtractedTickString}");
        }
        if (!int.TryParse(currentTicksString, out int currentTicks))
        {
            throw new Exception($"Unable to parse {nameof(currentTicks)} from {currentTicksString}");
        }
        var ticksNew = currentTicks - subtractTicks;
        var interaction = Context.Interaction as SocketMessageComponent;
        var alerts = ILogWidget.ParseAlertStatus(interaction.Message.Components);
        var progressTrack = IProgressTrack.FromEmbed(DbContext, interaction.Message.Embeds.FirstOrDefault(), ticksNew, alerts: alerts);
        await interaction.UpdateAsync(msg =>
        {
            msg.Components = progressTrack.MakeComponents().Build();
            msg.Embed = progressTrack.ToEmbed().Build();
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("progress-recommit:*,*")]
    public async Task RecommitProgress(string rankString, string currentTicksString)
    {
        SocketMessageComponent interaction = Context.Interaction as SocketMessageComponent;
        if (!int.TryParse(currentTicksString, out int currentTicks))
        {
            throw new Exception($"Unable to parse {nameof(currentTicks)} from {currentTicksString}");
        }
        var alerts = ILogWidget.ParseAlertStatus(interaction.Message.Components);
        ChallengeRank rank = Enum.Parse<ChallengeRank>(rankString);
        ProgressTrack progressTrack = IProgressTrack.FromEmbed(DbContext, interaction.Message.Embeds.FirstOrDefault(), currentTicks, alerts: alerts) as ProgressTrack;
        EmbedBuilder recommitAlert = progressTrack.Recommit(Random);
        await interaction.UpdateAsync(msg =>
        {
            msg.Embed = progressTrack.ToEmbed().Build();
            msg.Components = progressTrack.MakeComponents().Build();
        }).ConfigureAwait(false);
        // this receives an alert even if alerts are turned off because it involves displaying a roll result
        await interaction.FollowupAsync(embed: recommitAlert.Build()).ConfigureAwait(false);
    }

    [ComponentInteraction("progress-roll:*")]
    public async Task RollProgress(string ticksString, string moveName)
    {
        if (!int.TryParse(ticksString, out int ticks))
        {
            throw new Exception($"Unable to parse {nameof(ticks)} from {ticksString}");
        }
        var score = ITrack.GetScore(ticks);
        var interaction = Context.Interaction as SocketMessageComponent;
        var embed = interaction.Message.Embeds.FirstOrDefault();
        var roll = new ProgressRoll(Random, score, embed.Title, moveName: moveName);
        await ReplyAsync(
          embed: roll.ToEmbed().Build(),
          components: roll.MakeComponents().Build()
        ).ConfigureAwait(false);
    }

    [ComponentInteraction("progress-menu:*,*")]
    public async Task ProgressMenu(string rankString, string currentTicks, string[] values)
    {
        string option = values.FirstOrDefault();
        string operation = option.Split(":")[0];
        string[] arguments = option.Split(":").Length > 1 ? option.Split(":")[1].Split(",") : Array.Empty<string>();
        switch (operation)
        {
            case "progress-clear":
                await ClearProgress(subtractedTickString: arguments[0], currentTicksString: currentTicks);
                return;

            case "progress-mark":
                await MarkProgress(addTicksString: arguments[0], currentTicksString: currentTicks);
                return;

            case "progress-roll":
                await RollProgress(currentTicks, arguments[0]);
                return;

            case "progress-recommit":
                await RecommitProgress(rankString: rankString, currentTicksString: currentTicks);
                return;
        }
        return;
    }

    [ComponentInteraction("clock-reset")]
    public async Task ResetClock()
    {
        var interaction = Context.Interaction as SocketMessageComponent;
        var alerts = ILogWidget.ParseAlertStatus(interaction.Message.Components);
        var clock = IClock.FromEmbed(DbContext, interaction.Message.Embeds.FirstOrDefault(), alerts: alerts);
        await interaction.UpdateAsync(msg =>
        {
            clock.Filled = 0;
            msg.Components = clock.MakeComponents().Build();
            msg.Embed = clock.ToEmbed().Build();
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("clock-advance")]
    public async Task AdvanceClock()
    {
        var interaction = Context.Interaction as SocketMessageComponent;
        var alerts = ILogWidget.ParseAlertStatus(interaction.Message.Components);
        var clock = IClock.FromEmbed(DbContext, interaction.Message.Embeds.FirstOrDefault(), alerts: alerts);
        await interaction.UpdateAsync(msg =>
        {
            clock.Filled++;
            msg.Components = clock.MakeComponents().Build();
            msg.Embed = clock.ToEmbed().Build();
        }).ConfigureAwait(false);
        if (ILogWidget.ParseAlertStatus(interaction.Message.Components))
        {
            await interaction.FollowupAsync(embed: clock.AlertEmbed().Build()).ConfigureAwait(false);
        }
    }

    [ComponentInteraction("clock-advance:*")]
    public async Task AdvanceClock(string oddsString)
    {
        if (!Enum.TryParse<AskOption>(oddsString, out AskOption odds))
        {
            throw new Exception($"Unable to parse odds from {oddsString}");
        }
        var interaction = Context.Interaction as SocketMessageComponent;
        var alerts = ILogWidget.ParseAlertStatus(interaction.Message.Components);
        var clock = IClock.FromEmbed(DbContext, interaction.Message.Embeds.FirstOrDefault(), alerts: alerts);

        OracleAnswer answer = new(Random, odds, $"Does the clock *{clock.Title}* advance?");
        EmbedBuilder answerEmbed = answer.ToEmbed();
        string resultString = "";
        if (answer.IsYes)
        {
            clock.Filled += answer.IsMatch ? 2 : 1;
            resultString = answer.IsMatch ? $"The clock advances **twice** to {clock.Filled}/{clock.Segments}." : $"The clock advances to {clock.Filled}/{clock.Segments}.";
            answerEmbed = answerEmbed.WithThumbnailUrl(IClock.Images[clock.Segments][clock.Filled]);
            if (answer.IsMatch)
            {
                answerEmbed.WithFooter("You rolled a match! Envision how this situation or project gains dramatic support or inertia.");
            }
        }
        if (!answer.IsYes)
        {
            resultString = $"The clock remains at {clock.Filled}/{clock.Segments}";
            if (answer.IsMatch)
            {
                answerEmbed = answerEmbed.WithFooter("You rolled a match! Envision a surprising turn of events which pits new factors or forces against the clock.");
            }
        }
        answerEmbed.AddField("Result", resultString);
        answerEmbed = answerEmbed
          .WithThumbnailUrl(IClock.Images[clock.Segments][clock.Filled])
          .WithColor(IClock.ColorRamp[clock.Segments][clock.Filled]);
        await interaction.UpdateAsync(msg =>
        {
            msg.Components = clock.MakeComponents().Build();
            msg.Embed = clock.ToEmbed().Build();
        }).ConfigureAwait(false);
        // this is intentionally left insensitive to IWidget.ParseAlertStatus, because it's an oracle answer, not simply an increment alert
        await interaction.FollowupAsync(embed: answerEmbed.Build()).ConfigureAwait(false);
    }

    // TODO: refactor in same style as progress menu
    [ComponentInteraction("clock-menu")]
    public async Task ClockMenu(string[] values)
    {
        string optionValue = values.FirstOrDefault();
        if (optionValue.StartsWith("clock-advance:"))
        {
            var oddsString = optionValue.Split(":")[1];
            await AdvanceClock(oddsString);
            return;
        }
        switch (optionValue)
        {
            case "clock-reset":
                await ResetClock();
                return;
            case "clock-advance":
                await AdvanceClock();
                return;
        }
    }

    [ComponentInteraction("scene-challenge-menu:*,*")]
    public async Task SceneChallengeMenu(string rankString, string ticksString, string[] values)
    {
        string optionValue = values.FirstOrDefault();
        if (optionValue.StartsWith("progress"))
        {
            await ProgressMenu(rankString, ticksString, values);
            return;
        }
        if (optionValue.StartsWith("clock"))
        {
            await ClockMenu(values);
            return;
        }
    }
}