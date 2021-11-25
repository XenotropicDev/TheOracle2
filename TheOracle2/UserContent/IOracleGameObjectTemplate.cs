using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public List<FieldInfo> Fields { get; set; } = new();
        public Dictionary<string, string> Buttons { get; set; } = new Dictionary<string,string>();

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

    public class FieldInfo : EmbedFieldBuilder
    {
        private string title;
        private string text;
        private static Regex embededIdRegex = new(@"({\d+})");

        public new string Name { get => title; set => title = value; }
        public new string Value { get => text; set => text = value; }
        public new bool IsInline { get; set; } = true;

        private string ParseText(string input)
        {
            var ids = embededIdRegex.Matches(input)
                .Select(m => { 
                    int.TryParse(m.Groups[1].Value, out int temp); 
                    return temp; 
                })
                .Where(i => i != 0);

            return "Todo";
        }

    }
}
