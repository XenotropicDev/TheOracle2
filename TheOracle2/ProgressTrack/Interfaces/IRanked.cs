namespace TheOracle2.GameObjects;

/// <summary>
/// Interface for game objects that have a challenge rank.
/// </summary>
public interface IRanked
{
    public ChallengeRank Rank { get; set; }

    public static IRanked IncreaseRank(IRanked rankedItem)
    {
        int oldRankInt = (int)rankedItem.Rank;
        int newRankInt = Math.Min(oldRankInt + 1, 5);
        rankedItem.Rank = (ChallengeRank)newRankInt;
        return rankedItem;
    }

    public static IRanked DecreaseRank(IRanked rankedItem)
    {
        int oldRankInt = (int)rankedItem.Rank;
        int newRankInt = Math.Max(oldRankInt - 1, 1);
        rankedItem.Rank = (ChallengeRank)newRankInt;
        return rankedItem;
    }

    public static EmbedFieldBuilder RankField(ChallengeRank rank)
    {
        return new EmbedFieldBuilder()
            .WithName("Rank")
            .WithValue(rank.ToString());
    }

    public static ChallengeRank ParseRank(EmbedField rankField)
    {
        return Enum.Parse<ChallengeRank>(rankField.Value);
    }

    public static ChallengeRank ParseRank(Embed embed)
    {
        EmbedField rankField = embed.Fields.FirstOrDefault(field => field.Name == "Rank");
        return ParseRank(rankField);
    }

    public static readonly Dictionary<ChallengeRank, RankData> RankInfo = new()
    {
        {
            ChallengeRank.Troublesome,
            new RankData(rank: ChallengeRank.Troublesome, markTrack: ITrack.BoxSize * 3, markLegacy: 1, suffer: 1)
        },
        {
            ChallengeRank.Dangerous,
            new RankData(rank: ChallengeRank.Dangerous, markTrack: ITrack.BoxSize * 2, markLegacy: 2, suffer: 2)
        },
        {
            ChallengeRank.Formidable,
            new RankData(rank: ChallengeRank.Formidable, markTrack: ITrack.BoxSize, markLegacy: ITrack.BoxSize, suffer: 2)
        },
        {
            ChallengeRank.Extreme,
            new RankData(rank: ChallengeRank.Extreme, markTrack: 2, markLegacy: ITrack.BoxSize * 2, suffer: 3)
        },
        {
            ChallengeRank.Epic,
            new RankData(rank: ChallengeRank.Epic, markTrack: 1, markLegacy: ITrack.BoxSize * 3, suffer: 3)
        }
    };
}
