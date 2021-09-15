using OracleData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle2.UserContent
{
    public class OracleGuild
    {
        public static OracleGuild GetGuild(ulong id, EFContext context)
        {
            var user = context.OracleGuilds.Find(id);
            if (user != null) return user;

            user = new OracleGuild() {OracleGuildId = id };
            context.OracleGuilds.Add(user);
            return user;
        }

        public ulong OracleGuildId { get; internal set; }

        public ICollection<GameItem> GameItems { get; set; } = new List<GameItem>();
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}
