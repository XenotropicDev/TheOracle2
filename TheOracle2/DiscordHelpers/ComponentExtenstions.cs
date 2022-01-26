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

public static class InteractionExtensions
{
    /// <summary>
    /// Gets the component with the matching ID, if any.
    /// </summary>
    /// <param name="interaction">The interaction to get the component out of</param>
    /// <param name="customId">The ID to search for</param>
    /// <returns></returns>
    public static IMessageComponent GetComponentById(this SocketMessageComponent interaction, string customId)
    {
        var components = interaction.Message.Components;
        ActionRowComponent row = components
            .FirstOrDefault(r => r.Components.Any(c => c.CustomId == customId));
        var result = row?.Components.FirstOrDefault(c => c.CustomId == customId);
        return result;
    }

    /// <summary>
    /// Gets the triggering component of an interaction.
    /// </summary>
    public static IMessageComponent GetTriggeringComponent(this SocketMessageComponent interaction)
    {
        string customId = interaction.Data.CustomId;
        var component = GetComponentById(interaction, customId);
        return component;
    }

    /// <summary>
    /// Gets the originating menu options of an interaction.
    /// </summary>
    /// <param name="interaction">The interaction to get the select menus out of</param>
    /// <returns></returns>
    public static SelectMenuOption[] GetTriggeringSelectMenuItems(this SocketMessageComponent interaction)
    {
        var component = GetTriggeringComponent(interaction);
        SelectMenuComponent menu = component as SelectMenuComponent;
        var values = interaction.Data.Values;
        var results = menu?.Options.OfType<SelectMenuOption>().Where(option => values.Contains(option.Value));
        return results.ToArray();
    }

    /// <summary>
    /// Gets the first of the originating menu options of an interaction
    /// </summary>
    public static SelectMenuOption GetFirstSelectMenu(this SocketMessageComponent interaction)
    {
        var value = interaction.Data.Values.FirstOrDefault();
        return GetSelectItemWithId(interaction, value);
    }

    /// <summary>
    /// Gets the first select menu option out of an with a matching value.
    /// </summary>
    public static SelectMenuOption GetSelectItemWithId(this SocketMessageComponent interaction, string value)
    {
        if (value == null) return null;

        var component = GetTriggeringComponent(interaction);
        SelectMenuComponent menu = component as SelectMenuComponent;
        var result = menu?.Options.FirstOrDefault(option => value == option.Value);
        return result;
    }

    // Todo, I'm not sure if this adds enough to justify the extension method.
    /// <summary>
    /// Gets the label of the first originating menu option.
    /// </summary>
    public static string GetFirstSelectMenuLabel(this SocketMessageComponent interaction)
    {
        var option = GetFirstSelectMenu(interaction);
        return option?.Label;
    }

    /// <summary>
    /// Gets the labels of the originating menu options.
    /// </summary>
    public static string[] GetSrcOptionLabels(this SocketMessageComponent interaction)
    {
        var options = GetTriggeringSelectMenuItems(interaction);
        return options?.Select(option => option.Label).ToArray();
    }

    /// <summary>
    /// Gets the emoji of the first originating menu option.
    /// </summary>
    public static IEmote GetFirstSelectEmote(this SocketMessageComponent interaction)
    {
        var option = GetFirstSelectMenu(interaction);
        return option?.Emote;
    }

    /// <summary>
    /// Gets the emojis of the originating menu options.
    /// </summary>
    public static IEmote[] GetTriggeringEmote(this SocketMessageComponent interaction)
    {
        var options = GetTriggeringSelectMenuItems(interaction);
        return options?.Select(option => option.Emote).ToArray();
    }

    // Todo, I'm not sure if this adds enough to justify the extension method.
    /// <summary>
    /// Gets the description of the first originating menu option.
    /// </summary>
    public static string GetSrcOptionDescription(this SocketMessageComponent interaction)
    {
        var option = GetFirstSelectMenu(interaction);
        return option.Description;
    }

    /// <summary>
    /// Gets the emojis of the originating menu options.
    /// </summary>
    public static string[] GetSrcOptionDescriptions(this SocketMessageComponent interaction)
    {
        var options = GetTriggeringSelectMenuItems(interaction);
        return options.Select(option => option.Description).ToArray();
    }
}

public static class ComponentExtenstions
{
    public static bool ContainsCustomId(this ActionRowComponent components, string id)
    {
        if (components.Components.Count() == 0) { return false; }
        if (components.Components.Any(item => item.CustomId == id)) { return true; }
        return false;
    }

    public static bool ContainsCustomId(this IEnumerable<ActionRowComponent> components, string id)
    {
        if (components.Count() == 0) { return false; }
        if (components.Any(row => row.ContainsCustomId(id))) { return true; }
        return false;
    }

    public static ActionRowBuilder GetRowContainingId(this ComponentBuilder builder, string id)
    {
        ActionRowBuilder row = builder.ActionRows.Find(r => r.Components.Any(c => c.CustomId == id));
        return row;
    }

    public static ActionRowComponent GetRowContainingId(this IEnumerable<ActionRowComponent> components, string id)
    {
        ActionRowComponent row = components.FirstOrDefault(row => row.Components.Any(c => c.CustomId == id));
        return row;
    }

    public static IMessageComponent GetComponentById(this IEnumerable<ActionRowComponent> components, string id)
    {
        ActionRowComponent row = GetRowContainingId(components, id);
        IMessageComponent component = row.Components.FirstOrDefault(item => item.CustomId == id);
        return component;
    }

    public static IMessageComponent GetComponentById(this ComponentBuilder builder, string id)
    {
        ActionRowBuilder row = GetRowContainingId(builder, id);
        IMessageComponent component = row.Components.Find(c => c.CustomId == id);
        return component;
    }

    public static ComponentBuilder RemoveComponentById(this ComponentBuilder builder, string id)
    {
        var row = GetRowContainingId(builder, id);
        var item = GetComponentById(builder, id);
        if (item != null)
        {
            row.Components.Remove(item);
            if (row.Components.Count == 0)
            {
                builder.ActionRows.Remove(row);
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
