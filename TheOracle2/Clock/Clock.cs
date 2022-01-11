namespace TheOracle2.GameObjects;

public abstract class Clock : IClock
{
  protected Clock(Embed embed)
  {
    var values = IClock.Parseclock(embed);
    Title = embed.Title;
    Description = embed.Description;
    Filled = values.Item1;
    Segments = values.Item2;
  }
  protected Clock(ClockSize segments = (ClockSize)6, int filledSegments = 0, string title = "", string description = "")
  {
    if (filledSegments < 0 || filledSegments > ((int)segments))
    {
      throw new ArgumentOutOfRangeException(nameof(filledSegments), "filledSegments can't exceed segments");
    }
    Title = title;
    Segments = (int)segments;
    Filled = filledSegments;
    Description = description;
  }
  public int Segments { get; }
  public int Filled { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public abstract string EmbedCategory { get; }
  public bool IsFull => Filled >= Segments;
  public override string ToString()
  {
    return $"{Filled}/{Segments}";
  }

  public virtual EmbedBuilder ToEmbed()
  {
    return IClock.ToEmbedStub(EmbedCategory, Title, Segments, Filled).AddField(ToEmbedField());
  }
  public virtual EmbedFieldBuilder ToEmbedField()
  {
    return new EmbedFieldBuilder()
    .WithName("Clock")
    .WithValue(ToString())
    .WithIsInline(true);
  }
  public EmbedBuilder AlertEmbed()
  {
    EmbedBuilder embed = new EmbedBuilder().WithThumbnailUrl(
      IClock.Images[Segments][Filled])
    .WithColor(IClock.ColorRamp[Segments][Filled])
    .WithAuthor($"{EmbedCategory}: {Title}")
    .WithTitle(IsFull ? "The clock fills!" : $"The clock advances to {ToString()}");
    if (IsFull)
    {
      embed = embed.WithDescription(FillMessage);
    }
    return embed;
  }
  public virtual string FillMessage { get; set; } = "";
  public ButtonBuilder AdvanceButton()
  {
    return new ButtonBuilder()
    .WithLabel(IClock.AdvanceLabel)
    .WithStyle(ButtonStyle.Danger)
    .WithCustomId("clock-advance")
    .WithDisabled(IsFull)
    .WithEmote(new Emoji("🕦"));
  }
  public ButtonBuilder ResetButton()
  {
    return new ButtonBuilder()
    .WithLabel("Reset Clock")
    .WithStyle(ButtonStyle.Secondary)
    .WithCustomId("clock-reset")
    .WithDisabled(Filled == 0)
    .WithEmote(IClock.UxEmoji["reset"]);
  }
  public SelectMenuOptionBuilder ResetOption()
  {
    return new SelectMenuOptionBuilder()
    .WithLabel("Reset clock")
    .WithValue("clock-reset")
    .WithEmote(IClock.UxEmoji["reset"])
    .WithDefault(false);
  }
  public SelectMenuOptionBuilder AdvanceOption()
  {
    return new SelectMenuOptionBuilder()
    .WithLabel(IClock.AdvanceLabel)
    .WithValue("clock-advance")
    .WithEmote(new Emoji("🕦"))
    .WithDefault(false);
  }
  public virtual ComponentBuilder MakeComponents()
  {
    return new ComponentBuilder()
    .WithButton(AdvanceButton())
    .WithButton(ResetButton());
  }
}