using Discord.Interactions;
using Discord.WebSocket;

namespace TheOracle2.Commands;

public class SelectSupportModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("test-select", "select menu tests")]
    public async Task UpdateMessage()
    {
        await RespondAsync("Test of Select menus", components: new ComponentBuilder()
            .WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId("select-1")
                .WithMaxValues(2)
                .AddOption("select 1 value 1", "s1-value-1")
                .AddOption("select 1 value 2", "s1-value-2"))
            .WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId("select-2")
                .WithMaxValues(2)
                .AddOption("select 2 value 1", "s2-value-1")
                .AddOption("select 2 value 2", "s2-value-2"))
            .Build());
    }
}

public class SelectCommands : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("select-1")]
    public async Task UpdateMessage(string[] values)
    {
        await Context.Interaction
            .UpdateAsync(msg =>
            {
                msg.Content = $"Selected values {string.Join(", ", values)}";
                var comp = ComponentBuilder.FromMessage(Context.Interaction.Message);
                foreach (var row in comp.ActionRows.Where(r => r.Components.All(c => c.Type == ComponentType.SelectMenu)))
                {
                    var selectBuilder = (row.Components.FirstOrDefault() as SelectMenuComponent)?.ToBuilder();
                    if (selectBuilder != null && values.Contains(selectBuilder.CustomId))
                    {
                        row.WithComponents(new List<IMessageComponent> { selectBuilder.Build() });
                    }
                }
                msg.Components = comp.Build();
            });
    }

    [ComponentInteraction("select-2")]
    public async Task RespondToInteraction(string[] values)
    {
        await Context.Interaction.RespondAsync($"Selected values {string.Join(", ", values)}");
    }
}