using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle2.UserContent
{
    public interface IOracleGameObjectTemplate
    {

    }

    /* Things to solve/support:
     * Action buttons for oracles
     * Action buttons for trackers?
     * Action rows
     * drop down lists?
     * Fields with oracle names
     */
    public class NPCTemplate : IOracleGameObjectTemplate
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public IList<EmbedFieldBuilder> Fields { get; set; } = new List<EmbedFieldBuilder>();

        public async Task CreateMessage(SocketSlashCommand context)
        {
            var compBuilder = new ComponentBuilder()
                .WithButton("Asset Button", "custom-id");

            EmbedBuilder builder = new EmbedBuilder();
            if (Author != null) builder.WithAuthor(Author);
            if (Title != null) builder.WithTitle(Title);

            builder.WithFields(Fields);

            await context.RespondAsync(embed: builder.Build(), component: compBuilder.Build());
        }
    }
}
