using System.ComponentModel.DataAnnotations;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;

namespace TheOracle2.GameObjects;

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

    public PlayerCharacter(string name, int edge, int heart, int iron, int shadow, int wits, string? AvatarImageUrl, ulong ownerId, ulong? guildId, ulong messageId) : this()
    {
        Name = name;
        Edge = edge;
        Heart = heart;
        Iron = iron;
        Shadow = shadow;
        Wits = wits;
        UserId = ownerId;
        DiscordGuildId = guildId;
        MessageId = messageId;
        Image = AvatarImageUrl;
    }

    /// <summary>
    /// This constructor is only for EF/Json
    /// </summary>
    public PlayerCharacter()
    {
        Name = String.Empty;
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
    public ulong? DiscordGuildId { get; set; }
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
    public string? Image { get; set; }
    public IList<string> Impacts { get; set; }

    internal void BurnMomentum()
    {
        Momentum = Math.Max(2 - Impacts.Count, 0);
    }
}
