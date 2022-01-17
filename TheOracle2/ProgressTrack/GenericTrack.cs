using TheOracle2.UserContent;
namespace TheOracle2.GameObjects;
public class GenericTrack : ProgressTrack
{
  public GenericTrack(EFContext dbContext, Embed embed) : base(dbContext, embed) { }
  public GenericTrack(EFContext dbContext, Embed embed, int ticks) : base(dbContext, embed, ticks) { }
  public GenericTrack(EFContext dbContext, ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(dbContext, rank, ticks, title, description)
  { }
  public override string EmbedCategory => "Progress Track";
  public override bool CanRecommit => false;
  public override string TrackDescription => "Progress Track";
  public override string[] MoveReferences => Array.Empty<string>();
}