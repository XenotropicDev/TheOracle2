using Discord.WebSocket;

namespace TheOracle2;

public class ReferencedMessageCommandHandler
{
    private DiscordSocketClient _client;
    public ReferencedMessageCommandHandler()
    {
        
    }

    public void AddCommandHandler(DiscordSocketClient client)
    {
        _client = client;
        _client.MessageReceived += HandleCommandAsync;
    }

    internal static async Task<bool> Process(SocketUserMessage message)
    {
        Uri url;
        bool messageHasUrl = Uri.TryCreate(message.Content, UriKind.Absolute, out url)
            && (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps);

        if ((message.Attachments.Count > 0 || messageHasUrl) && message.ReferencedMessage.Embeds.Count > 0)
        {
            if (!messageHasUrl) url = new Uri(message.Attachments.First().Url);
            var embed = (message.ReferencedMessage as IUserMessage).Embeds.First();
            await message.ReferencedMessage.ModifyAsync(msg => msg.Embed = embed.ToEmbedBuilder().WithThumbnailUrl(url.ToString()).Build());
            if (messageHasUrl) await message.DeleteAsync().ConfigureAwait(false);
            return true;
        }

        return false;
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        if (message.ReferencedMessage != null && message.ReferencedMessage.Author.Id == _client.CurrentUser.Id)
        {
            if (await Process(message)) return;
        }
        return;
    }
}
