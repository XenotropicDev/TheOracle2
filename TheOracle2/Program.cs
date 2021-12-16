global using Discord;
global using Microsoft.Extensions.DependencyInjection;

using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

            var commandHandler = services.GetRequiredService<SlashCommandHandler>();
            commandHandler.LoadFromAssembly(Assembly.GetEntryAssembly(), services);

            await client.SetGameAsync("TheOracle v2 - Alpha", "", ActivityType.Playing).ConfigureAwait(false);

            client.SlashCommandExecuted += commandHandler.SlashCommandEvent;

            await Task.Delay(Timeout.Infinite);
        }
    }

    private async Task ClientReady()
    {
        try
        {
            var handler = _services.GetRequiredService<SlashCommandHandler>();

            var refCommands = _services.GetRequiredService<ReferencedMessageCommandHandler>();
            refCommands.AddCommandHandler(client);

            interactionService = new InteractionService(client);
            //interactionService.AddTypeConverter<Move>(new MoveReferenceConverter(_services));

            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            //await client.BulkOverwriteGlobalApplicationCommandsAsync(Array.Empty<ApplicationCommandProperties>());
#if DEBUG
            await interactionService.RegisterCommandsToGuildAsync(756890506830807071, false);
            await interactionService.RegisterCommandsToGuildAsync(916381023766470747, false);
            //await interactionService
#else
            //await interactionService.RegisterCommandsGloballyAsync();
#endif
            await handler.InstallCommandsAsync(_services, false);

            client.InteractionCreated += async (arg) =>
            {
                //make sure we are responsible for handling the command.
                switch (arg)
                {
                    case SocketSlashCommand slash:
                        if (!interactionService.SlashCommands.Any(cmd => cmd.Name == slash.CommandName || cmd.Module.SlashGroupName == slash.CommandName))
                            return;
                        break;

                    case SocketMessageComponent component:
                        if (!interactionService.ComponentCommands.Any(cmd => cmd.Name == component.Data.CustomId))
                            return;
                        break;

                    default:
                        break;
                }

                try
                {
                    var ctx = new SocketInteractionContext(client, arg);
                    await interactionService.ExecuteCommandAsync(ctx, _services);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                    // response, or at least let the user know that something went wrong during the command execution.
                    //if (arg.Type == InteractionType.ApplicationCommand)
                    //    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }
            };

            //client.ButtonExecuted += async (interaction) =>
            //{
            //    var ctx = new SocketInteractionContext<SocketMessageComponent>(client, interaction);
            //    await interactionService.ExecuteCommandAsync(ctx, _services);
            //};
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

    private static async Task RecreateDB(EFContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        OracleGuild contentReg = new OracleGuild() { OracleGuildId = 756890506830807071 };

        var baseDir = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Data");
        var file = baseDir.GetFiles("assets.json").FirstOrDefault();

        string text = file.OpenText().ReadToEnd();
        var root = JsonConvert.DeserializeObject<AssetRoot>(text);

        foreach (var asset in root.Assets)
        {
            asset.OracleGuilds.Add(contentReg);
            context.Assets.Add(asset);
        }

        file = baseDir.GetFiles("moves.json").FirstOrDefault();

        text = file.OpenText().ReadToEnd();
        var moveRoot = JsonConvert.DeserializeObject<MovesInfo>(text);

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
        var clientConfig = new DiscordSocketConfig { MessageCacheSize = 100, LogLevel = LogSeverity.Debug };
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