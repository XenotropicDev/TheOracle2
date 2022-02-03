using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle2.UserContent
{
    internal class DiscordEntity : IDiscordEntity
    {
        public bool IsEphemeral { get; set; }
        public List<Embed> Embeds { get; private set; } = new();
        public MessageComponent Components { get; private set; }

        public MessageComponent GetComponents()
        {
            return this.Components;
        }

        public Embed[] GetEmbeds()
        {
            return this.Embeds.ToArray();
        }

        public DiscordEntity WithComponent(MessageComponent component)
        {
            this.Components = component;
            return this;
        }

        public DiscordEntity WithEmbed(Embed embed)
        {
            this.Embeds.Add(embed);
            return this;
        }
    }
}
