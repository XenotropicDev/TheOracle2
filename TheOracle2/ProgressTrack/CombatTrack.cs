namespace TheOracle2.GameObjects;

using TheOracle2.UserContent;

public class CombatTrack : ProgressTrack
{
    public CombatTrack(EFContext dbContext, Embed embed, bool alerts = false) : base(dbContext, embed, alerts)
    {
    }

    public CombatTrack(EFContext dbContext, Embed embed, int ticks, bool alerts = false) : base(dbContext, embed, ticks, alerts)
    {
    }

    public CombatTrack(EFContext dbContext, ChallengeRank rank, int ticks = 0, string title = "", string description = "", bool alerts = false) : base(dbContext, rank, ticks, title, description, alerts)
    { }

    public override string TrackDescription => "Combat Objective";
    public override bool CanRecommit => false;
    public override string ResolveMoveName => "Take Decisive Action";

    public override string[] MoveReferences => new string[] {
      "Enter the Fray",
      "Gain Ground",
      "React Under Fire",
      "Strike",
      "Clash",
      "Take Decisive Action",
      "Face Defeat",
      "Battle"
    };
}
