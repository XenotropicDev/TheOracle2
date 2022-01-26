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

    /// <summary>
    /// Finds a GuildPlayer by their user id and guild id; if they don't exist, creates a new GuildPlayer for those ids. Returns the GuildPlayer.
    /// </summary>
    /// <param name="DbContext"></param>
    /// <param name="userId"></param>
    /// <param name="guildId"></param>
    /// <param name="pcId">Optional, a value to set LastUsedPcId.</param>
    public static GuildPlayer AddIfMissing(EFContext DbContext, ulong userId, ulong guildId, int pcId = 0)
    {
        var guildPlayer = DbContext.GuildPlayers.Find(userId, guildId);
        if (guildPlayer == null)
        {
            guildPlayer = new GuildPlayer(userId, guildId);
            DbContext.GuildPlayers.Add(guildPlayer);
        }
        if (pcId != 0)
        {
            guildPlayer.LastUsedPcId = pcId;
        }
        return guildPlayer;
    }
    /// <summary>
    /// Finds a GuildPlayer by their user id and guild id; if they don't exist, creates a new GuildPlayer for those ids. Returns the GuildPlayer.
    /// </summary>
    /// <param name="Context"></param>
    /// <param name="DbContext"></param>
    /// <param name="pcId">Optional, a value to set LastUsedPcId.</param>
    public static GuildPlayer GetAndAddIfMissing(IInteractionContext Context, EFContext DbContext, int pcId = 0)
    {
        var userId = Context.Interaction.User.Id;
        var guildId = Context.Guild?.Id ?? userId;
        return AddIfMissing(DbContext, userId, guildId, pcId);
    }
    public PlayerCharacter LastUsedPc(EFContext DbContext)
    {
        if (LastUsedPcId == 0)
        {
            return null;
        }
        return DbContext.PlayerCharacters.Find(LastUsedPcId);
    }
    /// <summary>
    /// Get all PCs owned by this GuildPlayer.
    /// </summary>
    /// <param name="DbContext"></param>
    public IEnumerable<PlayerCharacter> GetPcs(EFContext DbContext)
    {
        return GetUserPcs(DbContext, DiscordGuildId);
    }
    /// <summary>
    /// Get all PCs owned by the User of this GuildPlayer, regardless of server.
    /// </summary>
    /// <param name="DbContext"></param>
    /// <param name="guildId">Optional guildId to search within a specific guild.</param>
    public IEnumerable<PlayerCharacter> GetUserPcs(EFContext DbContext, ulong? guildId = null)
    {
        if (guildId == null)
        {
            return DbContext.PlayerCharacters.Where(pc => pc.UserId == UserId);
        }
        return DbContext.PlayerCharacters.Where(pc => pc.UserId == UserId && pc.DiscordGuildId == guildId);
    }
    public void CleanupLastUsedPc(EFContext DbContext)
    {
        if (LastUsedPc(DbContext) == null && this.LastUsedPcId != 0)
        {
            this.LastUsedPcId = 0;
        }
        return;
    }
}
