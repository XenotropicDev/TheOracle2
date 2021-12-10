using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle2.Commands;
public class RightClickCommands : InteractionModuleBase
{
    [MessageCommand("Recreate Message")]
    public async Task MoveToBottom(IMessage msg)
    {
        if (msg.Author.Id != Context.Client.CurrentUser.Id) return;

        var builder = ComponentBuilder.FromMessage(msg);
        await RespondAsync(msg.Content, embeds: msg.Embeds.OfType<Embed>().ToArray(), component: builder.Build()).ConfigureAwait(false);

        await msg.DeleteAsync().ConfigureAwait(false);
    }
}
