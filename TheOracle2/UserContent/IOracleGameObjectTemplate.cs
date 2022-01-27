using System.Text.RegularExpressions;
using Discord.WebSocket;
using TheOracle2.ActionRoller;

namespace TheOracle2.UserContent;

public interface IOracleGameObjectTemplate
{
}

/* Things to solve/support:
 * Action buttons for oracles
 * Action buttons for trackers?
 * Action rows
 * drop down lists
 * Fields with oracle names
 */

public class TempNPCContainer //TODO: remove this class
{
    public TempNPCContainer()
    {
        var NPC = new OracleGameObjectTemplate();

        NPC.Title = "oracle:nameId";
        NPC.Author = "oracle:npcTypeId";

        var field1 = new GameObjectFieldInfo();
        field1.Value = "oracle:npcDispositionId";
        field1.LookupMethod = LookupMethod.UseFirst;

        NPC.Fields.Add(field1);
    }
}

public class OracleGameObjectTemplate
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Footer { get; set; }
    public List<GameObjectFieldInfo> Fields { get; set; } = new();
    public List<IGameButton> Buttons { get; set; } = new();

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

public class OracleGameObjectBuilder
{
    public OracleGameObjectBuilder(TableRollerFactory rollerFactory)
    {
        RollFactory = rollerFactory;
    }

    public IDiscordEntity Build()
    {
        var roller = RollFactory.GetRoller()
    }

    public TableRollerFactory RollFactory { get; }
}

public class GameObjectFieldInfo
{
    private static Regex oracleRollerRegex = new(@"((oracle:|subcat:|tables:)?\d+)", RegexOptions.IgnoreCase);

    public GameObjectFieldInfo()
    {
    }

    public string Value { get; set; }
    public LookupMethod LookupMethod { get; set; } = LookupMethod.UseFirst;
    public bool IsInline { get; set; } = true;
    public string Title { get; set; }
}

public enum LookupMethod
{
    UseFirst,
    UseLast,
    UseAll,
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
