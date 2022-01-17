namespace TheOracle2.GameObjects;

public abstract class Clock : IClock
{
  protected Clock(Embed embed)
  {
    var values = IClock.ParseClock(embed);
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
  public string Footer { get; set; }
  public abstract string EmbedCategory { get; }
  public string AlertFooter { get; }
  public bool AlertOnIncrement { get; } = true;
  public bool AlertOnDecrement { get; }
  public bool LogOnIncrement { get; } = true;
  public bool LogOnDecrement { get; } = true;
  public bool IsFull => Filled >= Segments;

  public virtual EmbedBuilder ToEmbed()
  {
    var embed = new EmbedBuilder()
      .WithAuthor(EmbedCategory)
      .WithTitle(Title)
      .WithDescription(Description)
    ;
    return IClock.AddClockTemplate(embed, this);
  }
  public EmbedBuilder AlertEmbed()
  {
    return IClock.AlertStub(this);
  }
  public virtual string LogMessage => Filled == Segments ? "The clock fills!" : $"The clock advances to {Filled}/{Segments}";
  public virtual ComponentBuilder MakeComponents()
  {
    return new ComponentBuilder()
    .WithButton(IClock.AdvanceButton().WithDisabled(IsFull))
    .WithButton(IClock.ResetButton().WithDisabled(Filled == 0));
  }
}