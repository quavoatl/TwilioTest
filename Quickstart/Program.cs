using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Quickstart.Controllers;

namespace Quickstart
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .ConfigureLogging((context,builder) =>
            {
                builder.AddApplicationInsights("a4f633cd-b4b4-4cdb-b2c6-6e99e2b55e23");
                builder.AddFilter<ApplicationInsightsLoggerProvider>(typeof(Program).FullName, LogLevel.Trace);
                builder.AddFilter<ApplicationInsightsLoggerProvider>(typeof(VoiceController).FullName, LogLevel.Trace);
            });
    }
}
