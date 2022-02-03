namespace TheOracle2
{
    /// <summary>
    /// An interface for adapting objects into postable discord messages
    /// </summary>
    public interface IDiscordEntity
    {
        Embed[] GetEmbeds();

        MessageComponent GetComponents();

        bool IsEphemeral { get; set; }
    }
}
