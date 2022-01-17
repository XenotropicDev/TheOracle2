namespace TheOracle2.GameObjects;
using TheOracle2.UserContent;

public class ConnectionTrack : ProgressTrack
{
  public ConnectionTrack(EFContext dbContext, Embed embed) : base(dbContext, embed) { }
  public ConnectionTrack(EFContext dbContext, Embed embed, int ticks) : base(dbContext, embed, ticks) { }
  public ConnectionTrack(EFContext dbContext, ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(dbContext, rank, ticks, title, description)
  { }

  public override string MarkAlertTitle => "Develop Your Relationship";
  public override bool CanRecommit => true;
  public override string ResolveMoveName => "Forge a Bond";
  public override string TrackDescription => "Connection";
  public override string[] MoveReferences => new string[] {
    "Make a Connection",
    "Develop Your Relationship",
    "Test Your Relationship",
    "Forge a Bond" };
}