global using System.Linq;
global using Discord;
global using Microsoft.Extensions.DependencyInjection;
global using Newtonsoft.Json;

using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    public static async Task Main()
    {
        using var services = ConfigureServices();

        var client = services.GetRequiredService<DiscordSocketClient>();
        var commands = services.GetRequiredService<InteractionService>();
        var config = services.GetRequiredService<IConfiguration>();
        var handler = services.GetRequiredService<CommandHandler>();
        var db = services.GetRequiredService<ApplicationContext>();

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
        Console.WriteLine(msg.ToString());
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
