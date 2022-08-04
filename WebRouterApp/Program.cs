using System;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Logger;
using WebRouterApp.Core.Data;

namespace WebRouterApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            BinanceLogLevels.Register();

            var host = CreateHostBuilder(args).Build();

            MigrateAndSeedDb(host.Services);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddLog4Net(log4NetConfigFile: "log4net.config");
                });
        }

        private static void MigrateAndSeedDb(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<TradeRouterDbContext>();

                // var environment = serviceScope.ServiceProvider.GetRequiredService<IHostEnvironment>();
                // if (environment.IsDevelopment())
                // {
                //     dbContext.Database.EnsureDeleted();
                //     dbContext.Database.EnsureCreated();
                // }

                dbContext.Database.Migrate();
            }
        }
    }
}
