namespace TheOracle2.GameObjects;
public class CampaignClock : Clock
{
  public CampaignClock(Embed embed) : base(embed) { }

  public CampaignClock(ClockSize segments, int filledSegments, string title, string description = "") : base(segments, filledSegments, title, description) { }
  public override string EmbedCategory => "Campaign Clock";
  public override string LogMessage => "The event is triggered or the project is complete. Envision the outcome and the impact on your setting.";

  public SelectMenuBuilder MakeSelectMenu()
  {
    SelectMenuBuilder selectMenu = new SelectMenuBuilder()
      .WithPlaceholder("Advance clock...")
      .WithCustomId($"clock-menu:{Filled}/{Segments}")
      .WithMinValues(0)
      .WithMaxValues(1)
      .AddOption("test", "test");
    return selectMenu;
  }
  public override ComponentBuilder MakeComponents()
  {
    SelectMenuBuilder menu = new SelectMenuBuilder()
        .WithCustomId($"clock-menu:{Filled}/{Segments}")
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
    return new ComponentBuilder().WithSelectMenu(menu);
  }
}