using TheOracle2.UserContent;

namespace TheOracle2.GameObjects;

public class VowTrack : ProgressTrack
{
    public VowTrack(EFContext dbContext, Embed embed, bool alerts = false) : base(dbContext, embed, alerts)
    {
    }

    public VowTrack(EFContext dbContext, Embed embed, int ticks, bool alerts = false) : base(dbContext, embed, ticks, alerts)
    {
    }

    public VowTrack(EFContext dbContext, ChallengeRank rank, int ticks = 0, string title = "", string description = "", bool alerts = false) : base(dbContext, rank, ticks, title, description, alerts)
    { }

    public override string TrackDescription => "Vow";
    public override string ResolveMoveName => "Fulfill Your Vow";
    public override string MarkAlertTitle => "Reach a Milestone";
    public override bool CanRecommit => true;

    public override string[] MoveReferences => new string[] {
    "Swear an Iron Vow",
    "Reach a Milestone",
    "Fulfill Your Vow",
    "Forsake Your Vow", };
}
