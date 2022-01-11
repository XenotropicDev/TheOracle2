namespace TheOracle2;

public class RankData
{
  public RankData(ChallengeRank rank, int markTrack, int markLegacy, int? suffer = null)
  {
    Name = rank.ToString();
    Value = (int)rank;
    MarkTrack = markTrack;
    MarkLegacy = markLegacy;
    Suffer = suffer;
  }
  public string Name { get; set; }
  public int Value { get; set; }
  public int MarkTrack { get; set; }
  public int MarkLegacy { get; set; }
  public int? Suffer { get; set; }
  public ChallengeRank ToEnumValue()
  {
    return (ChallengeRank)Value;
  }
}

public enum ChallengeRank
{
  None,
  Troublesome,
  Dangerous,
  Formidable,
  Extreme,
  Epic
}