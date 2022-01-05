using TheOracle2;
namespace TheOracle2.GameObjects;

public class CampaignClock : Clock
{
  public override string ClockType { get => "Campaign Clock"; }
  public ButtonBuilder AdvanceButton(AskOption chance)
  {
    return base.AdvanceButton()
    .WithLabel(
      $"Advance Clock ({chance.GetType().GetField(chance.ToString())})"
      ).WithCustomId($"advance-clock-{(int)chance}")
    ;
  }
  public ButtonBuilder[] AdvanceButtons()
  {
    List<ButtonBuilder> buttons = new();
    foreach (AskOption askOption in Enum.GetValues(typeof(AskOption)))
    {
      buttons.Add(AdvanceButton(askOption));
    }
    return buttons.ToArray();
  }
}