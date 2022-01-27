using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace TheOracle2.UserContent;

public interface IOracleGameObjectTemplate
{
}

/* Things to solve/support:
 * Action buttons for oracles
 * Action buttons for trackers?
 * Action rows
 * drop down lists?
 * Fields with oracle names
 */

public class OracleGameObjectTemplate
{
    public string Title { get; set; }
    public string Author { get; set; }
    public List<FieldInfo> Fields { get; set; } = new();
    public List<IGameButton> Buttons { get; set; }

    public async Task CreateMessage(SocketSlashCommand context)
    {
        var compBuilder = new ComponentBuilder();

        foreach (var button in Buttons)
        {
            compBuilder.WithButton(button.AsButtonBuilder());
        }

        EmbedBuilder builder = new EmbedBuilder();
        if (Author != null) builder.WithAuthor(Author);
        if (Title != null) builder.WithTitle(Title);

        builder.WithFields(Fields);

        await context.RespondAsync(embed: builder.Build(), components: compBuilder.Build());
    }
}

public class FieldInfo : EmbedFieldBuilder
{
    private string title;
    private string text;
    private static Regex embededIdRegex = new(@"({\d+})");

    public new string Name { get => title; set => title = value; }
    public new string Value { get => text; set => text = value; }
    public new bool IsInline { get; set; } = true;

    private string ParseText(string input)
    {
        var ids = embededIdRegex.Matches(input)
            .Select(m =>
            {
                int.TryParse(m.Groups[1].Value, out int temp);
                return temp;
            })
            .Where(i => i != 0);

        return "Todo";
    }
}

public interface IGameButton
{
    public ButtonBuilder AsButtonBuilder();
}

public class OracleButton : ButtonBuilder, IGameButton
{
    public new IEmote Emote { get; set; }

    public new ButtonStyle Style { get; set; }

    public new string CustomId { get; set; }

    public new string Label { get; set; }

    public new string Url { get; set; }

    public new bool IsDisabled { get; set; }

    public ButtonBuilder AsButtonBuilder() => this;
}

public class TrackButton : ButtonBuilder, IGameButton
{
    public new IEmote Emote { get; set; }

    public new ButtonStyle Style { get; set; }

    public new string CustomId { get; set; }

    public new string Label { get; set; }

    public new string Url { get; set; }

    public new bool IsDisabled { get; set; }

    public ButtonBuilder AsButtonBuilder() => this;
}