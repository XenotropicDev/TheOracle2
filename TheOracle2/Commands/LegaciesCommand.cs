// using Discord.Interactions;
// using Discord.WebSocket;
// using TheOracle2.GameObjects;

// namespace TheOracle2;

// public class LegaciesCommand : InteractionModuleBase
// {
//   [SlashCommand("legacies", "Displays a dummy Legacies embed for development purposes.")]
//   public async Task BuildLegacies(
//     [MinValue(0)] int questTicks,
//     [MinValue(0)] int bondTicks,
//     [MinValue(0)] int discoveryTicks
//   )
//   {
//     Legacies legacies = new(questTicks, bondTicks, discoveryTicks);
//     await RespondAsync(
//       embed: legacies.ToEmbed().Build(),
//       components: legacies.MakeComponents().Build()
//     );
//   }
//   [ComponentInteraction("legacies-menu")]
//   public async Task LegaciesMenu(string[] values)
//   {
//     string operation = values.FirstOrDefault();
//     var interaction = Context.Interaction as SocketMessageComponent;
//     var legacies = new Legacies(interaction.Message.Embeds.FirstOrDefault());
//     var operationParams = operation.Split(":")[1].Split(",");
//     if (operation.StartsWith("legacy-mark:"))
//     {
//       Legacy legacy = Enum.Parse<Legacy>(operationParams[0]);
//       int ticks = int.Parse(operationParams[1]);
//       legacies.Mark(legacy, ticks);
//       await interaction.UpdateAsync(msg =>
//       {
//         msg.Embed = legacies.ToEmbed().Build();
//         msg.Components = legacies.MakeComponents().Build();
//       });
//     }
//     if (operation.StartsWith("legacy-clear:"))
//     {
//       Legacy legacy = Enum.Parse<Legacy>(operationParams[0]);
//       int ticks = int.Parse(operationParams[1]);
//       legacies.Mark(legacy, ticks * -1);
//       await interaction.UpdateAsync(msg =>
//       {
//         msg.Embed = legacies.ToEmbed().Build();
//         msg.Components = legacies.MakeComponents().Build();
//       });
//     }
//   }
// }