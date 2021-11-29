using Discord.Interactions;
using Discord.WebSocket;
using System.ComponentModel;
using TheOracle2.UserContent;

namespace TheOracle2
{
    public class PlayerCardCommand : InteractionModuleBase
    {
        public PlayerCardCommand(UserContent.EFContext dbContext)
        {
            DbContext = dbContext;
        }

        //public enum StatValue
        //{
        //    [Description("1")]
        //    One = 1,
        //    [Description("2")]
        //    Two = 2,
        //    [Description("3")]
        //    Three = 3,
        //    [Description("4")]
        //    Four = 4,
        //    [Description("5")]
        //    Five = 5,
        //}

        public EFContext DbContext { get; }

        [SlashCommand("player", "Generates a post to keep track of a player character's stats")]
        public async Task BuildAsset(string name, [MaxValue(4)][MinValue(1)] int edge, [MaxValue(4)][MinValue(1)] int heart, [MaxValue(4)][MinValue(1)] int iron, [MaxValue(4)][MinValue(1)] int shadow, [MaxValue(4)][MinValue(1)] int wits)
        {
            var compBuilder = new ComponentBuilder()
                .WithButton("+ Momentum", "add-momenutum", row: 0, style: ButtonStyle.Success)
                .WithButton("- Momentum", "lose-momenutum", row: 0, style: ButtonStyle.Secondary)
                .WithButton("+ Supply", "add-supply", row: 0, style: ButtonStyle.Success)
                .WithButton("- Supply", "lose-supply", row: 0, style: ButtonStyle.Secondary)
                .WithButton("Burn", "burn-momentum", row: 0, style: ButtonStyle.Danger)
                .WithButton("+ Health", "add-health", row: 1, style: ButtonStyle.Success)
                .WithButton("- Health", "lose-health", row: 1, style: ButtonStyle.Secondary)
                .WithButton("+ Spirit", "add-spirit", row: 1, style: ButtonStyle.Success)
                .WithButton("- Spirit", "lose-spirit", row: 1, style: ButtonStyle.Secondary)
                ;

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithAuthor($"Player Card");
            builder.WithTitle(name);
            builder.AddField("Stats", $"Edge: {edge}, Heart: {heart}, Iron: {iron}, Shadow: {shadow}, Wits: {wits}");
            builder.AddField("Health", 5);
            builder.AddField("Spirit", 5);
            builder.AddField("Supply", 5);
            builder.AddField("Momentum", 2);
            builder.AddField("XP", 0);

            await RespondAsync(embed: builder.Build(), component: compBuilder.Build());
        }

        public IList<SlashCommandBuilder> GetCommandBuilders()
        {
            var command = new SlashCommandBuilder()
                .WithName("player")
                .WithDescription("Generates a post to keep track of a player character's stats")

            .AddOption(new SlashCommandOptionBuilder()
                        .WithName("name")
                        .WithDescription("Sets the player's name")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.String))
            .AddOption(new SlashCommandOptionBuilder()
                        .WithName("edge")
                        .WithDescription("Sets the player's Edge")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Integer))
            .AddOption(new SlashCommandOptionBuilder()
                        .WithName("heart")
                        .WithDescription("Sets the player's Heart")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Integer))
            .AddOption(new SlashCommandOptionBuilder()
                        .WithName("iron")
                        .WithDescription("Sets the player's Iron")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Integer))
            .AddOption(new SlashCommandOptionBuilder()
                        .WithName("shadow")
                        .WithDescription("Sets the player's Shadow")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Integer))
            .AddOption(new SlashCommandOptionBuilder()
                        .WithName("wits")
                        .WithDescription("Sets the player's Wits")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Integer))
            ;
            return new List<SlashCommandBuilder>() { command };
        }

        [ButtonAction("add-momentum")]
        public async Task AddMomentum(SocketMessageComponent component)
        {
            await component.ModifyOriginalResponseAsync(msg => {
                var embed = msg.Embeds.Value.FirstOrDefault();
                var momentum = embed.Fields.FirstOrDefault(f => f.Name == "Momentum");
                if (int.TryParse(momentum.Value, out var value))
                {
                    value++;
                    if (value > 10) value = 10;
                    momentum = new EmbedFieldBuilder().WithName(momentum.Name).WithValue(value).WithIsInline(momentum.Inline).Build();
                }
            });
        }

        [ButtonAction("lose-momentum")]
        public async Task LoseMomentum(SocketMessageComponent component)
        {
            await component.ModifyOriginalResponseAsync(msg => {
                var embed = msg.Embeds.Value.FirstOrDefault();
                var momentum = embed.Fields.FirstOrDefault(f => f.Name == "Momentum");
                if (int.TryParse(momentum.Value, out var value))
                {
                    value--;
                    if (value < -6) value = -6;
                    momentum = new EmbedFieldBuilder().WithName(momentum.Name).WithValue(value).WithIsInline(momentum.Inline).Build();
                }
            });
        }

        public async Task HandleButton(SocketMessageComponent component)
        {
            // We can now check for our custom id
            switch (component.Data.CustomId)
            {
                // Since we set our buttons custom id as 'custom-id', we can check for it like this:
                case "custom-id":
                    // Lets respond by sending a message saying they clicked the button
                    await component.RespondAsync($"{component.User.Mention} has clicked the button!");
                    break;
            }
        }

        public bool CanHandleButton(string buttonId)
        {
            switch (buttonId)
            {
                case "add-momenutum":
                case "lose-momenutum":
                case "add-supply":
                case "lose-supply":
                case "burn-momentum":
                case "add-health":
                case "lose-health":
                case "add-spirit":
                case "lose-spirit":
                    return true;

                default:
                    return false;
            }
        }
    }
}