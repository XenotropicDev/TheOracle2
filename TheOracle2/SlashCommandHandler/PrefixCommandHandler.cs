using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;

namespace TheOracle2
{
    /// <summary>
    /// This class was directly copied from the old oracle bot. Once discord has better support for referencing messages this can removed, and the things depending on it can be converted over to application commands
    /// </summary>
    public class PrefixCommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private IServiceProvider _service = null;

        public PrefixCommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        public async Task InstallCommandsAsync(IServiceProvider service = null)
        {
            _service = service;
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _service);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasStringPrefix("! ", ref argPos) ||
                    message.HasCharPrefix('!', ref argPos) ||
                    message.HasMentionPrefix(_client.CurrentUser, ref argPos) ||
                    message.ReferencedMessage != null && message.ReferencedMessage.Author.Id == _client.CurrentUser.Id
                ) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Commands    {message.Author} entered the command: {message.Content}");

            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _service);

            if (!result.IsSuccess)
            {
                if (result.Error == CommandError.UnknownCommand) return;

                var commandSearch = _commands.Search(context, argPos);
                var triggeredCommand = commandSearch.Commands.FirstOrDefault();
                if (commandSearch.Commands.Count > 0 && triggeredCommand.Command.Name == "Roll" && result.Error == CommandError.BadArgCount) return;

                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
