using TheOracle2.DataClasses;

namespace TheOracle2;

public class DiscordOracleItems
{
    public EmbedBuilder EmbedBuilder { get; set; }
    public ComponentBuilder ComponentBuilder { get; set; }
}

public class DiscordOracleBuilder
{
    public DiscordOracleBuilder(OracleRollerResult root)
    {
        Root = root;
    }

    //Todo: this is the only real non-static member, do we want to make it a method parameter instead?
    private SelectMenuBuilder AddOracleSelect = new SelectMenuBuilder().WithPlaceholder("Add Oracle Roll").WithCustomId("add-oracle-select");

    private OracleRollerResult Root { get; }

    public DiscordOracleItems Build()
    {
        return new DiscordOracleItems { EmbedBuilder = GetEmbedBuilder(Root), ComponentBuilder = GetComponentBuilder(Root) };
    }

    private static EmbedBuilder GetEmbedBuilder(OracleRollerResult result)
    {
        var builder = new EmbedBuilder();

        builder.WithAuthor(result.Category)
            .WithTitle("Oracle Result");

        AddFieldsToBuilder(result, builder);

        return builder;
    }

    public static EmbedBuilder AddFieldsToBuilder(OracleRollerResult node, EmbedBuilder builder)
    {
        builder.AddField($"{node.Name} [{node.Roll}]", node.Result.Description, true);

        if (node.Result.Image != null) builder.WithThumbnailUrl(node.Result.Image);

        foreach (var child in node.ChildResults)
        {
            AddFieldsToBuilder(child, builder);
        }

        return builder;
    }

    private ComponentBuilder GetComponentBuilder(OracleRollerResult root)
    {
        var builder = new ComponentBuilder();

        AddComponents(builder, root, root);

        if (AddOracleSelect.Options.Count > 0)
        {
            builder.WithSelectMenu(AddOracleSelect);
        }

        if (!builder.ActionRows?.Any(ar => ar.Components.Count > 0) ?? false) return null;

        return builder;
    }

    private ComponentBuilder AddComponents(ComponentBuilder builder, OracleRollerResult node, OracleRollerResult root = null)
    {
        foreach (var item in node.FollowUpTables)
        {
            if ((item.Tables?.Count ?? 0) == 0 && item.Table != null) 
                AddOracleSelect.AddOption(item.Name, $"oracle:{item.Id}");
            
            if (item.Tables != null)
            {
                foreach (var table in item.Tables)
                {
                    AddOracleSelect.AddOption($"{item.Name} > {table.Name}" , $"tables:{table.Id}");
                }
            }
        }

        if (root != null && node.Result.Oracle?.UseWith != null)
        {
            foreach (var useWith in node.Result.Oracle.UseWith)
            {
                if (IsInResultSet(root, useWith.Oracle)) continue;

                AddOracleSelect.AddOption(useWith.Name, useWith.Oracle.Id.ToString(), emote: new Emoji("🧦"));
            }
        }

        foreach (var child in node.ChildResults)
        {
            AddComponents(builder, child, root);
        }

        return builder;
    }

    private bool IsInResultSet(OracleRollerResult result, Oracle oracle)
    {
        if (result.Result.Oracle == oracle) return true;

        foreach (var child in result.ChildResults)
        {
            if (IsInResultSet(child, oracle)) return true;
        }
        return false;
    }
}