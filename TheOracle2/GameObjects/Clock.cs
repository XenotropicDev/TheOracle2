using Discord;
namespace TheOracle2.GameObjects;

public class Clock
{
  public Clock(Embed embed)
  {
    Text = embed.Title;
    FilledSegments = ParseFilledSegments(embed.Description);
    Segments = ParseSegments(embed.Description);
  }
  public Clock(EmbedField embedField)
  {
    Text = embedField.Name.Replace(ClockType+": ", "");
    FilledSegments = ParseFilledSegments(embedField.Value);
    Segments = ParseSegments(embedField.Value);
  }
  public Clock(ClockSize segments = (ClockSize)6, int filledSegments = 0, string text = "")
  {
    if (filledSegments < 0 || filledSegments > ((int)segments))
    {
      throw new ArgumentOutOfRangeException(nameof(filledSegments), "filledSegments can't exceed segments");
    }
    Text = text;
    Segments = (int)segments;
    FilledSegments = filledSegments;
  }
  public EmbedBuilder ToEmbed()
  {
    return new EmbedBuilder()
      .WithAuthor(ClockType)
      .WithTitle(Text)
      .WithDescription(ToString())
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
  public void Advance(int amount = 1)
  {
    FilledSegments += amount;
    if (FilledSegments > Segments)
    {
      FilledSegments = Segments;
    }
  }

  public virtual string ClockType { get => "Clock"; }
  public bool IsFull { get => FilledSegments >= Segments; }

  public ButtonBuilder AdvanceButton()
  {
    return new ButtonBuilder()
    .WithLabel("Advance Clock")
    .WithCustomId("advance-clock")
    .WithStyle(ButtonStyle.Primary)
    .WithEmote(new Emoji("🕦"))
    .WithDisabled(IsFull);
  }
}