namespace TheOracle2.GameObjects;

/// <summary>
/// Interface for game widgets that can increment/decrement and might want to report such events to a log or embed.
/// </summary>
public interface ILogWidget : IWidget
{
    public bool AlertOnIncrement { get; }
    public bool AlertOnDecrement { get; }
    public bool LogOnIncrement { get; }
    public bool LogOnDecrement { get; }

    /// <summary>
    /// A message string. Sent to the log, and used as the alert embed title.
    /// </summary>
    public string LogMessage { get; }

    public string AlertFooter { get; }

    public EmbedBuilder AlertEmbed();

    public static bool ParseAlertStatus(ButtonComponent button)
    {
        return button.CustomId switch
        {
            "alert-toggle:True" => true,
            "alert-toggle:False" => false,
            _ => throw new Exception($"Unable to parse alert status from customId: {button.CustomId}")
        };
    }

    public static bool ParseAlertStatus(IEnumerable<ActionRowComponent> actionRows)
    {
        return actionRows.ContainsCustomId("alert-toggle:True");
    }

    public static ButtonBuilder ToggleAlertButton(bool toggleState = false)
    {
        string onOff = toggleState ? "ON" : "OFF";
        Emoji emoji = toggleState ? new Emoji("✔️") : new Emoji("❌");
        return new ButtonBuilder()
            .WithLabel($"Alerts: {onOff}")
            .WithCustomId($"alert-toggle:{toggleState}")
            .WithEmote(emoji)
            .WithStyle(ButtonStyle.Secondary);
    }

    /// <summary>
    /// Template/stub for use in building alert embeds.
    /// </summary>
    public static EmbedBuilder AlertStub(ILogWidget widget)
    {
        EmbedBuilder embed = new EmbedBuilder()
            .WithAuthor($"{widget.EmbedCategory}: {widget.Title}")
            .WithTitle(widget.LogMessage)
            ;
        if (!string.IsNullOrEmpty(widget.AlertFooter))
        {
            embed.WithFooter(widget.AlertFooter);
        }
        return embed;
    }
}
