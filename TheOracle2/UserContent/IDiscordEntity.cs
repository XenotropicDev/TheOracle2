namespace TheOracle2
{
    /// <summary>
    /// An interface for adapting objects into postable discord messages
    /// </summary>
    internal interface IDiscordEntity
    {
        Embed[] GetEmbeds();

        MessageComponent GetComponents();

        bool IsEphemeral { get; set; }
    }
}
