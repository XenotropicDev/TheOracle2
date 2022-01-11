namespace TheOracle2.GameObjects;
public class SceneChallenge : Clock, IProgressTrack
{
  public SceneChallenge(Embed embed) : base(embed)
  {
    Title = embed.Title;
    Description = embed.Description;
    Rank = IProgressTrack.ParseEmbedRank(embed);
    Ticks = IProgressTrack.ParseEmbedTicks(embed);
  }
  public SceneChallenge(ClockSize segments = (ClockSize)6, int filledSegments = 0, int ticks = 0, string title = "", string description = "") : base(segments, filledSegments, title, description)
  {
    Ticks = ticks;
  }
  public ChallengeRank Rank { get; set; } = ChallengeRank.Formidable;
  public RankData RankData { get => IProgressTrack.RankInfo[Rank]; }
  public int Ticks { get; set; }
  public int Score => (int)(Ticks / 4);
  public override string EmbedCategory { get; } = "Scene Challenge";

  public override string FillMessage { get; set; } = "When the tension clock is filled, time is up. You must resolve the encounter by making a progress roll.";

  public ProgressRoll Resolve(Random random)
  {
    return new ProgressRoll(random, Score, Title);
  }
  public override EmbedBuilder ToEmbed()
  {
    return IClock.ToEmbedStub(EmbedCategory, Title, Segments, Filled)
    .AddField(IProgressTrack.ToRankField(Rank))
    .AddField(IProgressTrack.ToProgressBarField(Ticks))
    .AddField(ToEmbedField())
    ;
  }
  public virtual ButtonBuilder ClearButton()
  {
    return IProgressTrack
      .ClearButton(RankData.MarkTrack)
        .WithDisabled(Ticks == 0);
  }
  public ButtonBuilder ResolveButton()
  {
    return IProgressTrack
      .ResolveButton(Score)
      .WithLabel("Resolve challenge")
    ;
  }
  public ButtonBuilder MarkButton()
  {
    return IProgressTrack
      .MarkButton(RankData.MarkTrack)
      .WithDisabled(Score >= 10 || IsFull)
    ;
  }
  public SelectMenuBuilder MakeSelectMenu()
  {
    SelectMenuBuilder menu = new SelectMenuBuilder()
    .WithCustomId("scene-challenge-menu")
    .WithPlaceholder("Manage scene challenge...")
    .WithMaxValues(1)
    .WithMinValues(0)
    ;
    if (!IsFull)
    {
      if (Score < 10) { menu = menu.AddOption(IProgressTrack.MarkOption(RankData.MarkTrack)); }
      menu = menu.AddOption(AdvanceOption());
    }
    menu = menu.AddOption(IProgressTrack.ResolveOption(Score));
    if (Ticks > 0) { menu = menu.AddOption(IProgressTrack.ClearOption(RankData.MarkTrack)); }
    if (Filled > 0) { menu = menu.AddOption(ResetOption()); }
    return menu;
  }
  public override ComponentBuilder MakeComponents()
  {
    return new ComponentBuilder()
      .WithSelectMenu(MakeSelectMenu())
      ;
  }
}