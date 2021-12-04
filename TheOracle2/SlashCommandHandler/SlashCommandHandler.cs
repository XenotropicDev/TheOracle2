using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TheOracle2;

public class SlashCommandHandler
{
    private IServiceProvider _service;
    private readonly DiscordSocketClient _client;

    private ILogger<SlashCommandHandler> _logger;

    public SlashCommandHandler(DiscordSocketClient client, ILogger<SlashCommandHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task SlashCommandEvent(SocketSlashCommand commandInteraction)
    {
        await ExecuteAsync(commandInteraction, _service);
    }

    public void LoadFromAssembly(Assembly assembly, IServiceProvider service = null)
    {
        _service = service;
        _logger = _service.GetRequiredService<ILoggerFactory>().CreateLogger<SlashCommandHandler>();
        foreach (var type in assembly.GetTypes())
        {
            foreach (var method in type.GetMethods().Where(m => m.GetCustomAttribute<OracleSlashCommandAttribute>() != null))
            {
                if (type.IsNotPublic)
                {
                    _logger.LogError($"{type} is not public");
                    continue;
                }
                if (!method.IsPublic)
                {
                    _logger.LogError($"{method} is not public");
                    continue;
                }

                if (method.GetCustomAttribute<AsyncStateMachineAttribute>() == null)
                {
                    _logger.LogError($"{method} must be async");
                    continue;
                }
                if (method.ReturnType != typeof(Task))
                {
                    _logger.LogError($"{method} must return a Task");
                    continue;
                }

                var commandInfo = method.GetCustomAttribute<OracleSlashCommandAttribute>();
                CommandList.Add(commandInfo.Name, method);
            }
        }
    }

    public Dictionary<string, MethodInfo> CommandList { get; set; } = new Dictionary<string, MethodInfo>();

    public async Task ExecuteAsync(SocketSlashCommand context, IServiceProvider services)
    {
        try
        {
            if (!CommandList.ContainsKey(context.Data.Name))
            {
                //await context.RespondAsync($"Unknown command {context.Data.Name}. Is it registered with the right name?", ephemeral:true);
                return;
            }
            var methodInfo = CommandList[context.Data.Name];
            var caller = ActivatorUtilities.CreateInstance(services, methodInfo.DeclaringType);
            (caller as ISlashCommand).Context = context;

            List<object> args = new List<object>();

            foreach (var arg in methodInfo.GetParameters())
            {
                var service = services.GetService(arg.ParameterType);
                if (service != null) args.Add(service);
            }

            if (args?.Count == 0) args = null;

            await (methodInfo.Invoke(caller, args?.ToArray()) as Task);
        }
        catch (HttpException ex)
        {
            string json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);
            _logger.LogError(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }

    public async Task InstallCommandsAsync(IServiceProvider services, bool DeleteExisting = true)
    {
        if (DeleteExisting)
        {
            await _client.Rest.DeleteAllGlobalCommandsAsync();
            _logger.LogInformation("Existing commands deleted");
        }

        var guild = _client.GetGuild(756890506830807071);

        var applicationCommands = new List<SlashCommandProperties>();

        foreach (var commandItem in CommandList)
        {
            var instance = ActivatorUtilities.CreateInstance(services, commandItem.Value.DeclaringType);
            if (instance is ISlashCommand command)
            {
                foreach (var builder in command.GetCommandBuilders())
                {
                    try
                    {
                        //Todo: remove the guild ID (used for rapid command deployment/updating)
                        applicationCommands.Add(builder.Build());
                        _logger.LogInformation($"Slash command for {builder.Name} created");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"failed to register slash command {builder.Name}", ex);
                    }
                }
            }
        }
        try
        {
#if DEBUG
                await _client.Rest.BulkOverwriteGuildCommands(applicationCommands.ToArray(), guild.Id);
#else
            await _client.Rest.BulkOverwriteGlobalCommands(applicationCommands.ToArray());
#endif
            _logger.LogInformation("Commands have been recreated");
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            Console.WriteLine(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }
}