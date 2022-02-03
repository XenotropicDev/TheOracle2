namespace TheOracle2.GameObjects;

public abstract class Clock : IClock
{
    protected Clock(Embed embed, bool alerts = false)
    {
        var values = IClock.ParseClock(embed);
        Title = embed.Title;
        Description = embed.Description;
        Filled = values.Item1;
        Segments = values.Item2;
        AlertOnIncrement = alerts;
    }

    protected Clock(ClockSize segments = (ClockSize)6, int filledSegments = 0, string title = "", string description = "", bool alerts = false)
    {
        if (filledSegments < 0 || filledSegments > ((int)segments))
        {
            throw new ArgumentOutOfRangeException(nameof(filledSegments), "filledSegments can't exceed segments");
        }
        Title = title;
        Segments = (int)segments;
        Filled = filledSegments;
        Description = description;
        AlertOnIncrement = alerts;
    }

    public int Segments { get; }
    public int Filled { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Footer { get; set; }
    public abstract string EmbedCategory { get; }
    public string AlertFooter { get; }
    public bool AlertOnIncrement { get; }
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

    public virtual EmbedBuilder AlertEmbed()
    {
        var embed = IClock.AlertStub(this);
        if (IsFull)
        { embed.WithDescription(ClockFillMessage); }
        return embed;
    }

    public virtual string LogMessage => IsFull ? "The clock fills!" : "The clock advances!";
    public abstract string ClockFillMessage { get; }

    public virtual ComponentBuilder MakeComponents()
    {
        return new ComponentBuilder()
        .WithButton(IClock.AdvanceButton().WithDisabled(IsFull))
        .WithButton(IClock.ResetButton().WithDisabled(Filled == 0))
        /// alert button
        // .WithButton(ILogWidget.ToggleAlertButton(AlertOnIncrement))
        ;
    }
}
