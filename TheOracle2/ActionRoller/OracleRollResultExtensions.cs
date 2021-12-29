using TheOracle2.DataClasses;

namespace TheOracle2;

public static class OracleResultDiscordBuilders
{
    public static EmbedBuilder GetEmbedBuilder(this OracleRollerResult result)
    {
        var builder = new EmbedBuilder();

        builder.WithAuthor(result.Result.Oracle.Category)
            .WithTitle("Oracle Result");

        AddFieldsToBuilder(result, builder);

        return builder;
    }

    public static EmbedBuilder AddFieldsToBuilder(this OracleRollerResult node, EmbedBuilder builder)
    {
        builder.AddField(node.Result.Oracle.Name, node.Result.Description, true);

        if (node.Result.Thumbnail != null) builder.WithThumbnailUrl(node.Result.Thumbnail);

        foreach (var child in node.ChildResults)
        {
            AddFieldsToBuilder(child, builder);
        }

        return builder;
    }

    public static ComponentBuilder GetComponentBuilder(this OracleRollerResult root)
    {
        var builder = new ComponentBuilder();

        AddComponents(builder, root, root);

        if (!builder.ActionRows.Any(ar => ar.Components.Count > 0)) return null;

        return builder;
    }

    public static bool IsInResultSet(OracleRollerResult result, Oracle oracle)
    {
        if (result.Result.Oracle == oracle) return true;

        foreach (var child in result.ChildResults)
        {
            if (IsInResultSet(child, oracle)) return true;
        }
        return false;
    }

    public static ComponentBuilder AddComponents(ComponentBuilder builder, OracleRollerResult node, OracleRollerResult root = null)
    {
        foreach (var item in node.FollowUpTables)
        {
            builder.WithButton(item.Name, $"oracle-followup:{item.Id}");
        }

        if (root != null && node.Result.Oracle.UseWith != null)
        {
            foreach (var useWith in node.Result.Oracle.UseWith)
            {
                if (IsInResultSet(root, useWith.Oracle)) continue;

                builder.WithButton(useWith.Name, $"oracle-followup:{useWith.Oracle.Id}", emote: new Emoji("🧦"));
            }
        }

        foreach (var child in node.ChildResults)
        {
            AddComponents(builder, child, root);
        }

        return builder;
    }
}