namespace TheOracle2.GameObjects;
using TheOracle2.UserContent;

public class ExpeditionTrack : ProgressTrack
{

  public ExpeditionTrack(EFContext dbContext, Embed embed) : base(dbContext, embed) { }
  public ExpeditionTrack(EFContext dbContext, Embed embed, int ticks) : base(dbContext, embed, ticks) { }
  public ExpeditionTrack(EFContext dbContext, ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(dbContext, rank, ticks, title, description)
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