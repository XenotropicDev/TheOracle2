using TheOracle2.Images;
namespace TheOracle2.GameObjects;

public abstract class Clock
{
  protected Clock(Embed embed)
  {
    Text = embed.Title;
    FilledSegments = ParseFilledSegments(embed.Description);
    Segments = ParseSegments(embed.Description);
  }
  protected Clock(EmbedField embedField)
  {
    Text = embedField.Name.Replace(ClockType + ": ", "");
    FilledSegments = ParseFilledSegments(embedField.Value);
    Segments = ParseSegments(embedField.Value);
  }
  protected Clock(ClockSize segments = (ClockSize)6, int filledSegments = 0, string text = "")
  {
    if (filledSegments < 0 || filledSegments > ((int)segments))
    {
      throw new ArgumentOutOfRangeException(nameof(filledSegments), "filledSegments can't exceed segments");
    }
    Text = text;
    Segments = (int)segments;
    FilledSegments = filledSegments;
  }

  public void Advance(int amount = 1)
  {
    FilledSegments += amount;
    if (FilledSegments > Segments)
    {
      FilledSegments = Segments;
    }
  }
  public void Reset()
  {
    FilledSegments = 0;
  }

  public EmbedBuilder ToEmbed()
  {
    return new EmbedBuilder()
      .WithAuthor(ClockType)
      .WithTitle(Text)
      .WithDescription(ToString())
      .WithThumbnailUrl(GetImage())
      ;
  }

  public EmbedFieldBuilder ToEmbedField(bool IsInline = true)
  {
    return new EmbedFieldBuilder()
    .WithName($"{ClockType}: {Text}")
    .WithValue(ToString())
    .WithIsInline(IsInline);
  }

  private static int ParseFilledSegments(string clockString, string delimiter = " / ")
  {
    return int.Parse(clockString.Split(delimiter)[0]);
  }
  private static int ParseSegments(string clockString, string delimiter = " / ")
  {
    return int.Parse(clockString.Split(delimiter)[1]);
  }
  public int Segments { get; }

  public string Text { get; set; }
  public int FilledSegments { get; set; }
  public override string ToString()
  {
    return $"{FilledSegments} / {Segments}";
  }


  public abstract string ClockType { get; }
  public bool IsFull { get => FilledSegments >= Segments; }
  public static string AdvanceLabel { get => "Advance Clock"; }
  public virtual ButtonBuilder AdvanceButton(string customId = "advance-clock")
  {
    return new ButtonBuilder()
    .WithLabel(AdvanceLabel)
    .WithStyle(ButtonStyle.Primary)
    .WithDisabled(IsFull)
    .WithCustomId(customId)
    .WithEmote(new Emoji("🕦"));
  }
  public virtual ButtonBuilder ResetButton(string customId = "reset-clock")
  {
    return new ButtonBuilder()
    .WithLabel("Reset Clock")
    .WithStyle(ButtonStyle.Danger)
    .WithCustomId(customId)
    .WithEmote(UxEmoji["reset"]);
  }
  public virtual ComponentBuilder MakeComponents()
  {
    return new ComponentBuilder()
    .WithButton(AdvanceButton())
    .WithButton(ResetButton());
  }
  protected static Dictionary<string, Emoji> UxEmoji
  {
    get => new Dictionary<string, Emoji>(){
      {"reset", new Emoji("↩️")}
    };
  }

  protected static Dictionary<int, Emoji> OddsEmoji
  {
    get => new Dictionary<int, Emoji>(){
      { 10, new Emoji("🕐")},
      { 25, new Emoji("🕒")},
      { 50, new Emoji("🕧")},
      { 75, new Emoji("🕘")},
      { 90, new Emoji("🕚")},
      {100, new Emoji("🕛")}
    };
  }
  protected static Dictionary<int, string[]> Images
  {
    get => new ClockImages();
  }
  public string GetImage()
  {
    return Images[Segments].ElementAt(FilledSegments);
  }

  public static Clock FromEmbed(Embed embed)
  {
    switch (embed.Author.ToString())
    {
      case "Campaign Clock":
        return new CampaignClock(embed);
      case "Tension Clock":
        return new TensionClock(embed);
      // case "Scene Challenge":
      //   return new SceneChallenge(embed);
      //   break;
      default:
        throw new ArgumentOutOfRangeException(nameof(embed), "Embed must be a 'Campaign Clock', 'Tension Clock', or 'Scene Challenge'");
    }
  }
}