using Discord.WebSocket;
using TheOracle2.GameObjects;

namespace TheOracle2.UserContent
{
    internal class PlayerCharacterEntity : IDiscordEntity
    {
        public PlayerCharacterEntity(PlayerCharacter Pc)
        {
            this.Pc = Pc;
        }

        public bool IsEphemeral { get; set; } = false;

        public PlayerCharacter Pc { get; }

        public MessageComponent GetComponents() => new ComponentBuilder()
                .WithButton("+Hp", $"add-health-{Pc.Id}", row: 0, style: ButtonStyle.Success)
                .WithButton("-Hp", $"lose-health-{Pc.Id}", row: 1, style: ButtonStyle.Secondary)
                .WithButton("+Sp", $"add-spirit-{Pc.Id}", row: 0, style: ButtonStyle.Success)
                .WithButton("-Sp", $"lose-spirit-{Pc.Id}", row: 1, style: ButtonStyle.Secondary)
                .WithButton("+Su", $"add-supply-{Pc.Id}", row: 0, style: ButtonStyle.Success)
                .WithButton("-Su", $"lose-supply-{Pc.Id}", row: 1, style: ButtonStyle.Secondary)
                .WithButton("+Mo", $"add-momentum-{Pc.Id}", row: 0, style: ButtonStyle.Success)
                .WithButton("-Mo", $"lose-momentum-{Pc.Id}", row: 1, style: ButtonStyle.Secondary)
                .WithButton("Burn", $"burn-momentum-{Pc.Id}", row: 0, style: ButtonStyle.Danger, emote: new Emoji("🔥"))
                .WithButton("...", $"player-more-{Pc.Id}", row: 0, style: ButtonStyle.Primary).Build();

        public async Task<IMessage> GetDiscordMessage(IInteractionContext context)
        {
            var channel = (Pc.ChannelId == context.Channel.Id) ? context.Channel : await (context.Client as DiscordSocketClient)?.Rest.GetChannelAsync(Pc.ChannelId) as IMessageChannel;
            return await channel.GetMessageAsync(Pc.MessageId);
        }

        public Embed[] GetEmbeds()
        {
            var builder = new EmbedBuilder()
            .WithAuthor($"Player Card")
            .WithTitle(Pc.Name)
            .WithThumbnailUrl(Pc.Image)
            .AddField("Stats", $"Edge: {Pc.Edge}, Heart: {Pc.Heart}, Iron: {Pc.Iron}, Shadow: {Pc.Shadow}, Wits: {Pc.Wits}")
            .AddField("Health", Pc.Health, true)
            .AddField("Spirit", Pc.Spirit, true)
            .AddField("Supply", Pc.Supply, true)
            .AddField("Momentum", Pc.Momentum, true)
            .AddField("XP", Pc.XpGained);

            if (Pc.Impacts.Count > 0)
                builder.AddField("Impacts", String.Join(", ", Pc.Impacts));

            return new Embed[] { builder.Build() };
        }

        /// <summary>
        /// Make an action roll using one of this PC's stats.
        /// </summary>
        public ActionRoll RollAction(Random random, RollableStats stat, int adds, string description = "", int? actionDie = null, int? challengeDie1 = null, int? challengeDie2 = null, string moveName = "")
        {
            ActionRoll roll = new ActionRoll(random: random,
                stat: GetStatValue(stat),
                adds: adds,
                momentum: Pc.Momentum,
                description: description,
                actionDie: actionDie,
                challengeDie1: challengeDie1,
                challengeDie2: challengeDie2,
                moveName: moveName,
                pcName: Pc.Name,
                statName: stat.ToString()
                );
            return roll;
        }

        private int GetStatValue(RollableStats stat)
        {
            return stat switch
            {
                RollableStats.Edge => Pc.Edge,
                RollableStats.Heart => Pc.Heart,
                RollableStats.Iron => Pc.Iron,
                RollableStats.Shadow => Pc.Shadow,
                RollableStats.Wits => Pc.Wits,
                RollableStats.Health => Pc.Health,
                RollableStats.Spirit => Pc.Spirit,
                RollableStats.Supply => Pc.Supply,
                _ => throw new NotImplementedException(),
            };
        }
    }
}