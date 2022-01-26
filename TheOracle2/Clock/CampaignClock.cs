namespace TheOracle2.GameObjects;

public class CampaignClock : Clock
{
    public CampaignClock(Embed embed, bool alerts = false) : base(embed, alerts) { }
    public CampaignClock(ClockSize segments, int filledSegments, string title, string description = "", bool alerts = false) : base(segments, filledSegments, title, description, alerts) { }
    public override string EmbedCategory => "Campaign Clock";
    public override string ClockFillMessage => "The event is triggered or the project is complete. Envision the outcome and the impact on your setting.";
    public override ComponentBuilder MakeComponents()
    {
        SelectMenuBuilder menu = new SelectMenuBuilder()
            .WithCustomId($"clock-menu")
            .WithMinValues(0);
        if (!IsFull)
        {
            menu.AddOption(IClock.AdvanceOption());
            foreach (AskOption odds in Enum.GetValues(typeof(AskOption)))
            {
                menu.AddOption(IClock.AdvanceAskOption(odds));
            }
        }
        if (Filled != 0)
        {
            menu.AddOption(IClock.ResetOption());
        }
        return new ComponentBuilder()
            .WithSelectMenu(menu)
            // .WithButton(ILogWidget.ToggleAlertButton(AlertOnIncrement))
            ;
    }
}
