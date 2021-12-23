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

    public virtual ICollection<Asset> Assets { get; set; }

    public virtual ICollection<OracleInfo> Oracles { get; set; }
    public virtual ICollection<Move> Moves { get; set; }
}