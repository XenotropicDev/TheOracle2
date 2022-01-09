namespace TheOracle2.GameObjects;

public abstract class Clock : IClock
{
  protected Clock(Embed embed)
  {
    Text = embed.Title;
    string[] clockString = embed.Description.Split("/");
    Filled = int.Parse(clockString[0]);
    Segments = int.Parse(clockString[1]);
  }
  protected Clock(EmbedField embedField)
  {
    Text = embedField.Name.Replace(ClockType + ": ", "");
    string[] clockString = embedField.Value.Split("/");
    Filled = int.Parse(clockString[0]);
    Segments = int.Parse(clockString[1]);
  }
  protected Clock(ClockSize segments = (ClockSize)6, int filledSegments = 0, string text = "")
  {
    if (filledSegments < 0 || filledSegments > ((int)segments))
    {
      throw new ArgumentOutOfRangeException(nameof(filledSegments), "filledSegments can't exceed segments");
    }
    Text = text;
    Segments = (int)segments;
    Filled = filledSegments;
  }

  public int Segments { get; }
  public int Filled { get; set; }
  public string Text { get; set; }
  public abstract string ClockType { get; }
  public bool IsFull => Filled >= Segments;
  public override string ToString()
  {
    return $"{Filled}/{Segments}";
  }
  public virtual EmbedBuilder ToEmbed()
  {
    return new EmbedBuilder()
      .WithAuthor(ClockType)
      .WithTitle(Text)
      .WithDescription(ToString())
      .WithThumbnailUrl(
        IClock.Images[Segments][Filled])
      .WithColor(
        IClock.ColorRamp[Segments][Filled])
      ;
  }
  public virtual EmbedFieldBuilder ToEmbedField()
  {
    return new EmbedFieldBuilder()
    .WithName($"{ClockType}: {Text}")
    .WithValue(ToString());
  }
  public EmbedBuilder AlertEmbed()
  {
    EmbedBuilder embed = new EmbedBuilder().WithThumbnailUrl(
      IClock.Images[Segments][Filled])
    .WithColor(IClock.ColorRamp[Segments][Filled])
    .WithAuthor($"{ClockType}: {Text}")
    .WithTitle(IsFull ? "The clock fills!" : $"The clock advances to {ToString()}");
    if (IsFull)
    {
      embed = embed.WithDescription(FillMessage);
    }
    return embed;
  }

  public virtual string FillMessage { get; set; }
  public ButtonBuilder AdvanceButton(string customId = "clock-advance")
  {
    return new ButtonBuilder()
    .WithLabel(IClock.AdvanceLabel)
    .WithStyle(ButtonStyle.Danger)
    .WithDisabled(IsFull)
    .WithCustomId(customId)
    .WithEmote(new Emoji("🕦"));
  }
  public ButtonBuilder ResetButton(string customId = "clock-reset")
  {
    return new ButtonBuilder()
    .WithLabel("Reset Clock")
    .WithStyle(ButtonStyle.Secondary)
    .WithCustomId(customId)
    .WithDisabled(Filled == 0)
    .WithEmote(IClock.UxEmoji["reset"]);
  }
  public virtual ComponentBuilder MakeComponents()
  {
    return new ComponentBuilder()
    .WithButton(AdvanceButton())
    .WithButton(ResetButton());
  }
}