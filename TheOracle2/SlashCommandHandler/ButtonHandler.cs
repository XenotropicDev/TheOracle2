using System.Reflection;
using System.Runtime.CompilerServices;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace TheOracle2;

public class ButtonHandler
{
    private IServiceProvider _service;

    private ILogger<ButtonHandler> _logger;

    public ButtonHandler(ILogger<ButtonHandler> logger, IServiceProvider services)
    {
        _logger = logger;
        _service = services;
    }

    public void LoadFromAssembly(Assembly assembly, IServiceProvider service = null)
    {
        _service = service;
        _logger = _service.GetRequiredService<ILoggerFactory>().CreateLogger<ButtonHandler>();
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

                var commandInfo = method.GetCustomAttribute<ButtonActionAttribute>();
                ButtonActions.Add(commandInfo.ButtonId, method);
            }
        }
    }

    public Dictionary<string, MethodInfo> ButtonActions { get; set; } = new Dictionary<string, MethodInfo>();

    public async Task Handler(SocketMessageComponent context)
    {
        if (!ButtonActions.ContainsKey(context.Data.CustomId))
        {
            await context.RespondAsync($"Unknown button {context.Data.CustomId}. Is it registered with the right name?", ephemeral: true);
            return;
        }

        var methodInfo = ButtonActions[context.Data.CustomId];
        var caller = ActivatorUtilities.CreateInstance(_service, methodInfo.DeclaringType);

        List<object> args = new List<object>();

        foreach (var arg in methodInfo.GetParameters())
        {
            var service = _service.GetService(arg.ParameterType);
            if (service != null) args.Add(service);
        }

        if (args?.Count == 0) args = null;

        await (methodInfo.Invoke(caller, args?.ToArray()) as Task);
    }
}
