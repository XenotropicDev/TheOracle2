using TheOracle2.UserContent;
namespace TheOracle2.GameObjects;
using System.Text.Json;
public class VowTrack : ProgressTrack
{
  public VowTrack(EFContext dbContext, Embed embed) : base(dbContext, embed) { }
  public VowTrack(EFContext dbContext, Embed embed, int ticks) : base(dbContext, embed, ticks) { }
  public VowTrack(EFContext dbContext, ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(dbContext, rank, ticks, title, description)
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