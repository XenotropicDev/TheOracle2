using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OracleData;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheOracle2;
using TheOracle2.UserContent;

namespace TheOracle
{
    internal class Program
    {
        private DiscordSocketClient client;
        private ServiceProvider _services;
        private ILogger<Program> logger;

        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using (ServiceProvider services = ConfigureServices())
            {
                _services = services;
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

                await client.SetGameAsync($"!Help | v{Assembly.GetEntryAssembly().GetName().Version}", "", ActivityType.Playing).ConfigureAwait(false);

                client.SlashCommandExecuted += commandHandler.SlashCommandEvent;

                var context = services.GetRequiredService<EFContext>();

                await RecreateDB(context);
                context.Database.EnsureCreated();

                var user = OracleGuild.GetGuild(756890506830807071, context);

                await Task.Delay(Timeout.Infinite);
            }
        }

        private async Task ClientReady()
        {
            await client.BulkOverwriteGlobalApplicationCommandsAsync(Array.Empty<ApplicationCommandProperties>());

            var handler = _services.GetRequiredService<SlashCommandHandler>();
            await handler.InstallCommandsAsync();
        }

        private static async Task RecreateDB(EFContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            OracleGuild contentReg = new OracleGuild() { OracleGuildId = 756890506830807071 };

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
            var clientConfig = new DiscordSocketConfig { MessageCacheSize = 100, LogLevel = LogSeverity.Info };
            client ??= new DiscordSocketClient(clientConfig);

            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("token.json", optional: true, reloadOnChange: true)
                .Build();

            return new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(config)
                .AddSingleton(new SlashCommandHandler(client, null))
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
}