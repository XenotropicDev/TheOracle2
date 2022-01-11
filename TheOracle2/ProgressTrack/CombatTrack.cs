namespace TheOracle2.GameObjects;

public class CombatTrack : ProgressTrack
{
  public CombatTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "") : base(rank, ticks, title, description)
  { }
  public override string EmbedCategory => "Combat Track";
}