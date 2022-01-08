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

        public string GetDiscordMessage()
        {
            return null;
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
    }
}