using OracleData;
using TheOracle2.DataClasses;

namespace TheOracle2.UserContent;

public class OracleGuild
{
    public static OracleGuild GetGuild(ulong id, EFContext context)
    {
        var user = context.OracleGuilds.Find(id);
        if (user != null) return user;

        user = new OracleGuild() { OracleGuildId = id };
        context.OracleGuilds.Add(user);
        return user;
    }

    public ulong OracleGuildId { get; internal set; }

    public ICollection<Asset> Assets { get; set; } = new List<Asset>();

    public ICollection<OracleInfo> Oracles { get; set; }
    public ICollection<Move> Moves { get; set; }
}