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
        Momentum = 2;
        XpGained = 0;
        XpSpent = 0;
        Impacts = new List<string>();
    }

    public int Id { get; set; }
    public ulong UserId { get; set; }
    public ulong DiscordGuildId { get; set; }
    public ulong MessageId { get; set; }
    public ulong ChannelId { get; set; }
    public string Name { get; set; }
    public int Edge { get => edge; set => edge = (value >= 4) ? 4 : (value <= 1) ? 1 : value; }
    public int Heart { get => heart; set => heart = (value >= 4) ? 4 : (value <= 1) ? 1 : value; }
    public int Iron { get => iron; set => iron = (value >= 4) ? 4 : (value <= 1) ? 1 : value; }
    public int Shadow { get => shadow; set => shadow = (value >= 4) ? 4 : (value <= 1) ? 1 : value; }
    public int Wits { get => wits; set => wits = (value >= 4) ? 4 : (value <= 1) ? 1 : value; }

    public int Health { get => health; set => health = (value >= 5) ? 5 : (value <= 0) ? 0 : value; }
    public int Spirit { get => spirit; set => spirit = (value >= 5) ? 5 : (value <= 0) ? 0 : value; }
    public int Supply { get => supply; set => supply = (value >= 5) ? 5 : (value <= 0) ? 0 : value; }
    public int Momentum { get => momentum; set => momentum = (value >= 10) ? 10 : (value <= -6) ? -6 : value; }
    public int XpGained { get; set; }
    public int XpSpent { get; set; }
    public string Image { get; set; }

    public IList<string> Impacts { get; set; }

    internal void BurnMomentum()
    {
        Momentum = Math.Max(2 - Impacts.Count, 0);
    }

    public GuildPlayer GetLastGuildPlayer(EFContext dbContext)
    {
        return dbContext.GuildPlayers.FirstOrDefault(guildPlayer => guildPlayer.LastUsedPcId == Id);
    }
}
