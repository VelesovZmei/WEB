using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Models;
using WEB.Data;

namespace WEB.Initializer
{
    public static class AfinnSeeding
    {
        public static async Task SeedAsync(string file, ApplicationDbContext context, ILogger logger)
        {
            logger.LogInformation("IMPORT of Afinn is started");

            List<Afinn> afinns;

            using (var reader = new StreamReader(file))
            {
                var json = reader.ReadToEnd();
                afinns = JsonSerializer.Deserialize<List<Afinn>>(json);
            }

            if (afinns?.Count > 0)
            {
                context.Afinns.AddRange(afinns);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            logger.LogInformation("IMPORT of Afinn is finished");
        }
    }
}
