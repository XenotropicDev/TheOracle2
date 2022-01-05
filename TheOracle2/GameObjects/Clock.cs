using Discord;
namespace TheOracle2.GameObjects;

public class Clock
{
  public Clock(Embed embed)
  {
    // if (embed.Author.ToString() != "Clock")
    Text = embed.Title;
    FilledSegments = int.Parse(embed.Description.Split(" / ")[0]);
    Segments = int.Parse(embed.Description.Split(" / ")[1]);
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
    .WithLabel("Advance")
    .WithCustomId("advance-clock")
    .WithStyle(ButtonStyle.Primary)
    .WithEmote(new Emoji("🕦"))
    .WithDisabled(IsFull);
  }
}