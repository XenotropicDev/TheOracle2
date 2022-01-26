namespace TheOracle2.GameObjects;

/// <summary>
/// Interface for game object classes that can be rendered as "widgets" (interactive embeds with components).
/// </summary>
public interface IWidget
{
    /// <summary>
    /// Used as the embed's Title. If the embed doesn't utilize this, it should be set by the user with a required string parameter, "title".
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Used as the embed's Author text, and to identify the class that an embed should be parsed into with a widget interface's FromEmbed() static method. It should not be accessible to the user.
    /// </summary>
    public string EmbedCategory { get; }

    /// <summary>
    /// Used as the embed's Description text. If the embed isn't a static display (e.g. an Asset or Encounter), it should generally be set by an optional string parameter, "description".
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Footer text passed to the Embed. Generally used to display conditional text or source information.
    /// </summary>
    public string Footer { get; }

    /// <summary>
    /// Generate this widget's embed to display alongside the embed from the MakeComponents() method.
    /// </summary>
    public EmbedBuilder ToEmbed();

    /// <summary>
    /// Generate this widget's components to display alongside the embed from the ToEmbed() method.
    /// </summary>
    public ComponentBuilder MakeComponents();

    /// <summary>
    /// Utility that generates an Embed stub for use in constructors to make it easy to format them consistently.
    /// </summary>
    protected static EmbedBuilder EmbedStub(IWidget widget)
    {
        EmbedBuilder embed = new EmbedBuilder()
          .WithAuthor(widget.EmbedCategory)
          .WithTitle(widget.Title);

        if (!string.IsNullOrEmpty(widget.Description))
        { embed.WithDescription(widget.Description); }

        if (!string.IsNullOrEmpty(widget.Footer))
        { embed.WithFooter(widget.Footer); }
        return embed;
    }
}
