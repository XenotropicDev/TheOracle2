using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using TheOracle2.UserContent;

namespace TheOracle2.GameObjects;

[Index(nameof(MessageId))]
public class PlayerCharacter
{
    private int momentum;
    private int supply;
    private int spirit;
    private int health;
    private int wits;
    private int shadow;
    private int iron;
    private int heart;
    private int edge;

    public PlayerCharacter(SocketInteractionContext interactionContext, string name, int edge, int heart, int iron, int shadow, int wits) : this(interactionContext.User.Id, interactionContext.Guild?.Id ?? interactionContext.User.Id, name, edge, heart, iron, shadow, wits)
    {
        ChannelId = interactionContext.Interaction.Channel.Id;
    }

    public PlayerCharacter(ulong ownerId, ulong guildId, ulong messageId, string name, int edge, int heart, int iron, int shadow, int wits) : this(ownerId, guildId, name, edge, heart, iron, shadow, wits)
    {
        MessageId = messageId;
    }

    private PlayerCharacter(ulong ownerId, ulong guildId, string name, int edge, int heart, int iron, int shadow, int wits) : this(name, edge, heart, iron, shadow, wits)
    {
        UserId = ownerId;
        DiscordGuildId = guildId;
    }

    private PlayerCharacter(string name, int edge, int heart, int iron, int shadow, int wits) : this()
    {
        Name = name;
        Edge = edge;
        Heart = heart;
        Iron = iron;
        Shadow = shadow;
        Wits = wits;
    }

    /// <summary>
    /// This constructor is only for EF/Json
    /// </summary>
    public PlayerCharacter()
    {
        Health = 5;
        Spirit = 5;
        Supply = 5;
        Momentum = MomentumResetBase;
        XpGained = 0;
        XpSpent = 0;
        Impacts = new List<string>();
    }

    public int Id { get; set; }
    public ulong UserId { get; set; }
    public ulong DiscordGuildId { get; set; }
    public ulong MessageId { get; set; }
    public ulong ChannelId { get; set; }
    /// <summary>
    /// Reconstructs the PC embed's jump URL from it's guild id, channel id, and message id, if possible. Otherwise returns an empty string. This will break if Discord changes its url structure (but that seems unlikely in the forseeable future).
    /// </summary>
    public string JumpUrl => DiscordGuildId != 0 && ChannelId != 0 && MessageId != 0 ? $"https://discord.com/channels/{DiscordGuildId}/{ChannelId}/{MessageId}" : string.Empty;
    public string Name { get; set; }
    public int Edge { get => edge; set => edge = (value >= 4) ? 4 : (value <= 1) ? 1 : value; }
    public int Heart { get => heart; set => heart = (value >= 4) ? 4 : (value <= 1) ? 1 : value; }
    public int Iron { get => iron; set => iron = (value >= 4) ? 4 : (value <= 1) ? 1 : value; }
    public int Shadow { get => shadow; set => shadow = (value >= 4) ? 4 : (value <= 1) ? 1 : value; }
    public int Wits { get => wits; set => wits = (value >= 4) ? 4 : (value <= 1) ? 1 : value; }

    public int Health { get => health; set => health = (value >= 5) ? 5 : (value <= 0) ? 0 : value; }
    public int Spirit { get => spirit; set => spirit = (value >= 5) ? 5 : (value <= 0) ? 0 : value; }
    public int Supply { get => supply; set => supply = (value >= 5) ? 5 : (value <= 0) ? 0 : value; }
    public IList<string> Impacts { get; set; }
    private const int MomentumResetBase = 2;
    private const int MomentumResetMin = 0;
    public int MomentumReset => Math.Max(MomentumResetBase - (ImpactCount), MomentumResetMin);
    public const int MomentumMin = -6;
    private const int MomentumMaxBase = 10;

    // seems silly, but it was throwing on empty lists and things like Any() didn't seem to work, maybe because the DB removes empty lists?
    public int ImpactCount => Impacts == null ? 0 : Impacts.Count;
    public int MomentumMax => MomentumMaxBase - (ImpactCount);
    public int Momentum { get => momentum; set => momentum = (value >= MomentumMax) ? MomentumMax : (value <= MomentumMin) ? MomentumMin : value; }
    public int XpGained { get; set; }
    public int XpSpent { get; set; }
    public string Image { get; set; }

    // this is mathematically required by the dice anyways, but is included as an extra safeguard against stuff getting weird.
    public const int MinMomentumToBurn = 2;

    /// <summary>
    /// Attempts to reset momentum to a PC's momentum reset value, generally only used by momentum burn (but see BurnMomentum() for a more complete option). Returns true if it succeeds, and false if it fails.
    /// </summary>
    public bool ResetMomentum()
    {
        if (Momentum >= MinMomentumToBurn)
        {
            Momentum = MomentumReset;
            return true;
        }
        return false;
    }
    /// <summary>
    /// Attempts to burn momentum on an ActionRoll; sets momentum on the PlayerCharacter if it succeeds, and returns the new ActionRoll result.
    /// </summary>
    public ActionRoll BurnMomentum(ActionRoll roll)
    {
        roll.Momentum = Momentum;
        if (!roll.IsBurnable)
        {
            throw new Exception($"Unable to burn {Momentum} momentum because it does not beat any challenge dice values ({roll.ChallengeDice})");
        }
        // this shouldn't happen normally, but if something goes wrong it might make it easier to diagnose where the math is incorrect.
        if (!ResetMomentum())
        {
            throw new Exception($"Unable to burn {Momentum} momentum. Momentum of less than {MinMomentumToBurn} can't cancel any challenge die result.");
        }
        roll.IsBurnt = true;
        return roll;
    }

    public GuildPlayer GetLastGuildPlayer(EFContext dbContext)
    {
        return dbContext.GuildPlayers.FirstOrDefault(guildPlayer => guildPlayer.LastUsedPcId == Id);
    }
}
