using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Models;

namespace WEB.Initializer
{
    public static class AdminInitialization
    {
        public static async Task CreateAdminAsync(
           UserManager<WebUser> userManager)
        {
            var user = await userManager.FindByNameAsync("Genesis").ConfigureAwait(false);
            if (user == null)
            {
                user = new WebUser("Genesis")
                {
                    Email = "gar0tarkhan@gmail.com"
                };

                IdentityResult result = await userManager.CreateAsync(user, "Asd@1130").ConfigureAwait(false);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin").ConfigureAwait(false);
                    
                }
            }
        }
    }
}
