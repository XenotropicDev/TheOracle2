using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TheOracle2;
using TheOracle2.Data;

namespace Server.Interactions.Helpers
{
    public static class MoveExtensions
    {
        public static SelectMenuOptionBuilder MoveAsSelectOption(this Move move, IEmoteRepository emotes)
        {
            var builder = new SelectMenuOptionBuilder();

            
            var closingBolds = move.Text.IndexOf("**", move.Text.IndexOf("**") + 2) + 2;
            var desc = (closingBolds > 0 && closingBolds < 70) ? move.Text[..closingBolds] : move.Text[..67] + "...";

            builder.WithValue($"reference-post-{move.Id}").WithDescription(desc).WithEmote(emotes.Reference).WithLabel(move.Name);

            return builder;
        }
    }
}
