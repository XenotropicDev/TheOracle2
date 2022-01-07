namespace TheOracle2.GameObjects;
using System.ComponentModel.DataAnnotations;

public class CampaignClock : Clock
{
  public CampaignClock(Embed embed) : base(embed) { }

  public CampaignClock(EmbedField embedField) : base(embedField) { }
  public CampaignClock(ClockSize segments, int filledSegments, string text) : base(segments, filledSegments, text) { }

  public override string ClockType { get => "Campaign Clock"; }

  private SelectMenuOptionBuilder AdvanceSelectMenuOption(int odds, string label, IEmote emoji)
  {
    return new SelectMenuOptionBuilder()
    .WithEmote(emoji)
    .WithLabel(label)
    .WithDescription(odds < 100 ? $"{odds}% chance for the clock to advance." : "Advance the clock without rolling Ask the Oracle.")
    .WithValue(odds < 100 ? $"advance-{odds}" : "advance")
    ;
  }
  public SelectMenuBuilder AdvanceSelectMenu()
  {
    // option labels should be Advance (likely), etc

    SelectMenuBuilder selectMenu = new SelectMenuBuilder()
    .WithPlaceholder("Advance clock...")
    .WithCustomId("advance-clock-menu")
    // .WithMinValues(1)
    // .WithMaxValues(1)
    ;
    if (!IsFull)
    {
      selectMenu.AddOption(AdvanceSelectMenuOption(100, CampaignClock.AdvanceLabel, OddsEmoji[100]));
      foreach (AskOption odds in Enum.GetValues(typeof(AskOption)))
      {
        // todo: make the strings work i guess!
        string label = $"{CampaignClock.AdvanceLabel} ({OracleAnswer.OddsString[(int)odds]})";
        SelectMenuOptionBuilder menuOption = AdvanceSelectMenuOption((int)odds, label, OddsEmoji[(int)odds]);
        selectMenu.AddOption(menuOption);
      }
    }

    selectMenu.AddOption(
      new SelectMenuOptionBuilder()
      .WithLabel("Reset clock")
      .WithValue("reset")
      .WithEmote(UxEmoji["reset"])
      )
;
    return selectMenu;
  }
  public override ComponentBuilder MakeComponents()
  {
    return new ComponentBuilder().WithSelectMenu(AdvanceSelectMenu());
  }
}