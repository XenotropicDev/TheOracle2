namespace TheOracle2.GameObjects;

public abstract class ProgressTrack : IProgressTrack
{
  protected internal ProgressTrack(Embed embed)
  {
    Rank = IProgressTrack.ParseEmbedRank(embed);
    Ticks = Math.Max(Math.Min(IProgressTrack.ParseEmbedTicks(embed), 40), 0);
    Title = embed.Title;
    Description = embed.Description;
  }

  protected internal ProgressTrack(ChallengeRank rank, int ticks = 0, string title = "", string description = "")
  {
    Rank = rank;
    Ticks = Math.Max(Math.Min(ticks, 40), 0);
    Title = title;
    Description = description;
  }
  public ChallengeRank Rank { get; set; }
  public int Ticks { get; set; }
  public RankData RankData { get => IProgressTrack.RankInfo[Rank]; }
  public string Title { get; set; }
  public string Description { get; set; }
  public int Score => (int)(Ticks / 4);
  public abstract string EmbedCategory { get; }
  public ProgressRoll Resolve(Random random)
  {
    return new ProgressRoll(random, Score, Title);
  }
  public EmbedBuilder ToEmbed()
  {
    return IProgressTrack.MakeEmbed(EmbedCategory, Rank, Title, Ticks);
  }
  public virtual ButtonBuilder ClearButton()
  {
    return IProgressTrack
      .ClearButton(RankData.MarkTrack)
        .WithDisabled(Ticks == 0);
  }
  public virtual SelectMenuOptionBuilder ClearOption()
  {
    return IProgressTrack.ClearOption(RankData.MarkTrack);
  }
  public virtual ButtonBuilder MarkButton()
  {
    return IProgressTrack
      .MarkButton(RankData.MarkTrack)
        .WithDisabled(Score >= 10);
  }
  public virtual SelectMenuOptionBuilder MarkOption()
  {
    return IProgressTrack.MarkOption(RankData.MarkTrack);
  }
  public virtual ButtonBuilder ResolveButton()
  {
    return IProgressTrack
      .ResolveButton(Score);
  }
  public virtual SelectMenuOptionBuilder ResolveOption()
  {
    return IProgressTrack.ResolveOption(Score);
  }
  public virtual ButtonBuilder RecommitButton()
  {
    return IProgressTrack
      .RecommitButton(Ticks, Rank);
  }

  public virtual SelectMenuOptionBuilder RecommitOption()
  {
    return IProgressTrack.RecommitOption(Ticks, Rank);
  }
  public virtual ComponentBuilder MakeComponents()
  {
    return new ComponentBuilder()
      .WithButton(ClearButton())
      .WithButton(MarkButton())
      .WithButton(ResolveButton());
  }
}