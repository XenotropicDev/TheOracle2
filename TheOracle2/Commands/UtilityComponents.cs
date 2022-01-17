using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.GameObjects;
namespace TheOracle2;

/// <summary>
/// General-purpose components for editing embeds and other tasks.
/// </summary>
public class UtilityComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
  [ComponentInteraction("ephemeral-reveal")]
  public async Task RevealEphemeral()
  {
    SocketMessageComponent interaction = Context.Interaction as SocketMessageComponent;
    SocketUserMessage message = interaction.Message;
    ComponentBuilder components = ComponentBuilder.FromComponents(message.Components);
    components.RemoveComponentById("ephemeral-reveal");
    await RespondAsync(ephemeral: false, embeds: message.Embeds as Embed[], components: components.Build());
  }
}