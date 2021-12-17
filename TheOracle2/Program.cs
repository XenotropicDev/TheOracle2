global using Discord;
global using Microsoft.Extensions.DependencyInjection;
global using System.Text.Json.Serialization;
global using System.Text.Json;
global using System.Linq;

using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OracleData;
using System.Reflection;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

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

            var context = services.GetRequiredService<EFContext>();

            //await RecreateDB(context).ConfigureAwait(true);
            context.Database.EnsureCreated();

            Console.WriteLine($"Starting TheOracle v{Assembly.GetEntryAssembly().GetName().Version}");
            client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync;
            logger = services.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();

            var token = Environment.GetEnvironmentVariable("DiscordToken");
            if (token == null)
            {
                token = services.GetRequiredService<IConfigurationRoot>().GetSection("DiscordToken").Value;
            }

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            client.Ready += ClientReady;



            await client.SetGameAsync("TheOracle v2 - Alpha", "", ActivityType.Playing).ConfigureAwait(false);


            await Task.Delay(Timeout.Infinite);
        }
    }

    private async Task ClientReady()
    {
        try
        {
            var handler = _services.GetRequiredService<SlashCommandHandler>();

            var commandHandler = _services.GetRequiredService<SlashCommandHandler>();
            commandHandler.LoadFromAssembly(Assembly.GetEntryAssembly(), _services);

            var refCommands = _services.GetRequiredService<ReferencedMessageCommandHandler>();
            refCommands.AddCommandHandler(client);

            interactionService = new InteractionService(client, new InteractionServiceConfig() {DefaultRunMode = Discord.Interactions.RunMode.Async });

            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            //await client.BulkOverwriteGlobalApplicationCommandsAsync(Array.Empty<ApplicationCommandProperties>());
#if DEBUG
            await interactionService.RegisterCommandsToGuildAsync(756890506830807071, false);
            await interactionService.RegisterCommandsToGuildAsync(916381023766470747, false);
#else
            //await interactionService.RegisterCommandsGloballyAsync();
#endif
            await handler.InstallCommandsAsync(_services, false);

            //todo: does this help the missed heartbeat, and if so why?
            //client.Disconnected += _ => client.StartAsync();

            //client.InteractionCreated += async (arg) =>
            //{
            //if (arg is SocketSlashCommand slashCommand && slashCommand.CommandName == "delay")
            //{
            //var cmd = new DelayCommand(slashCommand);
            //await cmd.DelayTest().ConfigureAwait(false);
            //}
            //};

            client.InteractionCreated += async (arg) =>
            {
                bool useInteractionService = true;
                switch (arg)
                {
                    case SocketSlashCommand slash:
                        logger.LogInformation($"{slash.User.Username} triggered slash command: {slash.CommandName}");
                        if (!interactionService.SlashCommands.Any(cmd => cmd.Name == slash.CommandName || cmd.Module.SlashGroupName == slash.CommandName))
                            useInteractionService = false;
                        if (!useInteractionService) await commandHandler.ExecuteAsync(slash, _services).ConfigureAwait(false);
                        break;

                    case SocketMessageComponent component:
                        logger.LogInformation($"{component.User.Username} triggered  message component: {component.Data.CustomId}");
                        if (!interactionService.ComponentCommands.Any(cmd => cmd.Name == component.Data.CustomId))
                            useInteractionService = false;
                        break;

                    default:
                        break;
                }

                try
                {
                    if (useInteractionService)
                    {
                        var ctx = new SocketInteractionContext(client, arg);
                        await interactionService.ExecuteCommandAsync(ctx, _services).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                }
            };
        }
        catch (Discord.Net.HttpException ex)
        {
            string json = JsonSerializer.Serialize(ex.Errors);
            logger.LogError(ex.Message, ex);
            logger.LogError(json);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
        }
    }

    private static async Task RecreateDB(EFContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        OracleGuild contentReg = new OracleGuild() { OracleGuildId = 756890506830807071 };

        var baseDir = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Data");
        var file = baseDir.GetFiles("assets.json").FirstOrDefault();

        string text = file.OpenText().ReadToEnd();
        var root = JsonSerializer.Deserialize<AssetRoot>(text);

        foreach (var asset in root.Assets)
        {
            asset.OracleGuilds.Add(contentReg);
            context.Assets.Add(asset);
        }

        file = baseDir.GetFiles("moves.json").FirstOrDefault();

        text = file.OpenText().ReadToEnd();
        var moveRoot = JsonSerializer.Deserialize<MovesInfo>(text);

        foreach (var move in moveRoot.Moves)
        {
            move.OracleGuilds.Add(contentReg);
            context.Moves.Add(move);
        }

        await context.SaveChangesAsync();
    }

    private Task LogAsync(LogMessage msg)
    {
        using (logger.BeginScope("[scope is enabled]"))
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    logger.LogCritical(msg.Message, msg.Exception);
                    break;

                case LogSeverity.Error:
                    logger.LogError(msg.Message, msg.Exception);
                    break;

                case LogSeverity.Warning:
                    logger.LogWarning(msg.Message, msg.Exception);
                    break;

                case LogSeverity.Info:
                    logger.LogInformation(msg.Message, msg.Exception);
                    break;

                case LogSeverity.Verbose:
                    logger.LogDebug(msg.Message, msg.Exception);
                    break;

                case LogSeverity.Debug:
                    logger.LogDebug(msg.Message, msg.Exception);
                    break;

                default:
                    break;
            }
        }
        return Task.CompletedTask;
    }

    private ServiceProvider ConfigureServices(DiscordSocketClient client = null, CommandService command = null)
    {
        var clientConfig = new DiscordSocketConfig { MessageCacheSize = 100, LogLevel = LogSeverity.Info,  };
        client ??= new DiscordSocketClient(clientConfig);

        var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("token.json", optional: true, reloadOnChange: true)
            .Build();

        return new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton(config)
            .AddSingleton(new SlashCommandHandler(client, null))
            .AddSingleton<ReferencedMessageCommandHandler>()
            .AddDbContext<EFContext>()
            .AddLogging(builder => builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = false;
                options.SingleLine = true;
                options.TimestampFormat = "hh:mm:ss ";
            }))
            .BuildServiceProvider();
    }
}