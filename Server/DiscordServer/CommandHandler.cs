using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OracleCommands;

public class CommandHandler
{
    private readonly InteractionService _commands;
    private readonly IConfiguration _configuration;
    private readonly DiscordSocketClient _discord;
    private readonly IServiceProvider _services;
    private readonly ILogger<CommandHandler> logger;

    public CommandHandler(InteractionService commands, DiscordSocketClient discord, IConfiguration configuration, IServiceProvider services, ILogger<CommandHandler> logger)
    {
        _commands = commands;
        _discord = discord;
        _configuration = configuration;
        _services = services;
        this.logger = logger;
    }

    public async Task Initialize()
    {
        try
        {
            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);
            _discord.ButtonExecuted += ButtonExecuted;
            _discord.SelectMenuExecuted += SelectExecuted;
            _discord.UserCommandExecuted += UserCommandExecuted;
            _discord.AutocompleteExecuted += AutoCompleteExecuted;
            _discord.SlashCommandExecuted += SlashCommandExecuted;
            _discord.MessageCommandExecuted += MessageCommandExecuted;
            _discord.ModalSubmitted += ModalSubmitted;
            _discord.Ready += Ready;
            _commands.SlashCommandExecuted += _commands_SlashCommandExecuted;
            _commands.AutocompleteHandlerExecuted += _commands_AutocompleteHandlerExecuted;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task RegisterCommands()
    {
        try
        {
            await _commands.RegisterCommandsToGuildAsync(756890506830807071, true);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private Task _commands_AutocompleteHandlerExecuted(IAutocompleteHandler arg1, Discord.IInteractionContext arg2, IResult arg3)
    {
        return Task.CompletedTask;
    }

    private Task _commands_SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
    {
        return Task.CompletedTask;
    }

    private async Task AutoCompleteExecuted(SocketAutocompleteInteraction arg)
    {
        logger.LogDebug($"{arg.User.Username} is executing Autocomplete Interaction {arg.Data.CommandName} with id:{arg.Data.CommandId}");
        var ctx = new SocketInteractionContext(_discord, arg);
        await _commands.ExecuteCommandAsync(ctx, _services);
    }

    private async Task ButtonExecuted(SocketMessageComponent arg)
    {
        logger.LogInformation($"{arg.User.Username} is executing Button Interaction {arg.Data.CustomId} with value: '{arg.Data.Value}'.");
        var ctx = new SocketInteractionContext<SocketMessageComponent>(_discord, arg);
        await _commands.ExecuteCommandAsync(ctx, _services);
    }

    private async Task MessageCommandExecuted(SocketMessageCommand arg)
    {
        logger.LogInformation($"{arg.User.Username} is executing Message Command {arg.Data.Name} with value(s): '{string.Join(", ", arg.Data.Options)}'.");
        var ctx = new SocketInteractionContext<SocketMessageCommand>(_discord, arg);
        await _commands.ExecuteCommandAsync(ctx, _services);
    }

    private async Task ModalSubmitted(SocketModal arg)
    {
        logger.LogInformation($"{arg.User.Username} is submitting a Modal for {arg.Data.CustomId} with {arg.Data.Components.Count} Components.");
        var ctx = new SocketInteractionContext<SocketModal>(_discord, arg);
        await _commands.ExecuteCommandAsync(ctx, _services);
    }

    private async Task Ready()
    {
        await RegisterCommands();
        _discord.Ready -= Ready;
    }

    private async Task SelectExecuted(SocketMessageComponent arg)
    {
        logger.LogInformation($"{arg.User.Username} is executing Select Interaction {arg.Data.CustomId} with value(s): '{string.Join(", ", arg.Data.Values ?? Array.Empty<string>())}'.");
        var ctx = new SocketInteractionContext<SocketMessageComponent>(_discord, arg);
        await _commands.ExecuteCommandAsync(ctx, _services);
    }

    private async Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        logger.LogInformation($"{arg.User.Username} is executing Slash Command {arg.Data.Name} with value(s): '{string.Join(", ", arg.Data.Options)}'.");
        var ctx = new SocketInteractionContext<SocketSlashCommand>(_discord, arg);
        await _commands.ExecuteCommandAsync(ctx, _services);
    }

    private async Task UserCommandExecuted(SocketUserCommand arg)
    {
        logger.LogInformation($"{arg.User.Username} is executing User Command {arg.Data.Name}");
        var ctx = new SocketInteractionContext<SocketUserCommand>(_discord, arg);
        await _commands.ExecuteCommandAsync(ctx, _services);
    }
}
