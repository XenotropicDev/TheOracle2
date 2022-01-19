using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2;

public class PlayerCardCommand : InteractionModuleBase<SocketInteractionContext>
{
    public PlayerCardCommand(EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public EFContext DbContext { get; }

    [SlashCommand("player", "Generates a post to keep track of a player character's stats")]
    public async Task BuildPlayerCard(string name, [MaxValue(4)][MinValue(1)] int edge, [MaxValue(4)][MinValue(1)] int heart, [MaxValue(4)][MinValue(1)] int iron, [MaxValue(4)][MinValue(1)] int shadow, [MaxValue(4)][MinValue(1)] int wits)
    {
        await DeferAsync();
        var pc = new PlayerCharacter(Context, name, edge, heart, iron, shadow, wits);
        DbContext.PlayerCharacters.Add(pc);
        var userId = Context.Interaction.User.Id;
        var guildId = Context.Guild?.Id ?? userId;
        var guildPlayer = DbContext.GuildPlayers.Find(userId, guildId);
        if (guildPlayer == null)
        {
            guildPlayer = new GuildPlayer(Context);
            DbContext.GuildPlayers.Add(guildPlayer);
        }

        await DbContext.SaveChangesAsync();

        if (guildPlayer.LastUsedPcId != pc.Id)
        {
            guildPlayer.LastUsedPcId = pc.Id;
            await DbContext.SaveChangesAsync();
        }
        // for debugging without having to alt tab. can be safely removed once this draft PR is finalized
        // string json = "```" + JsonConvert.SerializeObject(guildPlayer, Formatting.Indented) + "```";
        // var entity = new PlayerCharacterEntity(pc);
        // await FollowupAsync(text: json, embeds: entity.GetEmbeds(), components: entity.GetComponents());
    }
}

public class PlayerCardComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public PlayerCardComponents(EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public EFContext DbContext { get; }

    [ComponentInteraction("add-momentum-*")]
    public async Task AddMomentum(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Momentum++).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-momentum-*")]
    public async Task LoseMomentum(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Momentum--).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-supply-*")]
    public async Task loseSupply(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Supply--).ConfigureAwait(false);
    }

    [ComponentInteraction("add-supply-*")]
    public async Task addSupply(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Supply++).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-health-*")]
    public async Task loseHealth(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Health--).ConfigureAwait(false);
    }

    [ComponentInteraction("add-health-*")]
    public async Task addHealth(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Health++).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-spirit-*")]
    public async Task loseSpirit(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Spirit--).ConfigureAwait(false);
    }

    [ComponentInteraction("add-spirit-*")]
    public async Task addSpirit(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.Spirit++).ConfigureAwait(false);
    }

    [ComponentInteraction("add-xp-*")]
    public async Task addXp(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.XpGained++).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-xp-*")]
    public async Task loseXp(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.XpGained--).ConfigureAwait(false);
    }

    [ComponentInteraction("burn-momentum-*")]
    public async Task burn(string pcId)
    {
        await UpdatePCValue(pcId, pc => pc.BurnMomentum()).ConfigureAwait(false);
    }

    [ComponentInteraction("player-more-*")]
    public async Task toggleMore(string pcId)
    {
        try
        {
            await Context.Interaction.UpdateAsync(msg =>
            {
                ComponentBuilder components = ComponentBuilder.FromMessage(Context.Interaction.Message);

                components.TryAdd(ButtonBuilder.CreateSuccessButton("+Xp", $"add-xp-{pcId}").Build(), 2);
                components.TryAdd(ButtonBuilder.CreateSecondaryButton("-Xp", $"lose-xp-{pcId}").Build(), 2);

                components.ReplaceComponentById($"player-more-{pcId}", ButtonBuilder.CreatePrimaryButton("less", $"player-less-{pcId}").Build());

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

    [ComponentInteraction("player-less-*")]
    public async Task toggleLess(string pcId)
    {
        try
        {
            await Context.Interaction.UpdateAsync(msg =>
            {
                ComponentBuilder components = ComponentBuilder.FromMessage(Context.Interaction.Message)
                .RemoveComponentById($"add-xp-{pcId}")
                .RemoveComponentById($"lose-xp-{pcId}");

                components.ReplaceComponentById($"player-less-{pcId}", ButtonBuilder.CreatePrimaryButton("...", $"player-more-{pcId}").Build());

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

    private async Task UpdatePCValue(string pcId, Action<PlayerCharacter> change)
    {
        if (!int.TryParse(pcId, out var Id)) return;
        var pc = DbContext.PlayerCharacters.Find(Id);
        var userId = Context.User.Id;
        var guildId = Context.Guild?.Id ?? Context.User.Id;
        var guildPlayer = DbContext.GuildPlayers.Find(userId, guildId);
        if (guildPlayer != null)
        {
            guildPlayer.LastUsedPcId = Id;
        }
        if (guildPlayer == null)
        {
            guildPlayer = new GuildPlayer(userId, guildId, Id);
            DbContext.GuildPlayers.Add(guildPlayer);
        }
        if (pc.MessageId != Context.Interaction.Message.Id)
        {
            pc.MessageId = Context.Interaction.Message.Id;
            pc.ChannelId = Context.Interaction.Channel.Id;
        }

        change(pc);
        await DbContext.SaveChangesAsync();

        var entity = new PlayerCharacterEntity(pc);
        await Context.Interaction.UpdateAsync(msg =>
        {
            msg.Embeds = entity.GetEmbeds();
        }).ConfigureAwait(false);
        // for debugging without having to alt tab. can be safely removed once this draft PR is finalized
        // string json = "```" + JsonConvert.SerializeObject(guildPlayer, Formatting.Indented) + "```";
        // await Context.Interaction.FollowupAsync(json);
    }
}
