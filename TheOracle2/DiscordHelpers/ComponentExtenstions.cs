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

  public static IMessageComponent GetById(this SocketMessageComponent interaction, string customId)
  {
    var components = interaction.Message.Components;
    ActionRowComponent row = components
      .FirstOrDefault(r => r.Components.Any(c => c.CustomId == customId));
    var result = row.Components.FirstOrDefault(c => c.CustomId == customId);
    return result;
  }
  /// <summary>
  /// Gets the source component of an interaction.
  /// </summary>
  public static IMessageComponent GetSrc(this SocketMessageComponent interaction)
  {
    string customId = interaction.Data.CustomId;
    var component = GetById(interaction, customId);
    return component;
  }
  /// <summary>
  /// Gets the originating menu options of an interaction.
  /// </summary>
  public static SelectMenuOption[] GetSrcOptions(this SocketMessageComponent interaction)
  {
    var component = GetSrc(interaction);
    SelectMenuComponent menu = component as SelectMenuComponent;
    var values = interaction.Data.Values as string[];
    var results = menu.Options.Where(option => values.Contains<string>(option.Value));
    return results as SelectMenuOption[];
  }
  /// <summary>
  /// Gets the first of the orginating menu options of an interaction
  /// </summary>
  public static SelectMenuOption GetSrcOption(this SocketMessageComponent interaction)
  {
    var value = interaction.Data.Values.FirstOrDefault();
    return GetSrcOption(interaction, value);
  }
  /// <summary>
  /// Gets the first option of the originating menu with a matching value.
  /// </summary>
  public static SelectMenuOption GetSrcOption(this SocketMessageComponent interaction, string value)
  {
    var component = GetSrc(interaction);
    SelectMenuComponent menu = component as SelectMenuComponent;
    var result = menu.Options.FirstOrDefault(option => value == option.Value);
    return result;
  }
  /// <summary>
  /// Gets the label of the first originating menu option.
  /// </summary>
  public static string GetSrcOptionLabel(this SocketMessageComponent interaction)
  {
    var option = GetSrcOption(interaction);
    return option.Label;
  }
  /// <summary>
  /// Gets the labels of the originating menu options.
  /// </summary>
  public static string[] GetSrcOptionLabels(this SocketMessageComponent interaction)
  {
    var options = GetSrcOptions(interaction);
    return options.Select(option => option.Label) as string[];
  }
  /// <summary>
  /// Gets the emoji of the first originating menu option.
  /// </summary>
  public static IEmote GetSrcOptionEmoji(this SocketMessageComponent interaction)
  {
    var option = GetSrcOption(interaction);
    return option.Emote;
  }
  /// <summary>
  /// Gets the emojis of the originating menu options.
  /// </summary>
  public static IEmote[] GetSrcOptionEmojis(this SocketMessageComponent interaction)
  {
    var options = GetSrcOptions(interaction);
    return options.Select(option => option.Emote) as IEmote[];
  }

  /// <summary>
  /// Gets the description of the first originating menu option.
  /// </summary>
  public static string GetSrcOptionDescription(this SocketMessageComponent interaction)
  {
    var option = GetSrcOption(interaction);
    return option.Description;
  }
  /// <summary>
  /// Gets the emojis of the originating menu options.
  /// </summary>
  public static string[] GetSrcOptionDescriptions(this SocketMessageComponent interaction)
  {
    var options = GetSrcOptions(interaction);
    return options.Select(option => option.Description) as string[];
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