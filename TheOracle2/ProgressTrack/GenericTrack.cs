namespace TheOracle2.GameObjects;

public class GenericTrack : ProgressTrack
{
  public GenericTrack(Embed embed) : base(embed) { }
  public GenericTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(rank, ticks, title, description)
  { }
  public override string EmbedCategory => "Progress Track";
}