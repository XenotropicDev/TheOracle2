namespace TheOracle2
{
    internal interface IDiscordEntity
    {
        Embed[] GetEmbeds();

        MessageComponent GetComponents();

        public Task<IMessage> GetDiscordMessage(IInteractionContext context);

        bool IsEphemeral { get; set; }
    }
}