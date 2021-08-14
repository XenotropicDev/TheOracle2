using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle2
{
    interface ISlashCommand
    {
        public SocketSlashCommand Context { get; set; }

        IList<SlashCommandBuilder> GetCommandBuilders();
    }
}
