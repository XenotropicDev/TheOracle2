using System.Reflection;
using Server.GameInterfaces;
using TheOracle2;
using TheOracle2.GameObjects;

namespace Server.DiceRoller
{
    public interface IActionRoll : IMatchable
    {
        IDie Action { get; set; }
        IDie Challenge1 { get; set; }
        IDie Challenge2 { get; set; }
        IronswornRollOutcome GetOutcome();
        bool IMatchable.IsMatch => Challenge1 != null && Challenge2 != null && Challenge1.CompareTo(Challenge2) == 0;
    }

    public abstract class ActionRoll : IActionRoll, IDiscordEntity
    {
        public ActionRoll()
        {
            Name = "Action Roll";
            Title = "Roll Outcome";
            Action = new DieStatic(0, 0);
            Challenge1 = new DieStatic(0, 0);
            Challenge2 = new DieStatic(0, 0);
        }

        public IDie Action { get; set; }
        public IDie Challenge1 { get; set; }
        public IDie Challenge2 { get; set; }
        public bool IsEphemeral { get; set; } = false;

        public virtual string Name { get; set; }
        public virtual string Title { get; set; }

        public EmbedBuilder GetEmbed()
        {
            var embed = new EmbedBuilder()
                .WithAuthor(Name)
                .WithTitle(Title);

            embed.WithColor(GetOutcome().OutcomeColor())
                .WithThumbnailUrl(GetOutcome().OutcomeIcon());

            return embed;
        }

        public ComponentBuilder GetComponents()
        {
            throw new NotImplementedException();
        }

        public virtual IronswornRollOutcome GetOutcome()
        {
            if (Action.Value > Math.Max(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.StrongHit;
            if (Action.Value > Math.Min(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.WeakHit;
            return IronswornRollOutcome.Miss;
        }
    }

    public class ActionRollRandom : ActionRoll
    {
        private List<int> ActionAdds;
        private int? momentum;
        private string description;

        public ActionRollRandom(Random random, int stat, int adds, int? momentum = null, string description = "", int? actionDie = null, int? challengeDie1 = null, int? challengeDie2 = null) : base()
        {
            this.momentum = momentum;
            this.description = description;

            Action = new DieRandom(random, 6, actionDie);
            Challenge1 = new DieRandom(random, 10, challengeDie1);
            Challenge2 = new DieRandom(random, 10, challengeDie2);
            ActionAdds = new List<int>() { stat, adds };
            this.momentum = momentum;
        }

        public int ActionScore { get => Math.Min(Action.Value + ActionAdds.Sum(), 10); }

        public bool CanBurn()
        {
            if (momentum == null) return false;
            return momentum > ActionScore && momentum > Math.Min(Challenge1.Value, Challenge2.Value);
        }

        public IronswornRollOutcome BurnResult()
        {
            if (momentum == null) return IronswornRollOutcome.Miss;
            if (momentum > Math.Max(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.StrongHit;
            if (momentum > Math.Min(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.WeakHit;
            return IronswornRollOutcome.Miss;
        }

        public override IronswornRollOutcome GetOutcome()
        {
            if (ActionScore > Math.Max(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.StrongHit;
            if (ActionScore > Math.Min(Challenge1.Value, Challenge2.Value)) return IronswornRollOutcome.WeakHit;
            return IronswornRollOutcome.Miss;
        }

        public new EmbedBuilder GetEmbed()
        {
            var embed = base.GetEmbed();

            if (!string.IsNullOrWhiteSpace(description)) { embed.WithDescription(description); }

            if (CanBurn()) embed.WithFooter($"You may burn +{momentum} momentum for a {BurnResult().ToOutcomeString()} (see p. 32).");
            if (Action.Value + ActionAdds.Sum() > 10) embed.WithFooter(IronswornRollResources.OverMaxMessage);

            embed.AddField("Action Score", $"{Action.Value} + {String.Join(" + ", ActionAdds)} = {ActionScore}")
                .AddField("Challenge Dice", $"{Challenge1.Value}, {Challenge2.Value}");

            return embed;
        }
    }

    public class ProgressRollRandom : ActionRoll
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

        public new EmbedBuilder GetEmbed()
        {
            var embed = base.GetEmbed();

            if (!String.IsNullOrWhiteSpace(description)) embed.WithDescription(description);

            embed.AddField("Action Score", $"{Action.Value}")
                .AddField("Challenge Dice", $"{Challenge1.Value}, {Challenge2.Value}");

            return embed;
        }
    } 
}
