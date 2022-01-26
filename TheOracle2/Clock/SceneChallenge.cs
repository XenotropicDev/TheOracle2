namespace TheOracle2.GameObjects;
using TheOracle2.UserContent;
public class SceneChallenge : ProgressTrack, IClock
{
    public SceneChallenge(EFContext dbContext, Embed embed, bool alerts = false) : base(dbContext, embed, alerts)
    {
        Tuple<int, int> clockData = IClock.ParseClock(embed);
        Filled = clockData.Item1;
        Segments = clockData.Item2;
    }
    public SceneChallenge(EFContext dbContext, Embed embed, int ticks, bool alerts = false) : base(dbContext, embed, ticks, alerts)
    {
        Tuple<int, int> clockData = IClock.ParseClock(embed);
        Filled = clockData.Item1;
        Segments = clockData.Item2;
    }
    public SceneChallenge(EFContext dbContext, SceneChallengeClockSize segments = (SceneChallengeClockSize)6, int filledSegments = 0, int ticks = 0, string title = "", string description = "", ChallengeRank rank = ChallengeRank.Formidable, bool alerts = false) : base(dbContext, rank, ticks, title, description, alerts)
    {
        Filled = filledSegments;
        Segments = (int)segments;
    }
    public SceneChallenge(EFContext dbContext, ClockSize segments = (ClockSize)6, int filledSegments = 0, int ticks = 0, string title = "", string description = "", ChallengeRank rank = ChallengeRank.Formidable, bool alerts = false) : base(dbContext, rank, ticks, title, description, alerts)
    {
        Filled = filledSegments;
        Segments = (int)segments;
    }
    public override string EmbedCategory => "Scene Challenge";
    public string FooterMessage { get; set; } = "When the tension clock is filled, time is up. You must resolve the encounter by making a progress roll.";
    public override string ResolveMoveName => "Resolve Scene Challenge";
    public override string TrackDescription => "Scene Challenge";
    public override bool CanRecommit => false;
    public override string MarkAlertTitle => "Mark Progress";
    public int Segments { get; }
    public int Filled { get; set; }
    public bool IsFull => Filled >= Segments;
    public string ClockFillMessage => "Time is up. You must resolve the encounter by making a progress roll.";
    public override EmbedBuilder ToEmbed()
    {
        return IClock.AddClockTemplate(base.ToEmbed(), this);
    }
    public SelectMenuBuilder MakeSelectMenu()
    {
        SelectMenuBuilder menu = ProgressTrack.MenuStub(this, "scene-challenge-");
        if (!IsFull)
        {
            if (Score < ITrack.TrackSize) { menu = menu.AddOption(MarkOption()); }
            menu = menu.AddOption(IClock.AdvanceOption());
        }
        menu = menu.AddOption(ResolveOption());
        if (Ticks > 0) { menu = menu.AddOption(ClearOption()); }
        if (Filled > 0) { menu = menu.AddOption(IClock.ResetOption()); }
        return menu;
    }
    public override ComponentBuilder MakeComponents()
    {
        var embed = new ComponentBuilder()
            .WithSelectMenu(MakeSelectMenu())
            // .WithButton(ILogWidget.ToggleAlertButton(LogOnIncrement))
            ;

        return embed;
    }

    public override EmbedBuilder AlertEmbed()
    {
        var embed = IClock.AlertStub(this);
        if (IsFull)
        { embed.WithDescription(ClockFillMessage); }

        return embed;
    }

    // TODO: this should probably return the scene challenge specific versions of the FS and SaA moves, meaning rsek needs to include them in Dataforged. while no formal move exists for progress resolution, its rules ought to be included in some way as well.
    public override string[] MoveReferences => Array.Empty<string>();
}
