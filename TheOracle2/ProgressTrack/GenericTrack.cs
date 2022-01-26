using TheOracle2.UserContent;

namespace TheOracle2.GameObjects;

public class GenericTrack : ProgressTrack
{
    public GenericTrack(EFContext dbContext, Embed embed, bool alerts = false) : base(dbContext, embed, alerts)
    {
    }

    public GenericTrack(EFContext dbContext, Embed embed, int ticks, bool alerts = false) : base(dbContext, embed, ticks, alerts)
    {
    }

    public GenericTrack(EFContext dbContext, ChallengeRank rank, int ticks = 0, string title = "", string description = "", bool alerts = false) : base(dbContext, rank, ticks, title, description, alerts)
    { }

    public override string EmbedCategory => "Progress Track";
    public override bool CanRecommit => false;
    public override string TrackDescription => "Progress Track";
    public override string[] MoveReferences => Array.Empty<string>();
}
