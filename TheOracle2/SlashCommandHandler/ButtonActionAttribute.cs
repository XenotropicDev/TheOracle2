namespace TheOracle2;

internal class ButtonActionAttribute : Attribute
{
    public string ButtonId { get; set; }

    public ButtonActionAttribute(string buttonId)
    {
        this.ButtonId = buttonId;
    }
}