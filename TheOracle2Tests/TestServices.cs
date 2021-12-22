using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2.UserContent;

namespace TheOracle2Tests
{
    public class TestServices
    {
        public static IServiceProvider GetServices()
        {
            return new ServiceCollection()
                .AddDbContext<EFContext>()
                .AddSingleton<Random>()
                .AddLogging(builder => {
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
