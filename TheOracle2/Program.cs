using Discord;
using Discord.Commands;
using Discord.Net;
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
                client.Ready += Client_Ready;
                logger = services.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();

                var token = Environment.GetEnvironmentVariable("DiscordToken");
                if (token == null)
                {
                    token = services.GetRequiredService<IConfigurationRoot>().GetSection("DiscordToken").Value;
                }

                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();

                var commandHandler = services.GetRequiredService<SlashCommandHandler>();
                commandHandler.LoadFromAssembly(Assembly.GetEntryAssembly(), services);

                await client.SetGameAsync($"!Help | v{Assembly.GetEntryAssembly().GetName().Version}", "", ActivityType.Playing).ConfigureAwait(false);

                client.InteractionCreated += Client_InteractionCreated;
                client.InteractionCreated += commandHandler.Client_InteractionCreated;

                var context = services.GetRequiredService<EFContext>();

                await RecreateDB(context);
                context.Database.EnsureCreated();

                var gi = context.GameItems.First();
                var user = OracleGuild.GetGuild(756890506830807071, context);

                await Task.Delay(Timeout.Infinite);
            }
        }

        private static async Task RecreateDB(EFContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var gameItem = new GameItem() { GameItemId = 1, Name = "core" };
            context.GameItems.Add(gameItem);

            OracleGuild contentReg = new OracleGuild() { OracleGuildId = 756890506830807071 };
            contentReg.GameItems.Add(gameItem);
            context.OracleGuilds.Add(contentReg);

            var assetsJson = File.ReadAllText("GameData\\assets.json");
            var assets = JsonConvert.DeserializeObject<AssetRoot>(assetsJson);
            foreach (var asset in assets.Assets)
            {
                context.GameAssets.Add(asset);
            }

            await context.SaveChangesAsync();
        }

        private async Task Client_Ready()
        {
            try
            {
                //var commandHandler = _services.GetRequiredService<SlashCommandHandler>();
                //await commandHandler.InstallCommandsAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"{nameof(Client_Ready)} threw an error: {ex}");
            }

        }

        private async Task Client_InteractionCreated(SocketInteraction interaction)
        {
            // Checking the type of this interaction
            switch (interaction)
            {
                // Button clicks/selection dropdowns
                case SocketMessageComponent componentInteraction:
                    await MessageComponentHandler(componentInteraction);
                    break;

                default:
                    break;
            }
        }

        private async Task MessageComponentHandler(SocketMessageComponent interaction)
        {
            // Get the custom ID 
            var customId = interaction.Data.CustomId;
            // Get the user
            var user = (SocketGuildUser)interaction.User;
            // Get the guild
            var guild = user.Guild;

            // Respond with the update message. This edits the message which this component resides.
            await interaction.UpdateAsync(msgProps => msgProps.Content = $"Clicked {interaction.Data.CustomId}!");

            // Also you can followup with a additional messages
            await interaction.FollowupAsync($"Clicked {interaction.Data.CustomId}!", ephemeral: true);

            // If you are using selection dropdowns, you can get the selected label and values using these
            var selectedLabel = ((SelectMenu)interaction.Message.Components.First().Components.First()).Options.FirstOrDefault(x => x.Value == interaction.Data.Values.FirstOrDefault())?.Label;
            var selectedValue = interaction.Data.Values.First();
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
            var clientConfig = new DiscordSocketConfig { MessageCacheSize = 100, LogLevel = LogSeverity.Info, AlwaysAcknowledgeInteractions = false };
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