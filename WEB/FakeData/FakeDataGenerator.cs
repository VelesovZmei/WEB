#if SEEDING
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Models;

namespace WEB.FakeData
{
    public class FakeDataGenerator
    {
        private readonly UserManager<WebUser> _userManager;
        private readonly ILogger log;

        public FakeDataGenerator(
            UserManager<WebUser> userManager,
            ILogger logger)
        {
            _userManager = userManager;
            log = logger;
        }

        public async Task SeedRandomAsync()
        {
            log.LogInformation("FakeDataGenerator: Fake data seeding is started");

            var fakeUsers = FakeUser.Generate(100);

            foreach (var item in fakeUsers)
            {
                var result = await _userManager
                    .CreateAsync(item, "E93119953a92=")
                    .ConfigureAwait(false);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(item, "Client").ConfigureAwait(false);
                }
            }

            log.LogInformation("FakeDataGenerator: Fake data seeding is finished");

            await Task.CompletedTask;
        }
    }
}
#endif
