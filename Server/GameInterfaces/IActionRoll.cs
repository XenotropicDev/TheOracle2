using Server.GameInterfaces;
using TheOracle2;
using TheOracle2.GameObjects;

namespace Server.DiceRoller
{
    public interface IActionRoll : IMatchable, IDiscordEntity
    {
        IDie Action { get; set; }
        IDie Challenge1 { get; set; }
        IDie Challenge2 { get; set; }

        IronswornRollOutcome GetOutcome();
    }

    public class ActionRollRandom : IActionRoll
    {
        private List<int> ActionAdds;
        private string description;
        private readonly IEmoteRepository emotes;
        private int? momentum;

        public ActionRollRandom(Random random, IEmoteRepository emotes, int stat, int adds, int? momentum = null, string description = "", int? actionDie = null, int? challengeDie1 = null, int? challengeDie2 = null) : base()
        {
            this.emotes = emotes;
            this.momentum = momentum;
            this.description = description;

            Name = "Action Roll";

            Action = new DieRandom(random, 6, actionDie);
            Challenge1 = new DieRandom(random, 10, challengeDie1);
            Challenge2 = new DieRandom(random, 10, challengeDie2);
            ActionAdds = new List<int>() { stat, adds };
            this.momentum = momentum;
        }

        public IDie Action { get; set; }
        public int ActionScore { get => Math.Min(Action.Value + ActionAdds.Sum(), 10); }
        public IDie Challenge1 { get; set; }
        public IDie Challenge2 { get; set; }
        public bool IsEphemeral { get; set; } = false;
        public bool IsMatch => Challenge1 != null && Challenge2 != null && Challenge1.CompareTo(Challenge2) == 0;
        public string Name { get; set; }
        public string? DiscordMessage { get; set; } = null;

        public IronswornRollOutcome BurnResult()
        {
            if (momentum == null) return IronswornRollOutcome.Miss;
            if (momentum > Math.Max(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.StrongHit;
            if (momentum > Math.Min(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.WeakHit;
            return IronswornRollOutcome.Miss;
        }

        public bool CanBurn()
        {
            if (momentum == null) return false;
            return momentum > ActionScore && momentum > Math.Min(Challenge1.Value, Challenge2.Value);
        }

        public ComponentBuilder? GetComponents()
        {
            var builder = new ComponentBuilder();
            if (GetOutcome() == IronswornRollOutcome.Miss)
            {
                builder.WithButton("Pay the Price", "roll-oracle:Oracles/Moves/Pay_the_Price", emote: emotes.Roll);
            }
            return builder;
        }

        public EmbedBuilder GetEmbed()
        {
            var embed = new EmbedBuilder()
                .WithAuthor(Name)
                .WithTitle(GetOutcome().ToOutcomeString(IsMatch))
                .WithColor(GetOutcome().OutcomeColor())
                .WithThumbnailUrl(GetOutcome().OutcomeIcon());

            if (!string.IsNullOrWhiteSpace(description)) { embed.WithDescription(description); }

            if (CanBurn()) embed.WithFooter($"You may burn +{momentum} momentum for a {BurnResult().ToOutcomeString()} (see p. 32).");
            if (Action.Value + ActionAdds.Sum() > 10) embed.WithFooter(IronswornRollResources.OverMaxMessage);

            embed.AddField("Action Score", $"{Action.Value} + {String.Join(" + ", ActionAdds)} = {ActionScore}")
                .AddField("Challenge Dice", $"{Challenge1.Value}, {Challenge2.Value}");

            return embed;
        }

        public IronswornRollOutcome GetOutcome()
        {
            if (ActionScore > Math.Max(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.StrongHit;
            if (ActionScore > Math.Min(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.WeakHit;
            return IronswornRollOutcome.Miss;
        }
    }

    public class ProgressRollRandom : IActionRoll
    {
        private readonly string description;

        public ProgressRollRandom(Random random, int progressAmount, string description, int? challengeDie1 = null, int? challengeDie2 = null) : base()
        {
            Action = new DieRandom(random, 6, progressAmount);
            Challenge1 = new DieRandom(random, 10, challengeDie1);
            Challenge2 = new DieRandom(random, 10, challengeDie2);

            Name = "Progress Roll";
            this.description = description;
        }

        public IDie Action { get; set; }
        public IDie Challenge1 { get; set; }
        public IDie Challenge2 { get; set; }
        public bool IsEphemeral { get; set; } = false;
        public bool IsMatch => Challenge1 != null && Challenge2 != null && Challenge1.CompareTo(Challenge2) == 0;
        public string Name { get; set; }
        public string? DiscordMessage { get; set; } = null;

        public ComponentBuilder? GetComponents()
        {
            return null;
        }

        public EmbedBuilder GetEmbed()
        {
            var embed = new EmbedBuilder()
                .WithAuthor(Name)
                .WithTitle(GetOutcome().ToOutcomeString())
                .WithColor(GetOutcome().OutcomeColor())
                .WithThumbnailUrl(GetOutcome().OutcomeIcon());

            if (!String.IsNullOrWhiteSpace(description)) embed.WithDescription(description);

            embed.AddField("Action Score", $"{Action.Value}")
                .AddField("Challenge Dice", $"{Challenge1.Value}, {Challenge2.Value}");

            return embed;
        }

        public IronswornRollOutcome GetOutcome()
        {
            if (Action.Value > Math.Max(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.StrongHit;
            if (Action.Value > Math.Min(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.WeakHit;
            return IronswornRollOutcome.Miss;
        }
    }
}
