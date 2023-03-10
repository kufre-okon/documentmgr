using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace documentmgr
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
                 .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
                 .Build();
            //Initialize Logger
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
            try
            {
                Log.Information("Application Starting... in " + environment);

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseIISIntegration();
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseStartup<Startup>();
                });
    }
}