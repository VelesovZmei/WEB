using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;
using Serilog;
using WEB.Data;
#if SEEDING
using WEB.FakeData;
#endif
using WEB.Initializer;
using WEB.Logging;


namespace WEB
{
    public class Program
    {
        public static async Task /*async Task void*/ Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
#if SEEDING
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<WebUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await AppRolesInitializer.CreateRoleAdminAsync(roleManager).ConfigureAwait(false);
                await AppRolesInitializer.CreateRoleClientAsync(roleManager).ConfigureAwait(false);
                await AdminInitialization.CreateAdminAsync(userManager).ConfigureAwait(false);

                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    //Fake data seeding
                    var fake = new FakeDataGenerator(userManager, logger);
                    await fake.SeedRandomAsync().ConfigureAwait(false);

                    // Afinn seeding
                    var context = services.GetService<ApplicationDbContext>();
                    string webrootContentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data");
                    var path = Path.Combine(webrootContentPath, "AFINN-ru.json");
                    await AfinnSeeding.SeedAsync(path, context, logger).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Main: An error \"{ex.Message}\" occurred while initial seeding database");
                }
            }
#endif
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseContentRoot(Directory.GetCurrentDirectory())
            //.ConfigureHostConfiguration(config =>
            //{
            //    config.AddEnvironmentVariables(prefix: "DOTNET_");
            //    if (args !=null)
            //    {
            //        config.AddCommandLine(args);
            //    }
            //})
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                // Adds loggers for console, debug, event source, and EventLog (Windows only)
                var section = hostingContext.Configuration.GetSection("Logging:Destination:Path");
                var path = Path.Combine(Directory.GetCurrentDirectory(), section.Value);
                var fileMask = hostingContext.Configuration.GetSection("Logging:Destination:FileMask").Value;
                logging
                    //.ClearProviders()
                    .AddConfiguration(section)
                    .AddFile(Path.Combine(path, $"{DateTime.UtcNow.ToString(fileMask)}.log"))
                    .AddEventSourceLogger();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
