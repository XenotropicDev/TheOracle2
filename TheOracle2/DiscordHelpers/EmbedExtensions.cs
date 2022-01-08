using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle2.DiscordHelpers
{
    public static class EmbedExtensions
    {
        public static EmbedAuthorBuilder ToEmbedAuthorBuilder(this EmbedAuthor author)
        {
            return new EmbedAuthorBuilder().WithName(author.Name).WithUrl(author.Url).WithIconUrl(author.IconUrl);
        }
    }
}
