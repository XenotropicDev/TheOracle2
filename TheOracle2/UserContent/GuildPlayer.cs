using Discord.Interactions;
using TheOracle2.GameObjects;

namespace TheOracle2.UserContent;

/// <summary>
/// Represents a set of preferences associated with a specific user in a specific guild. A player's settings in one guild are independent of that same player's settings in another guild.
/// </summary>
public class GuildPlayer
{
    public GuildPlayer(EFContext dbContext, ulong userId, ulong discordGuildId)
    {
        DbContext = dbContext;
        UserId = userId;
        DiscordGuildId = discordGuildId;
    }

    public GuildPlayer(EFContext dbContext, ulong userId, ulong discordGuildId, int lastUsedPcId) : this(dbContext, userId, discordGuildId)
    {
        LastUsedPcId = lastUsedPcId;
    }

    public GuildPlayer(EFContext dbContext, SocketInteractionContext interaction) : this(dbContext, interaction.User.Id, interaction.Guild?.Id ?? interaction.User.Id)
    {
    }

    public GuildPlayer(EFContext dbContext, SocketInteractionContext interaction, int lastUsedPcId) : this(dbContext, interaction)
    {
        LastUsedPcId = lastUsedPcId;
    }

    /// <summary>
    /// The EFContext of this GuildPlayer.
    /// </summary>
    public EFContext DbContext { get; set; }

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
    /// <param name="userId"></param>
    /// <param name="guildId"></param>
    /// <param name="pcId">Optional, a value to set LastUsedPcId.</param>
    public static GuildPlayer GetAndAddIfMissing(EFContext dbContext, ulong userId, ulong guildId, int pcId = 0)
    {
        var guildPlayer = dbContext.GuildPlayers.Find(userId, guildId);
        if (guildPlayer == null)
        {
            guildPlayer = new GuildPlayer(dbContext, userId, guildId);
            dbContext.GuildPlayers.Add(guildPlayer);
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
    /// <param name="dbContext"></param>
    /// <param name="Context"></param>
    /// <param name="pcId">Optional, a value to set LastUsedPcId.</param>
    public static GuildPlayer GetAndAddIfMissing(EFContext dbContext, IInteractionContext Context, int pcId = 0)
    {
        var userId = Context.Interaction.User.Id;
        var guildId = Context.Guild?.Id ?? userId;
        return GetAndAddIfMissing(dbContext, userId, guildId, pcId);
    }

    public PlayerCharacter LastUsedPc()
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
    public IEnumerable<PlayerCharacter> GetPcs()
    {
        return GetUserPcs(DiscordGuildId);
    }

    /// <summary>
    /// Get all PCs owned by the User of this GuildPlayer, regardless of server.
    /// </summary>
    /// <param name="guildId">Optional guildId to search within a specific guild.</param>
    public IEnumerable<PlayerCharacter> GetUserPcs(ulong? guildId = null)
    {
        if (guildId == null)
        {
            return DbContext.PlayerCharacters.Where(pc => pc.UserId == UserId);
        }
        return DbContext.PlayerCharacters.Where(pc => pc.UserId == UserId && pc.DiscordGuildId == guildId);
    }

    public void CleanupLastUsedPc()
    {
        if (LastUsedPc() == null && LastUsedPcId != 0)
        {
            LastUsedPcId = 0;
        }
    }
}