namespace TheOracle2.GameObjects;
using TheOracle2.UserContent;

public class CombatTrack : ProgressTrack
{
  public CombatTrack(EFContext dbContext, Embed embed) : base(dbContext, embed) { }
  public CombatTrack(EFContext dbContext, Embed embed, int ticks) : base(dbContext, embed, ticks) { }
  public CombatTrack(EFContext dbContext, ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(dbContext, rank, ticks, title, description)
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