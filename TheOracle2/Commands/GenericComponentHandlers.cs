using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace TheOracle2.Commands;

public class GenericComponentHandlers : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly ILogger<GenericComponentHandlers> logger;

    public GenericComponentHandlers(ILogger<GenericComponentHandlers> logger)
    {
        this.logger = logger;
    }

    [ComponentInteraction("delete-original-response")]
    public async Task DeleteOriginalAction()
    {
        await DeferAsync();
        await Context.Interaction.Message.DeleteAsync();
    }

    public static ButtonBuilder CancelButton(string label = null)
    {
        return new ButtonBuilder(label ?? "Cancel", "delete-original-response", style: ButtonStyle.Secondary);
    }
}