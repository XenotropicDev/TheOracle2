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

    public string ParentUrl => Interaction.Message.GetJumpUrl();
    public SocketMessageComponent Interaction => Context.Interaction as SocketMessageComponent;

    public Embed PrimaryEmbed => Interaction.Message.Embeds.FirstOrDefault();
    public bool SendAlerts => ILogWidget.ParseAlertStatus(Context.Interaction.Message.Components);

    private readonly Random Random;
    public EFContext DbContext { get; set; }

    [ComponentInteraction("progress-mark:*,*")]
    public async Task MarkProgress(string addTicksString, string currentTicksString)
    {
        if (!int.TryParse(currentTicksString, out int currentTicks)) { throw new ArgumentException($"Unable to parse current ticks from {currentTicksString}"); }

        if (!int.TryParse(addTicksString, out int addedTicks)) { throw new ArgumentException($"Unable to parse added ticks from {addedTicks}"); }
        ProgressTrack progressTrack = IProgressTrack.FromEmbed(DbContext, PrimaryEmbed, currentTicks, alerts: SendAlerts) as ProgressTrack;

        // EmbedBuilder alert = progressTrack.Mark(addedTicks);
        progressTrack.Mark(addedTicks);

        await Interaction.UpdateAsync(msg =>
        {
            msg.Components = progressTrack.MakeComponents().Build();
            msg.Embed = progressTrack.ToEmbed().Build();
        }).ConfigureAwait(false);

        // if (SendAlerts)
        // {
        //     await Interaction.FollowupAsync(embed: alert.Build()).ConfigureAwait(false);
        // }
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
        var progressTrack = IProgressTrack.FromEmbed(DbContext, PrimaryEmbed, ticksNew, alerts: SendAlerts);
        await Interaction.UpdateAsync(msg =>
        {
            msg.Components = progressTrack.MakeComponents().Build();
            msg.Embed = progressTrack.ToEmbed().Build();
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("progress-recommit:*,*")]
    public async Task RecommitProgress(string rankString, string currentTicksString)
    {
        if (!int.TryParse(currentTicksString, out int currentTicks))
        {
            throw new Exception($"Unable to parse {nameof(currentTicks)} from {currentTicksString}");
        }
        ChallengeRank rank = Enum.Parse<ChallengeRank>(rankString);
        ProgressTrack progressTrack = IProgressTrack.FromEmbed(DbContext, PrimaryEmbed, currentTicks, alerts: SendAlerts) as ProgressTrack;
        EmbedBuilder recommitAlert = progressTrack.Recommit(Random).WithUrl(ParentUrl);
        await Interaction.UpdateAsync(msg =>
        {
            msg.Embed = progressTrack.ToEmbed().Build();
            msg.Components = progressTrack.MakeComponents().Build();
        }).ConfigureAwait(false);
        // this sends an alert even if alerts are turned off because it involves displaying a roll result
        await Interaction.FollowupAsync(embed: recommitAlert.Build()).ConfigureAwait(false);
    }

    [ComponentInteraction("progress-roll:*,*")]
    public async Task RollProgress(string ticksString, string moveName)
    {
        if (!int.TryParse(ticksString, out int ticks))
        {
            throw new Exception($"Unable to parse {nameof(ticks)} from {ticksString}.");
        }
        var score = ITrack.GetScore(ticks);
        var roll = new ProgressRoll(Random, score, PrimaryEmbed.Title, moveName: moveName);
        var embed = roll.ToEmbed();
        embed.WithUrl(ParentUrl);
        await ReplyAsync(
          embed: embed.Build(),
          components: roll.MakeComponents().Build()
        ).ConfigureAwait(false);
    }

    [ComponentInteraction("progress-menu:*,*")]
    public async Task ProgressMenu(string rankString, string currentTicks, string[] values)
    {
        string option = values.FirstOrDefault();
        var optionLabel = Interaction.GetFirstSelectMenuLabel();
        string operation = option.Split(":")[0];
        string[] arguments = option.Split(":").Count() > 1 ? option.Split(":")[1].Split(",") : Array.Empty<string>();
        switch (operation)
        {
            case "progress-clear":
                await ClearProgress(
                    subtractedTickString: arguments[0],
                    currentTicksString: currentTicks).ConfigureAwait(false);
                break;

            case "progress-mark":
                await MarkProgress(
                    addTicksString: arguments[0],
                    currentTicksString: currentTicks)
                .ConfigureAwait(false);
                break;

            case "progress-roll":
                await RollProgress(
                    ticksString: currentTicks,
                    moveName: optionLabel)
                    .ConfigureAwait(false);
                break;

            case "progress-recommit":
                await RecommitProgress(
                    rankString: rankString,
                    currentTicksString: currentTicks)
                    .ConfigureAwait(false);
                break;
        }
        return;
    }

    [ComponentInteraction("clock-reset")]
    public async Task ResetClock()
    {
        var clock = IClock.FromEmbed(DbContext, PrimaryEmbed, alerts: SendAlerts);
        await Interaction.UpdateAsync(msg =>
        {
            clock.Filled = 0;
            msg.Components = clock.MakeComponents().Build();
            msg.Embed = clock.ToEmbed().Build();
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("clock-advance")]
    public async Task AdvanceClock()
    {
        var clock = IClock.FromEmbed(DbContext, PrimaryEmbed, alerts: SendAlerts);
        await Interaction.UpdateAsync(msg =>
        {
            clock.Filled++;
            msg.Components = clock.MakeComponents().Build();
            msg.Embed = clock.ToEmbed().Build();
        }).ConfigureAwait(false);
        // if (SendAlerts)
        // {
        //     await Interaction.FollowupAsync(embed: clock.AlertEmbed().Build()).ConfigureAwait(false);
        // }
    }

    [ComponentInteraction("clock-advance:*")]
    public async Task AdvanceClock(string oddsString)
    {
        if (!Enum.TryParse<AskOption>(oddsString, out AskOption odds))
        {
            throw new Exception($"Unable to parse odds from {oddsString}");
        }
        var clock = IClock.FromEmbed(DbContext, PrimaryEmbed, alerts: SendAlerts);

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
            resultString = $"The clock remains at {clock.Filled}/{clock.Segments}.";
            if (answer.IsMatch)
            {
                answerEmbed = answerEmbed.WithFooter("You rolled a match! Envision a surprising turn of events which pits new factors or forces against the clock.");
            }
        }
        answerEmbed.AddField("Result", resultString);
        answerEmbed = answerEmbed
            .WithUrl(ParentUrl)
            .WithThumbnailUrl(IClock.Images[clock.Segments][clock.Filled])
            .WithColor(IClock.ColorRamp[clock.Segments][clock.Filled]);
        await Interaction.UpdateAsync(msg =>
        {
            msg.Components = clock.MakeComponents().Build();
            msg.Embed = clock.ToEmbed().Build();
        }).ConfigureAwait(false);

        await Interaction.FollowupAsync(embed: answerEmbed.Build()).ConfigureAwait(false);
    }

    // TODO: refactor in same style as progress menu
    [ComponentInteraction("clock-menu")]
    public async Task ClockMenu(string[] values)
    {
        string optionValue = values.FirstOrDefault();
        if (optionValue.StartsWith("clock-advance:"))
        {
            var oddsString = optionValue.Split(":")[1];
            await AdvanceClock(oddsString).ConfigureAwait(false);
            return;
        }
        switch (optionValue)
        {
            case "clock-reset":
                await ResetClock().ConfigureAwait(false);
                return;
            case "clock-advance":
                await AdvanceClock().ConfigureAwait(false);
                return;
        }
    }

    [ComponentInteraction("scene-challenge-menu:*,*")]
    public async Task SceneChallengeMenu(string rankString, string ticksString, string[] values)
    {
        string optionValue = values.FirstOrDefault();
        if (optionValue.StartsWith("progress"))
        {
            await ProgressMenu(rankString, ticksString, values).ConfigureAwait(false);
            return;
        }
        if (optionValue.StartsWith("clock"))
        {
            await ClockMenu(values).ConfigureAwait(false);
            return;
        }
        return;
    }
}
