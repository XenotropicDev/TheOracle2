using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace TheOracle2.Commands;

public class GenericComponentHandlers : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>> {
    private readonly ILogger<GenericComponentHandlers> logger;

    public GenericComponentHandlers(ILogger<GenericComponentHandlers> logger) {
        this.logger = logger;
    }

    [ComponentInteraction("delete-original-response")]
    public async Task DeleteOriginalAction() {
        await DeferAsync();
        await Context.Interaction.Message.DeleteAsync();
    }

    public static ButtonBuilder CancelButton(string label = null) {
        return new ButtonBuilder(label ?? "Cancel", "delete-original-response", style: ButtonStyle.Secondary);
    }
    [ComponentInteraction("alerts-toggle:*")]
    public async Task ToggleAlerts(string togglesToString) {
        if (!bool.TryParse(togglesToString, out bool alerts)) { throw new Exception($"Unable to parse {togglesToString} to boolean"); }
        bool newBtnTarget = !alerts;
        ButtonBuilder newBtn = ToggleAlertsButton(newBtnTarget);
        try {
            await Context.Interaction.UpdateAsync(msg => {
                ComponentBuilder components = ComponentBuilder.FromMessage(Context.Interaction.Message);
                _ = components.ReplaceComponentById($"alerts-toggle:{togglesToString}", newBtn.Build());
                msg.Components = components.Build();
            }).ConfigureAwait(false);
        }
        catch (HttpException hex) {
            string json = JsonConvert.SerializeObject(hex.Errors, Formatting.Indented);
            Console.WriteLine(json);
            throw;
        }
    }
    public static ButtonBuilder ToggleAlertsButton(bool togglesTo) {
        string onOff = togglesTo ? "ON" : "OFF";
        Emoji emoji = togglesTo ? new Emoji("✔️") : new Emoji("❌");
        return new ButtonBuilder()
            .WithLabel($"Alerts: {onOff}")
            .WithCustomId($"alerts-toggle:{togglesTo}")
            .WithEmote(emoji)
            .WithStyle(ButtonStyle.Secondary);
    }
}