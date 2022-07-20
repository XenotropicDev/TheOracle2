global using System.Linq;
global using Discord;
global using Microsoft.Extensions.DependencyInjection;
global using Newtonsoft.Json;

using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;
using OracleCommands;
using Server.Data;
using Server.OracleRoller;
using Server.DiscordServer;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheOracle2;

class OracleServer
{
    internal static Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger(); //This gets overwritten by the ConfiguredServices

    public static async Task Main()
    {
        using var services = ConfigureServices();

        var client = services.GetRequiredService<DiscordSocketClient>();
        var commands = services.GetRequiredService<InteractionService>();
        var config = services.GetRequiredService<IConfiguration>();
        var handler = services.GetRequiredService<CommandHandler>();
        var db = services.GetRequiredService<ApplicationContext>();

        //db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        await handler.Initialize().ConfigureAwait(false);

        await client.LoginAsync(Discord.TokenType.Bot, GetToken(services)).ConfigureAwait(false);

        //client.Ready += ClientReady;
        client.Log += LogAsync;
        commands.Log += LogAsync;

        await client.StartAsync();

        await client.SetGameAsync("TheOracle v2.1 - Alpha", "", ActivityType.Playing).ConfigureAwait(false);

        await Task.Delay(Timeout.Infinite);
    }

    private static Task LogAsync(LogMessage msg)
    {
        if (msg.Exception?.GetType() == typeof(System.TimeoutException))
        {
            logger.Warning(msg.Exception.Message);
            return Task.CompletedTask;
        }

        switch (msg.Severity)
        {
            case LogSeverity.Critical:
                logger.Fatal(msg.Exception, msg.Message);
                break;
            case LogSeverity.Error:
                logger.Error(msg.Exception, msg.Message);
                break;
            case LogSeverity.Warning:
                logger.Warning(msg.Exception, msg.Message);
                break;
            case LogSeverity.Info:
                logger.Information(msg.Exception, msg.Message);
                break;
            case LogSeverity.Verbose:
                logger.Verbose(msg.Exception, msg.Message);
                break;
            case LogSeverity.Debug:
                logger.Debug(msg.Exception, msg.Message);
                break;
            default:
                break;
        }
        return Task.CompletedTask;
    }

    static ServiceProvider ConfigureServices()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("token.json", optional: true, reloadOnChange: true)
            .AddJsonFile("dbSettings.json", optional: false, reloadOnChange: true)
            .SetBasePath(Directory.GetCurrentDirectory())
            .Build();

        var dbConn = config.GetSection("dbConnectionString").Value;
        var dbPass = config.GetSection("dbPassword").Value;
        var dbConnBuilder = new NpgsqlConnectionStringBuilder(dbConn) {Password = dbPass };

        var interactionServiceConfig = new InteractionServiceConfig()  { UseCompiledLambda = true };
        var logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                    .CreateLogger();

        return new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(interactionServiceConfig)
            .AddSingleton<InteractionService>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<Random>()
            .AddSingleton<IOracleRoller, RandomOracleRoller>()
            .AddSingleton<IOracleRepository, JsonOracleRepository>()
            .AddSingleton<IMoveRepository, JsonMoveRepository>()
            .AddSingleton<IEmoteRepository, HardCodedEmoteRepo>()
            .AddLogging(builder => builder.AddSerilog(logger)
                .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning)
                .AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning)
                )
            .AddDbContext<ApplicationContext>(options => options.UseNpgsql(dbConnBuilder.ConnectionString))
            .BuildServiceProvider();
    }

    private static string GetToken(IServiceProvider services)
    {
        var token = Environment.GetEnvironmentVariable("DiscordToken");
        if (token == null)
        {
            token = services.GetRequiredService<IConfiguration>().GetSection("DiscordToken").Value;
        }

        if (token == null)
        {
            Console.WriteLine($"Couldn't find a discord token. Please enter it here. (It will be saved to the token.json file in your bin folder)");
            token = Console.ReadLine();

            var json = JsonConvert.SerializeObject(new { DiscordToken = token });
            File.WriteAllText("token.json", json);
        }

        return token ?? String.Empty;
    }

}
