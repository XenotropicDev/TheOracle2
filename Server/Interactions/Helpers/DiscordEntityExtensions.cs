using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TheOracle2;

namespace Server.Interactions.Helpers
{
    public static class DiscordEntityExtensions
    {
        public static Embed[]? AsEmbedArray(this IDiscordEntity entity)
        {
            var embed = entity.GetEmbed();
            if (embed == null) return null;

            return new Embed[] { embed.Build() };
        }

        public static MessageComponent? AsMessageComponent(this IDiscordEntity entity)
        {
            var comp = entity.GetComponents();
            if (comp == null) return null;

            return comp.Build();
        }
    }
}
