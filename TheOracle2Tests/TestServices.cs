global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using System;
global using System.Linq;
global using System.Threading.Tasks;
global using TheOracle2.UserContent;
global using TheOracle2Tests;
using Microsoft.Extensions.Logging;

namespace TheOracle2Tests
{
    public class TestServices
    {
        public static IServiceProvider GetServices()
        {
            return new ServiceCollection()
                .AddDbContext<EFContext>()
                .AddSingleton(new Random(0))
                .AddLogging(builder =>
                {
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = false;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    });
                    builder.SetMinimumLevel(LogLevel.Warning);
                })
                .BuildServiceProvider();
        }
    }
}