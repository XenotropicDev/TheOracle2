using Discord.WebSocket;
using TheOracle2.UserContent;

namespace TheOracle2;

public class ReferencedMessageCommandHandler
{
    private DiscordSocketClient _client;

    public EFContext DbContext { get; }

    public ReferencedMessageCommandHandler(EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public void AddCommandHandler(DiscordSocketClient client)
    {
        _client = client;
        _client.MessageReceived += HandleCommandAsync;
    }

    internal async Task<bool> Process(SocketUserMessage message)
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

            var pc = DbContext.PlayerCharacters.FirstOrDefault(pc => pc.MessageId == message.Id);
            if (pc != null)
            {
                pc.Image = url.ToString();
                await DbContext.SaveChangesAsync();
            }

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
            if (await Process(message).ConfigureAwait(false)) return;
        }
        return;
    }
}
