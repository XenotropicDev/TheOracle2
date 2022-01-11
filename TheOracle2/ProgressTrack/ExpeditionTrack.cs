namespace TheOracle2.GameObjects;

public class ExpeditionTrack : ProgressTrack
{
  public ExpeditionTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(rank, ticks, title, description)
  {

  }
  public override string EmbedCategory => "Expedition Track";
}