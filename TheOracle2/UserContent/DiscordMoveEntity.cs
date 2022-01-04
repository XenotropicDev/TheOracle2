using TheOracle2.ActionRoller;
using TheOracle2.DataClasses;

namespace TheOracle2.UserContent
{
    internal class DiscordMoveEntity : IDiscordEntity
    {
        public DiscordMoveEntity(Move move, bool ephemeral = false)
        {
            Move = move;
            IsEphemeral = ephemeral;
        }

        public bool IsEphemeral { get; set; }
        public Move Move { get; }

        public MessageComponent GetComponents()
        {
            return null;
        }

        public string GetDiscordMessage()
        {
            return null;
        }

        public Embed[] GetEmbeds()
        {
            return new Embed[] {new EmbedBuilder()
                .WithAuthor(Move.Category)
                .WithTitle(Move.Name)
                .WithDescription(Move.Text).Build() };
        }
    }
}