using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using Models;
using WEB.JWT;

namespace WEB.Controllers
{
    public class JwtAuthController : Controller
    {
        // TODO: refresh-token
        private readonly UserManager<WebUser> _userManager;

        public JwtAuthController(UserManager<WebUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("/token")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Token(string username)
        {
            var identity = await GetIdentityAsync(username);

            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: JwtOptions.ISSUER,
                audience: JwtOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(JwtOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(
                    JwtOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Json(response);
        }

        private async Task<ClaimsIdentity> GetIdentityAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType,
                        await _userManager.IsInRoleAsync(user, "Admin")
                        ? "Admin"
                        : "Client")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Token", 
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            // User not found
            return null;
        }
    }
}