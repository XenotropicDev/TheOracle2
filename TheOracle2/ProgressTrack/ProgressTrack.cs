namespace TheOracle2.GameObjects;
using TheOracle2.UserContent;

public abstract class ProgressTrack : IProgressTrack, IMoveRef
{
  protected internal ProgressTrack(EFContext dbContext, Embed embed)
  {
    Rank = IProgressTrack.ParseRank(embed);
    Ticks = ITrack.ParseTrack(embed);
    Title = embed.Title;
    Description = embed.Description;
    DbContext = dbContext;
  }
  protected internal ProgressTrack(EFContext dbContext, Embed embed, int ticks)
  {
    Rank = IProgressTrack.ParseRank(embed);
    Ticks = ticks;
    Title = embed.Title;
    Description = embed.Description;
    DbContext = dbContext;
  }
  protected internal ProgressTrack(EFContext dbContext, ChallengeRank rank, int ticks = 0, string title = "", string description = "")
  {
    Rank = rank;
    Ticks = ticks;
    Title = title;
    Description = description;
    DbContext = dbContext;
  }

  public EFContext DbContext { get; }
  public bool LogOnIncrement { get; }
  public bool LogOnDecrement { get; }
  public bool AlertOnIncrement { get; }
  public bool AlertOnDecrement { get; }
  public EmbedBuilder AlertEmbed()
  {
    return new EmbedBuilder();
  }
  public ChallengeRank Rank { get; set; }
  private int _ticks;
  /// <summary>
  /// The current number of progress ticks, from 0 to 40.
  /// </summary>
  public int Ticks { get => _ticks; set => _ticks = Math.Max(0, Math.Min(value, ITrack.MaxTicks)); }
  public RankData RankData => IProgressTrack.RankInfo[Rank];
  public int Score => ITrack.GetScore(Ticks);
  /// <inheritdoc/>
  public virtual string EmbedCategory => TrackDescription + " Progress Track";
  /// <summary>
  /// A short string describing what the track represents, like "Vow" or "Combat Objective". Used in formatting certain output; EmbedCategory defaults to this + " Progress Track".
  /// </summary>
  public abstract string TrackDescription { get; }
  /// <inheritdoc/>
  public string Title { get; set; }
  /// <inheritdoc/>
  public string Description { get; set; }
  /// <inheritdoc/>
  public string Footer { get; set; }
  public string AlertFooter { get; set; }
  public string LogMessage { get; set; }
  public ProgressRoll Roll(Random random)
  {
    return new ProgressRoll(random, Score, Title);
  }
  /// <inheritdoc/>
  public virtual EmbedBuilder ToEmbed()
  {
    EmbedBuilder embed = new EmbedBuilder()
      .WithAuthor(EmbedCategory)
      .WithTitle(Title)
      .WithDescription(Description);
    return IProgressTrack.ProgressTemplate(this, embed);
  }
  /// <summary>
  /// Generates a menu option for clearing one unit of progress.
  /// </summary>
  public virtual SelectMenuOptionBuilder ClearOption()
  {
    return IProgressTrack.ClearOption(RankData.MarkTrack);
  }
  public virtual string MarkAlertTitle => "Mark Progress";

  /// <summary>
  /// Generates a menu option for marking one unit of progress.
  /// </summary>
  public virtual SelectMenuOptionBuilder MarkOption()
  {
    return IProgressTrack.MarkOption(RankData.MarkTrack, MarkAlertTitle);
  }
  public virtual string ResolveMoveName => $"Resolve {EmbedCategory}";

  /// <summary>
  /// Generates a menu option for making a progress roll with the track's current progress score.
  /// </summary>
  public virtual SelectMenuOptionBuilder ResolveOption()
  {
    return IProgressTrack.ResolveOption(ResolveMoveName);
  }

  public abstract string[] MoveReferences { get; }

  public virtual SelectMenuBuilder MoveRefMenu()
  {
    return IMoveRef.MenuBase(this);
  }
  public static SelectMenuBuilder MenuStub(ProgressTrack track, string prefix = "progress-", string suffix = "")
  {
    return new SelectMenuBuilder()
    .WithPlaceholder($"Manage {track.TrackDescription.ToLowerInvariant()}...")
    .WithCustomId(prefix + $"menu:{track.Rank},{track.Ticks}" + suffix)
    .WithMaxValues(1)
    .WithMinValues(0)
    ;
  }
  /// <summary>
  /// Whether this type of progress track should display buttons or menu options to recommit.
  /// </summary>
  public abstract bool CanRecommit { get; }
  public virtual SelectMenuOptionBuilder RecommitOption()
  {
    return IProgressTrack.RecommitOption($"Recommit after a Miss on {ResolveMoveName}.");
  }
  public virtual SelectMenuBuilder MakeMenu()
  {
    SelectMenuBuilder menu = MenuStub(this);
    if (Ticks < 40)
    {
      menu = menu.AddOption(MarkOption());
    }

    menu.AddOption(ResolveOption());

    if (CanRecommit)
    {
      menu.AddOption(RecommitOption());
    }

    if (Ticks > 0)
    {
      menu = menu.AddOption(ClearOption());
    }
    return menu;
  }
  /// <inheritdoc/>
  public virtual ComponentBuilder MakeComponents()
  {
    ComponentBuilder components = new ComponentBuilder().WithSelectMenu(MakeMenu());
    if (MoveReferences.Length > 0)
    {
      components.WithSelectMenu(MoveRefMenu());
    }
    return components;
  }
  /// <summary>
  /// Recommits to a vow after a miss (for e.g. Fulfill Your Vow); updates the ProgressTrack and returns an alert embed that summarizes the result.
  /// </summary>
  public virtual EmbedBuilder Recommit(Random random)
  {
    EmbedBuilder embed = new EmbedBuilder()
    .WithAuthor(ResolveMoveName + ": " + Title)
    .WithTitle("Recommit");
    string oldRank = Rank.ToString();
    ChallengeDice dice = new(random);
    embed.AddField(dice.ToEmbedField().WithIsInline(true));

    int progressCleared = dice.Min();
    Ticks -= progressCleared * ITrack.BoxSize;
    Rank = (ChallengeRank)Math.Min(((int)Rank) + 1, (int)ChallengeRank.Epic);
    embed.AddField(
      "Progress Cleared", $"{progressCleared}", true);
    embed = IProgressTrack.ProgressTemplate(this, embed);
    if (Rank.ToString() != oldRank)
    { embed.Fields?.FirstOrDefault(field => field.Name == "Rank")?.WithValue($"~~{oldRank}~~ {this.Rank}"); }
    return embed;
  }
  /// <inheritdoc/>
  public EmbedBuilder Mark(int addTicks, string alertTitle)
  {
    Ticks += addTicks;
    string emojiString = string.Join(" ", ITrack.TicksToEmojiList(addTicks));
    string tickString = ITrack.TickString(addTicks);
    string message = $"{tickString} of progress marked. Progress is now {ITrack.TickString(Ticks)}.";
    return new EmbedBuilder()
      .WithAuthor(TrackDescription + ": " + Title)
      .WithTitle(alertTitle + " " + emojiString)
      .WithDescription(message)
      ;
  }
  /// <inheritdoc/>
  public EmbedBuilder Mark(int addTicks)
  {
    return Mark(addTicks, MarkAlertTitle);
  }
  /// <inheritdoc/>
  public EmbedBuilder Mark()
  {
    return Mark(RankData.MarkTrack);
  }
}