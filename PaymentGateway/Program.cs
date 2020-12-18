using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;

namespace PaymentGateway.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(theme: AnsiConsoleTheme.Grayscale)
                .WriteTo.Debug()
                .WriteTo.File($"Logs\\PaymentGateway.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            try
            {
                Log.Information($"PaymentGateway is starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal("Applcation startup failure, error message: {@ex}", ex);
                Environment.Exit(0);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .UseSerilog();
    }
}
