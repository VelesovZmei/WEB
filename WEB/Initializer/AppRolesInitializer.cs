using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WEB.Initializer
{
    public static class AppRolesInitializer
    {
        public static async Task CreateRoleClientAsync(RoleManager<IdentityRole> roleManager)
        {
            var role = await roleManager.FindByNameAsync("Client").ConfigureAwait(false);
            if (role == null)
            {
                role = new IdentityRole("Client")
                {
                    NormalizedName = "CLIENT"
                };
                await roleManager.CreateAsync(role).ConfigureAwait(false);
            }
        }

        public static async Task CreateRoleAdminAsync(RoleManager<IdentityRole> roleManager)
        {
            var role = await roleManager.FindByNameAsync("Admin").ConfigureAwait(false);
            if (role == null)
            {
                role = new IdentityRole("Admin")
                {
                    NormalizedName = "ADMIN"
                };
                await roleManager.CreateAsync(role).ConfigureAwait(false);
            }
        }
    }
}
