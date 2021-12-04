namespace TheOracle2.DiscordHelpers;

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
}
