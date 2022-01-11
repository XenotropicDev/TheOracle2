namespace TheOracle2.GameObjects;

public class VowTrack : ProgressTrack
{
  public VowTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(rank, ticks, title, description)
  {

  }
  public override string EmbedCategory => "Vow Track";
}