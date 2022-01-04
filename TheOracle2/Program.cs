global using Discord;
global using Microsoft.Extensions.DependencyInjection;
global using System.Linq;
global using Newtonsoft.Json;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using TheOracle2.UserContent;
using System.Diagnostics;

namespace TheOracle2;

internal class Program
{
    private DiscordSocketClient client;
    private ServiceProvider _services;
    private ILogger<Program> logger;
    private InteractionService interactionService;

    public static void Main(string[] args)
    => new Program().MainAsync().GetAwaiter().GetResult();

    public async Task MainAsync()
    {
        using (ServiceProvider services = ConfigureServices())
        {
            _services = services;

            await CheckDB();

            Console.WriteLine($"Starting TheOracle v{Assembly.GetEntryAssembly().GetName().Version}");
            client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync;
            logger = services.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();

            string token = GetToken();

            await client.LoginAsync(TokenType.Bot, token);
            client.Ready += ClientReady;
            await client.StartAsync();

            await client.SetGameAsync("TheOracle v2 - Alpha", "", ActivityType.Playing).ConfigureAwait(false);

            await Task.Delay(Timeout.Infinite);
        }
    }

    private async Task ClientReady()
    {
        //Todo: Do we need a different one time ready method? I'm not sure if this gets fired on a reconnect.
        try
        {
            var oracleCommandHandler = _services.GetRequiredService<SlashCommandHandler>();
            oracleCommandHandler.LoadFromAssembly(Assembly.GetEntryAssembly(), _services);

            var refCommands = _services.GetRequiredService<ReferencedMessageCommandHandler>();
            refCommands.AddCommandHandler(client);

            interactionService = new InteractionService(client, new InteractionServiceConfig() { DefaultRunMode = Discord.Interactions.RunMode.Async });
            interactionService.Log += LogAsync;

            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            //await client.BulkOverwriteGlobalApplicationCommandsAsync(Array.Empty<ApplicationCommandProperties>());
#if DEBUG
            foreach (var guild in GetDebugGuilds())
            {
                await interactionService.RegisterCommandsToGuildAsync(guild, true);
            }
#else
            await interactionService.RegisterCommandsGloballyAsync(deleteMissing: true);
#endif
            await oracleCommandHandler.InstallCommandsAsync(_services, false);

            client.InteractionCreated += async (arg) =>
            {
                switch (arg)
                {
                    case SocketSlashCommand slash:
                        logger.LogInformation($"{slash.User.Username} triggered slash command: {slash.CommandName}");
                        if (!interactionService.SlashCommands.Any(cmd => cmd.Name == slash.CommandName || cmd.Module.SlashGroupName == slash.CommandName))
                        {
                            await oracleCommandHandler.ExecuteAsync(slash, _services);
                        }
                        else
                        {
                            var slashCtx = new SocketInteractionContext(client, slash);
                            await interactionService.ExecuteCommandAsync(slashCtx, _services).ConfigureAwait(false);
                        }
                        break;

                    case SocketMessageComponent component:
                        logger.LogInformation($"{component.User.Username} triggered message component (global event): {component.Data.CustomId}");
                        var msgCtx = new SocketInteractionContext<SocketMessageComponent>(client, component);
                        await interactionService.ExecuteCommandAsync(msgCtx, _services).ConfigureAwait(false);
                        break;

                    case SocketAutocompleteInteraction auto:
                        logger.LogInformation($"{arg.User.Username} triggered an auto complete interaction for {auto.Data.CommandName}, value: {auto.Data.Current.Value}");
                        var autoCtx = new SocketInteractionContext(client, arg);
                        await interactionService.ExecuteCommandAsync(autoCtx, _services).ConfigureAwait(false);
                        break;

                    default:
                        logger.LogInformation($"{arg.User.Username} triggered unknown interaction {arg.GetType()}: {arg.Data}");
                        var ctx = new SocketInteractionContext(client, arg);
                        await interactionService.ExecuteCommandAsync(ctx, _services).ConfigureAwait(false);
                        break;
                }
            };
        }
        catch (Discord.Net.HttpException ex)
        {
            string json = JsonConvert.SerializeObject(ex.Errors);
            logger.LogError(ex.Message, ex);
            logger.LogError(json);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
        }
    }

    private Task LogAsync(LogMessage msg)
    {
        var message = msg.Message ?? msg.Exception.Message;
        using (logger.BeginScope("[scope is enabled]"))
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    logger.LogCritical(message, msg.Exception);
                    break;

                case LogSeverity.Error:
                    logger.LogError(message, msg.Exception);
                    break;

                case LogSeverity.Warning:
                    logger.LogWarning(message, msg.Exception);
                    break;

                case LogSeverity.Info:
                    logger.LogInformation(message, msg.Exception);
                    break;

                case LogSeverity.Verbose:
                    logger.LogDebug(message, msg.Exception);
                    break;

                case LogSeverity.Debug:
                    logger.LogDebug(message, msg.Exception);
                    break;

                default:
                    break;
            }
        }
        return Task.CompletedTask;
    }

    private ServiceProvider ConfigureServices(DiscordSocketClient client = null, CommandService command = null)
    {
        var clientConfig = new DiscordSocketConfig { MessageCacheSize = 100, LogLevel = LogSeverity.Info, };
        client ??= new DiscordSocketClient(clientConfig);

        var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("token.json", optional: true, reloadOnChange: true)
            .Build();

        return new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton(config)
            .AddSingleton(new SlashCommandHandler(client, null))
            .AddSingleton<ReferencedMessageCommandHandler>()
            .AddSingleton<Random>()
            .AddDbContext<EFContext>()
            .AddLogging(builder => 
                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = false;
                    options.SingleLine = true;
                    options.TimestampFormat = "hh:mm:ss ";
                })
                .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning)
            )
            
            .BuildServiceProvider();
    }

    private async Task CheckDB()
    {
        var context = _services.GetRequiredService<EFContext>();
#if DEBUG
        Console.WriteLine($"\nYou are debugging, do you want to recreate the database? (y/n)");
        if (Console.ReadKey(true).Key == ConsoleKey.Y) { Console.WriteLine("Rebuilding Database..."); await context.RecreateDB().ConfigureAwait(true); }
#endif

        if (!context.HasTables())
        {
            Console.WriteLine($"\nDatabase not found, do you want to create it? (y/n)");

            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                Console.WriteLine("Rebuilding Database...");
                await context.RecreateDB().ConfigureAwait(true);
            }
            else
            {
                Console.WriteLine($"Cannot continue without database.");
            }
        }
    }

    private string GetToken()
    {
        var token = Environment.GetEnvironmentVariable("DiscordToken");
        if (token == null)
        {
            token = _services.GetRequiredService<IConfigurationRoot>().GetSection("DiscordToken").Value;
        }

        if (token == null)
        {
            Console.WriteLine($"Couldn't find a discord token. Please enter it here. (It will be saved to the token.json file in your bin folder)");
            token = Console.ReadLine();

            var json = JsonConvert.SerializeObject(new { DiscordToken = token });
            File.WriteAllText("token.json", json);
        }

        return token;
    }

    public ulong[] GetDebugGuilds()
    {
        if (!File.Exists("debugGuilds.json"))
        {
            Console.WriteLine($"\nYou are running in debug and haven't configured any debug guilds.\nPlease enter at least one guild ID below.\nSeparate multiple guild IDs with commas.\nThey will be saved to the debugGuilds.json file in your bin folder, for later editing if needed.");
            var jsonArray = Console.ReadLine();

            List<ulong> guilds = new List<ulong>();
            var values = jsonArray.Split(',');

            foreach (var guildId in values)
            {
                if (!ulong.TryParse(guildId, out var id)) continue;
                guilds.Add(id);
            }

            var jsonSave = JsonConvert.SerializeObject(guilds);
            File.WriteAllText("debugGuilds.json", jsonSave);
        }

        var file = File.ReadAllText("debugGuilds.json");

        return JsonConvert.DeserializeObject<ulong[]>(file);
    }
}