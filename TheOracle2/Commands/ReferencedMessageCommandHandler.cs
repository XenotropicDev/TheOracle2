using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using TheOracle2.UserContent;

namespace TheOracle2;

public class ReferencedMessageCommandHandler
{
    private DiscordSocketClient _client;
    private readonly ILogger<ReferencedMessageCommandHandler> logger;

    public EFContext DbContext { get; }

    public ReferencedMessageCommandHandler(EFContext dbContext, ILogger<ReferencedMessageCommandHandler> logger)
    {
        DbContext = dbContext;
        this.logger = logger;
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

            var pc = DbContext.PlayerCharacters.FirstOrDefault(pc => pc.MessageId == message.ReferencedMessage.Id);
            if (pc != null)
            {
                pc.Image = url.ToString();
                await DbContext.SaveChangesAsync();
            }

            try
            {
                var chan = message.Channel as IGuildChannel;
                var bot = await chan?.Guild.GetUserAsync(_client.CurrentUser.Id);

                if (messageHasUrl && bot?.GetPermissions(chan).Has(ChannelPermission.ManageMessages) == true) await message.DeleteAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError($"Couldn't delete URL reference post: {ex}");
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
            logger.LogInformation($"Received reference message command. Content: '{message.Content}', Attachments: {message.Attachments.Count}");
            if (await Process(message).ConfigureAwait(false)) return;
        }
        return;
    }
}
