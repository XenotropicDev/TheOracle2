using Discord.WebSocket;
using TheOracle2.GameObjects;

namespace TheOracle2.UserContent
{
    internal class PlayerCharacterEntity : IDiscordEntity
    {
        public PlayerCharacterEntity(EFContext dbContext, PlayerCharacter Pc)
        {
            this.Pc = Pc;
            DbContext = dbContext;
        }

        public bool IsEphemeral { get; set; } = false;
        public EFContext DbContext { get; set; }

        public PlayerCharacter Pc { get; }

        /// <summary>
        /// Sets an EmbedBuilder's author link to the PC's embed, and the author icon to the PC's image. This should be used to attribute embeds to the originating character when appropriate.
        /// </summary>
        public async Task<EmbedBuilder> AddPcAuthorTemplate(EmbedBuilder embed, IInteractionContext context)
        {
            if (Pc.MessageId > 0)
            {
                embed.Author.WithUrl(await GetJumpUrl(context));
            }
            if (!string.IsNullOrEmpty(Pc.Image))
            {
                embed.Author.WithIconUrl(Pc.Image);
            }
            return embed;
        }

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
        public async Task<IMessage> GetMessageAsync(IDiscordClient client)
        {
            var channel = await GetChannelAsync(client);
            return await channel.GetMessageAsync(Pc.MessageId);
        }
        public async Task<IMessage> GetMessageAsync(IInteractionContext context)
        {
            if (context.Channel.Id == Pc.ChannelId)
            {
                return await context.Channel.GetMessageAsync(Pc.MessageId);
            }
            return await GetMessageAsync(context.Client);
        }
        public async Task<IMessageChannel> GetChannelAsync(IDiscordClient client)
        {
            return await client.GetChannelAsync(Pc.ChannelId) as IMessageChannel;
        }
        /// <summary>
        /// Infers a jump URL to the PC's embed from the PC's Discord Guild, Channel Id, and Message Id.
        /// Otherewise, attempts to look up the message.
        /// </summary>

        public async Task<string> GetJumpUrl(IInteractionContext context)
        {
            if (!string.IsNullOrEmpty(Pc.JumpUrl))
            {
                return Pc.JumpUrl;
            }
            var msg = await GetMessageAsync(context.Client);
            if (msg != null)
            {
                var url = msg.GetJumpUrl();
                if (url != null)
                {
                    return url;
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Adds the PC's portrait and jump link to an existing author field.
        /// </summary>
        public async Task<EmbedAuthorBuilder> PcAuthorTemplate(IInteractionContext context, EmbedAuthorBuilder authorField)
        {
            if (Pc.Image != null)
            {
                authorField.WithIconUrl(Pc.Image);
            }

            var jumpUrl = await GetJumpUrl(context);
            if (jumpUrl != null)
            {
                authorField.WithUrl(jumpUrl);
            }
            return authorField;
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
        /// Make an action roll using one of this PC's stats, and style the Author field to reflect their image and embed url.
        /// </summary>
        public async Task<ActionRoll> RollAction(IInteractionContext context, Random random, RollableStats stat, int adds, string description = "", int? actionDie = null, int? challengeDie1 = null, int? challengeDie2 = null, string moveName = "")
        {
            var jumpUrl = await GetJumpUrl(context);
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
                )
            {
                AuthorIcon = Pc.Image,
                AuthorUrl = jumpUrl
            };
            return roll;
        }
        public async Task<ButtonBuilder> GetJumpButton(IInteractionContext context)
        {
            return new ButtonBuilder()
                .WithLabel(Pc.Name)
                .WithStyle(ButtonStyle.Link)
                .WithUrl(await GetJumpUrl(context))
                .WithEmote(new Emoji("👤"))
                ;
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
