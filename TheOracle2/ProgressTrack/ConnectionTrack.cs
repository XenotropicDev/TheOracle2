namespace TheOracle2.GameObjects;

public class ConnectionTrack : ProgressTrack
{
  public ConnectionTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(rank, ticks, title, description)
  {

  }
  public override string EmbedCategory => "Connection Track";
}