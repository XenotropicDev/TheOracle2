namespace TheOracle2.GameObjects;
public class CampaignClock : Clock
{
  public CampaignClock(Embed embed) : base(embed) { }

  public CampaignClock(ClockSize segments, int filledSegments, string title, string description = "") : base(segments, filledSegments, title, description) { }
  public override string EmbedCategory => "Campaign Clock";
  public override string FillMessage => "The event is triggered or the project is complete. Envision the outcome and the impact on your setting.";

  private static SelectMenuOptionBuilder AdvanceAskOption(int odds, string label, IEmote emoji)
  {
    return new SelectMenuOptionBuilder()
    .WithEmote(emoji)
    .WithLabel(label)
    .WithDescription(odds == 100 ? "Advance the clock without rolling Ask the Oracle." : $"{odds}% chance for the clock to advance.")
    .WithValue(odds == 100 ? "clock-advance" : $"clock-advance-{odds}")
    .WithDefault(false)
    ;
  }
  public SelectMenuBuilder AdvanceSelectMenu()
  {
    SelectMenuBuilder selectMenu = new SelectMenuBuilder()
    ;
    if (!IsFull)
    {
      selectMenu = selectMenu.AddOption(AdvanceAskOption(100, IClock.AdvanceLabel, IClock.OddsEmoji[100]));
      foreach (AskOption odds in Enum.GetValues(typeof(AskOption)))
      {
        string label = $"{IClock.AdvanceLabel} ({OracleAnswer.OddsString[(int)odds]})";
        SelectMenuOptionBuilder menuOption = AdvanceAskOption((int)odds, label, IClock.OddsEmoji[(int)odds]);
        selectMenu = selectMenu.AddOption(menuOption);
      }
    }
    return selectMenu.AddOption(AdvanceOption())
    .WithPlaceholder("Advance clock...")
    .WithCustomId("clock-menu")
    .WithMinValues(0)
    .WithMaxValues(1);
  }
  public override ComponentBuilder MakeComponents()
  {
    return new ComponentBuilder().WithSelectMenu(AdvanceSelectMenu());
  }
}