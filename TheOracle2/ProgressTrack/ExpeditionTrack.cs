namespace TheOracle2.GameObjects;

using TheOracle2.UserContent;

public class ExpeditionTrack : ProgressTrack
{
    public ExpeditionTrack(EFContext dbContext, Embed embed, bool alerts = false) : base(dbContext, embed, alerts)
    {
    }

    public ExpeditionTrack(EFContext dbContext, Embed embed, int ticks, bool alerts = false) : base(dbContext, embed, ticks, alerts)
    {
    }

    public ExpeditionTrack(EFContext dbContext, ChallengeRank rank, int ticks = 0, string title = "", string description = "", bool alerts = false) : base(dbContext, rank, ticks, title, description, alerts)
    { }

    public override string TrackDescription => "Expedition";
    public override string MarkAlertTitle => "Undertake an Expedition";
    public override string ResolveMoveName => "Finish an Expedition";
    public override bool CanRecommit => true;

    public override string[] MoveReferences => new string[] {
    "Undertake an Expedition",
    "Explore a Waypoint",
    "Make a Discovery",
    "Confront Chaos",
    "Finish an Expedition",
    "Set a Course",
    };
}
