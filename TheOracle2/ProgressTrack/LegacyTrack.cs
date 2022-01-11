namespace TheOracle2.GameObjects;

public class LegacyTrack : ProgressTrack
{
  public LegacyTrack(Legacy legacy) : base(ChallengeRank.None)
  {
    Legacy = legacy;
  }
  public Legacy Legacy;
  public override string EmbedCategory => $"{Legacy} Legacy Track";
  public void Mark(ChallengeRank rewardRank)
  {
    Ticks += IProgressTrack.RankInfo[rewardRank].MarkLegacy;
  }
}

public enum Legacy { Quests, Discoveries, Bonds }