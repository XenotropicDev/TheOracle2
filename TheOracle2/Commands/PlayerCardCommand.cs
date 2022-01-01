using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using TheOracle2.UserContent;

namespace TheOracle2;

public class PlayerCardCommand : InteractionModuleBase
{
    public PlayerCardCommand(UserContent.EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public EFContext DbContext { get; }

    [SlashCommand("player", "Generates a post to keep track of a player character's stats")]
    public async Task BuildPlayerCard(string name, [MaxValue(4)][MinValue(1)] int edge, [MaxValue(4)][MinValue(1)] int heart, [MaxValue(4)][MinValue(1)] int iron, [MaxValue(4)][MinValue(1)] int shadow, [MaxValue(4)][MinValue(1)] int wits)
    {
        var compBuilder = new ComponentBuilder()
        .WithButton("+Hp", "add-health", row: 0, style: ButtonStyle.Success)
        .WithButton("-Hp", "lose-health", row: 1, style: ButtonStyle.Secondary)
        .WithButton("+Sp", "add-spirit", row: 0, style: ButtonStyle.Success)
        .WithButton("-Sp", "lose-spirit", row: 1, style: ButtonStyle.Secondary)
        .WithButton("+Su", "add-supply", row: 0, style: ButtonStyle.Success)
        .WithButton("-Su", "lose-supply", row: 1, style: ButtonStyle.Secondary)
        .WithButton("+Mo", "add-momentum", row: 0, style: ButtonStyle.Success)
        .WithButton("-Mo", "lose-momentum", row: 1, style: ButtonStyle.Secondary)
        .WithButton("Burn", "burn-momentum", row: 0, style: ButtonStyle.Danger, emote: new Emoji("🔥"))
        .WithButton("...", "player-more", row: 0, style: ButtonStyle.Primary)
        ;

        EmbedBuilder builder = new EmbedBuilder();
        builder.WithAuthor($"Player Card");
        builder.WithTitle(name);
        builder.AddField("Stats", $"Edge: {edge}, Heart: {heart}, Iron: {iron}, Shadow: {shadow}, Wits: {wits}");
        builder.AddField("Health", 5, true);
        builder.AddField("Spirit", 5, true);
        builder.AddField("Supply", 5, true);
        builder.AddField("Momentum", 2, true);
        builder.AddField("XP", 0);

        await RespondAsync(embed: builder.Build(), components: compBuilder.Build());
    }

    [ComponentInteraction("add-momentum")]
    public async Task AddMomentum()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        await interaction.UpdateAsync(msg =>
        {
            msg.Embeds = UpdateField(interaction, "Momentum", 1, -6, 12);
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-momentum")]
    public async Task LoseMomentum()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        await interaction.UpdateAsync(msg =>
        {
            msg.Embeds = UpdateField(interaction, "Momentum", -1, -6, 12);
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-supply")]
    public async Task loseSupply()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        await interaction.UpdateAsync(msg =>
        {
            msg.Embeds = UpdateField(interaction, "Supply", -1);
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("add-supply")]
    public async Task addSupply()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        await interaction.UpdateAsync(msg =>
        {
            msg.Embeds = UpdateField(interaction, "Supply", 1);
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-health")]
    public async Task loseHealth()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        await interaction.UpdateAsync(msg =>
        {
            msg.Embeds = UpdateField(interaction, "Health", -1);
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("add-health")]
    public async Task addHealth()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        await interaction.UpdateAsync(msg =>
        {
            msg.Embeds = UpdateField(interaction, "Health", 1);
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-spirit")]
    public async Task loseSpirit()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        await interaction.UpdateAsync(msg =>
        {
            msg.Embeds = UpdateField(interaction, "Spirit", -1);
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("add-spirit")]
    public async Task addSpirit()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        await interaction.UpdateAsync(msg =>
        {
            msg.Embeds = UpdateField(interaction, "Spirit", 1);
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("add-xp")]
    public async Task addXp()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        await interaction.UpdateAsync(msg =>
        {
            msg.Embeds = UpdateField(interaction, "XP", 1);
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-xp")]
    public async Task loseXp()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        await interaction.UpdateAsync(msg =>
        {
            msg.Embeds = UpdateField(interaction, "XP", -1);
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("burn-momentum")]
    public async Task burn()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        await interaction.UpdateAsync(msg =>
        {
            var embed = interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
            var field = embed.Fields.FindIndex(f => f.Name == "Momentum");

            //Todo: add support for debilities `Momentum = 2 - player.Debilities`
            embed.Fields[field].Value = 2;
            msg.Embeds = new Embed[] { embed.Build() };
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("player-more")]
    public async Task toggleMore()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        try
        {
            await interaction.UpdateAsync(msg =>
            {
                ComponentBuilder components = ComponentBuilder.FromMessage(interaction.Message);

                components.TryAdd(ButtonBuilder.CreateSuccessButton("+Xp", "add-xp").Build(), 2);
                components.TryAdd(ButtonBuilder.CreateSecondaryButton("-Xp", "lose-xp").Build(), 2);

                components.ReplaceComponentById("player-more", ButtonBuilder.CreatePrimaryButton("less", "player-less").Build());

                msg.Components = components.Build();
            }).ConfigureAwait(false);
        }
        catch (HttpException hex)
        {
            string json = JsonConvert.SerializeObject(hex.Errors, Formatting.Indented);

            Console.WriteLine(json);
            throw;
        }
    }

    [ComponentInteraction("player-less")]
    public async Task toggleLess()
    {
        var interaction = Context.Interaction as SocketMessageComponent;

        try
        {
            await interaction.UpdateAsync(msg =>
            {
                ComponentBuilder components = ComponentBuilder.FromMessage(interaction.Message)
                .RemoveComponentById("add-xp")
                .RemoveComponentById("lose-xp");

                components.ReplaceComponentById("player-less", ButtonBuilder.CreatePrimaryButton("...", "player-more").Build());

                msg.Components = components.Build();
            }).ConfigureAwait(false);
        }
        catch (HttpException hex)
        {
            string json = JsonConvert.SerializeObject(hex.Errors, Formatting.Indented);
            Console.WriteLine(json);
            throw;
        }
    }

    private static Embed[] UpdateField(SocketMessageComponent interaction, string fieldName, int change, int min = 0, int max = 5)
    {
        var embed = interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
        embed.ChangeNumericField(fieldName, change, min, max);
        return new Embed[] { embed.Build() };
    }
}