using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.GameObjects;
namespace TheOracle2.UserContent;

/// <summary>
/// Represents a set of preferences associated with a specific user when they use the bot in a given guild. A player's settings in one guild are independent of that same player's settings in another guild.
/// </summary>
public class GuildPlayer
{
    public GuildPlayer(ulong userId, ulong discordGuildId)
    {
        UserId = userId;
        DiscordGuildId = discordGuildId;
    }
    public GuildPlayer(ulong userId, ulong discordGuildId, int lastUsedPcId) : this(userId: userId, discordGuildId: discordGuildId)
    {
        LastUsedPcId = lastUsedPcId;
    }
    public GuildPlayer(SocketInteractionContext interaction) : this(userId: interaction.User.Id, discordGuildId: interaction.Guild?.Id ?? interaction.User.Id) { }
    public GuildPlayer(SocketInteractionContext interaction, int lastUsedPcId) : this(interaction)
    {
        LastUsedPcId = lastUsedPcId;
    }
    /// <summary>
    /// The user's discord snowflake id.
    /// </summary>
    public ulong UserId { get; set; }
    /// <summary>
    /// The discord guild's snowflake id.
    /// </summary>
    public ulong DiscordGuildId { get; set; }
    /// <summary>
    /// The id of the PlayerCharacter last touched by the GuildPlayer.
    /// </summary>
    public int LastUsedPcId { get; set; }
}
