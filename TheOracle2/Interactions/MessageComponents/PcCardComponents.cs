using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using TheOracle2.Commands;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;


namespace TheOracle2;

public class PcCardComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public PcCardComponents(EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public GuildPlayer GuildPlayer => GuildPlayer.GetAndAddIfMissing(DbContext, Context);
    public EFContext DbContext { get; }

    public override async Task AfterExecuteAsync(ICommandInfo command)
    {
        await DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

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
        if (!int.TryParse(pcId, out var Id))
        {
            throw new ArgumentException($"Unable to parse integer from {pcId}");
        }
        var pc = await DbContext.PlayerCharacters.FindAsync(Id);
        if (pc.MessageId != Context.Interaction.Message.Id)
        {
            pc.MessageId = Context.Interaction.Message.Id;
            pc.ChannelId = Context.Interaction.Channel.Id;
        }
        change(pc);
        GuildPlayer.LastUsedPcId = Id;
        var entity = new PlayerCharacterEntity(pc);
        await Context.Interaction.UpdateAsync(msg =>
        {
            msg.Embeds = entity.GetEmbeds();
        }).ConfigureAwait(false);
    }
}
