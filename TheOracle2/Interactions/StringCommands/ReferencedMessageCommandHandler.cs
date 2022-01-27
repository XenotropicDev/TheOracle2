using Discord.Commands;
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

            var pcData = DbContext.PlayerCharacters.FirstOrDefault(pcData => pcData.MessageId == message.ReferencedMessage.Id);
            if (pcData != null)
            {
                pcData.Image = url.ToString();
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

public class MovePostModule : ModuleBase
{
    [Command("MovePost", RunMode = RunMode.Async)]
    [Alias("MoveMessage")]
    [Summary("Uses inline replies to move a message from one channel to another channel on the server. Make sure to Tag (#channel-name) the channel you want.")]
    [Remarks("")]
    public async Task MoveMessage(IGuildChannel mentionedChannel)
    {
        var message = Context.Message.ReferencedMessage;
        if (message == null || message.Author.Id != Context.Client.CurrentUser.Id)
        {
            await ReplyAsync($"I can only move my own messages.").ConfigureAwait(false);
            return;
        }

        if (mentionedChannel == Context.Channel) return;
        if (mentionedChannel is not ITextChannel)
        {
            await ReplyAsync("Mentioned channel must be a text channel");
            return;
        }

        var user = await mentionedChannel.GetUserAsync(Context.Client.CurrentUser.Id);
        var permissions = user?.GetPermissions(mentionedChannel);
        if (permissions == null || !permissions.HasValue || !permissions.Value.SendMessages || !permissions.Value.AddReactions)
        {
            await ReplyAsync("I don't have permissions for that channel.");
            return;
        }

        var components = ComponentBuilder.FromMessage(message);

        var newMsg = await (mentionedChannel as IMessageChannel).SendMessageAsync(message.Content, embed: message.Embeds.FirstOrDefault().ToEmbedBuilder().Build(), components: components.Build());

        await Task.Run(async () =>
        {
            var fetchedMessage = await Context.Channel.GetMessageAsync(message.Id); //This is to get around a bug in discord.net
            var reactionsToAdd = fetchedMessage.Reactions.Where(item => item.Value.IsMe).Select(item => item.Key);
            foreach (var reaction in reactionsToAdd)
            {
                await newMsg.AddReactionAsync(reaction);
                await Task.Delay(300); //Manual delay to avoid the rate limiter
            }

            if (message.IsPinned)
            {
                await newMsg.PinAsync();
            }

            await message.DeleteAsync();
        }).ConfigureAwait(false);
    }
}
