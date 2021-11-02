using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Discord;
using Discord.Net;
using Newtonsoft.Json;

namespace TheOracle2
{
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
                foreach (var method in type.GetMethods().Where(m => m.GetCustomAttribute<SlashCommandAttribute>() != null))
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

                    var commandInfo = method.GetCustomAttribute<SlashCommandAttribute>();
                    CommandList.Add(commandInfo.Name, method);
                }
            }
        }

        public Dictionary<string, MethodInfo> CommandList { get; set; }  = new Dictionary<string,MethodInfo>();

        public async Task ExecuteAsync(SocketSlashCommand context, IServiceProvider services)
        {
            if (!CommandList.ContainsKey(context.Data.Name))
            {
                await context.RespondAsync($"Unknown command {context.Data.Name}. Is it registered with the right name?", ephemeral:true);
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
            
            if (!args.Any()) args = null;

            await (methodInfo.Invoke(caller, args?.ToArray()) as Task);
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
                await _client.Rest.BulkOverwriteGuildCommands(applicationCommands.ToArray(), guild.Id);
                _logger.LogInformation("Commands have been recreated");
            }
            catch (ApplicationCommandException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);
                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

        }

    }
}
