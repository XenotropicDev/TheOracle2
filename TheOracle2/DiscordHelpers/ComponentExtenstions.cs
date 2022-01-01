using Discord.WebSocket;

namespace TheOracle2;

public static class ContextExtensions
{
    public static async Task DeleteOriginalResponseAsync(this SocketSlashCommand ctx)
    {
        var msg = await ctx.GetOriginalResponseAsync().ConfigureAwait(false);
        await msg.DeleteAsync().ConfigureAwait(false);
    }
}

public static class EmbedExtensions
{
    /// <summary>
    /// Changes a numeric field on an embed
    /// </summary>
    /// <param name="embed">the embed builder to change</param>
    /// <param name="fieldName">the name of the field to look for</param>
    /// <param name="change">the amount to change by (use negative numbers to decrease)</param>
    /// <param name="min">the lowest the value can be</param>
    /// <param name="max">the highest the value can be</param>
    /// <returns>The embed builder provided</returns>
    public static EmbedBuilder ChangeNumericField(this EmbedBuilder embed, string fieldName, int change, int min, int max)
    {
        int index = embed.Fields.FindIndex(f => f.Name == fieldName);
        if (int.TryParse(embed.Fields[index].Value.ToString(), out var value))
        {
            value += change;
            if (value < min) value = min;
            if (value > max) value = max;
            embed.Fields[index].Value = value;
        }
        return embed;
    }
}

public static class ComponentExtenstions
{
    public static ComponentBuilder RemoveComponentById(this ComponentBuilder builder, string id)
    {
        var row = builder.ActionRows.FindIndex(r => r.Components.Any(c => c.CustomId == id));
        var item = builder.ActionRows[row].Components.Find(c => c.CustomId == id);
        if (item != null)
        {
            builder.ActionRows[row].Components.Remove(item);
            if (builder.ActionRows[row].Components.Count == 0)
            {
                builder.ActionRows.Remove(builder.ActionRows[row]);
            }
        }

        return builder;
    }

    public static ComponentBuilder ReplaceComponentById(this ComponentBuilder builder, string id, IMessageComponent replacement)
    {
        var rows = builder.ActionRows.Where(r => r.Components.Any(c => c.CustomId == id));
        foreach (var row in rows)
        {
            int index = row.Components.FindIndex(c => c.CustomId == id);
            if (index != -1) row.Components[index] = replacement;
        }

        return builder;
    }

    public static ComponentBuilder TryAdd(this ComponentBuilder builder, IMessageComponent component, int row)
    {
        if (builder.ActionRows.Any(r => r.Components.Any(c => c.CustomId == component.CustomId))) return builder;

        if (builder.ActionRows.Count - 1 >= row)
        {
            builder.ActionRows[row].Components.Add(component);
            return builder;
        }

        var currentRow = new ActionRowBuilder().AddComponent(component);

        builder.ActionRows.Add(currentRow);

        return builder;
    }

    public static ComponentBuilder MarkSelectionByOptionId(this ComponentBuilder component, string optionValue)
    {
        foreach (var row in component.ActionRows)
        {
            var selectBuilder = (row.Components.FirstOrDefault() as SelectMenuComponent)?.ToBuilder();
            if (selectBuilder != null)
            {
                var option = selectBuilder.Options.FirstOrDefault(o => o.Value == optionValue);
                if (option != null)
                {
                    option.WithDefault(true);
                    row.WithComponents(new List<IMessageComponent> { selectBuilder.Build() });
                }
            }
        }

        return component;
    }
}