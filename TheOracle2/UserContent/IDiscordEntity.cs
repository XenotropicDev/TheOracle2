namespace TheOracle2
{
    internal interface IDiscordEntity
    {
        Embed[] GetEmbeds();

        MessageComponent GetComponents();

        string GetDiscordMessage();
        bool IsEphemeral { get; set; }
    }
}