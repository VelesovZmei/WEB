using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using Serilog;
using WEB.Data;
using WEB.RSS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WEB.JWT;
using Hangfire;
using System.Net.Http;

namespace WEB
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<WebUser, IdentityRole>(options =>
             {
                 // Password settings
                 options.Password.RequiredLength = 8;
                 options.Password.RequiredUniqueChars = 4;
                 options.Password.RequireDigit = true;
                 options.Password.RequireLowercase = true;
                 options.Password.RequireNonAlphanumeric = true;
                 options.Password.RequireUppercase = true;

                 // Lockout settings
                 options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                 options.Lockout.MaxFailedAccessAttempts = 5;
                 options.Lockout.AllowedForNewUsers = true;

                 // User settings
                 // options.User.AllowedUserNameCharacters = "0123456789";
                 options.User.RequireUniqueEmail = false;

                 // SignIn settings
                 options.SignIn.RequireConfirmedEmail = false;
                 options.SignIn.RequireConfirmedPhoneNumber = false;
             })
                //.AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    // Cookie settings
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                    options.LoginPath = "/Identity/Account/Login";
                    options.LogoutPath = "/Identity/Account/Logout";
                    options.AccessDeniedPath = "/Identity/Account/Login";
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // укзывает, будет ли валидироваться издатель при валидации токена
                            ValidateIssuer = true,
                            // строка, представляющая издателя
                            ValidIssuer = JwtOptions.ISSUER,

                            // будет ли валидироваться потребитель токена
                            ValidateAudience = true,
                            // установка потребителя токена
                            ValidAudience = JwtOptions.AUDIENCE,
                            // будет ли валидироваться время существования
                            ValidateLifetime = true,

                            // установка ключа безопасности
                            IssuerSigningKey = JwtOptions.GetSymmetricSecurityKey(),
                            // валидация ключа безопасности
                            ValidateIssuerSigningKey = true,
                        };
                    });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireRoleAdmin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireRoleClient", policy => policy.RequireRole("Client"));
            });
            services.AddControllersWithViews();
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Identity", "/Account");
                //options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/ExternalLogin");
                //options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/ForgotPassword");
                //options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/ForgotPasswordConfirmation");
                options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Login");
                options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Logout");
                options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Register");
                //options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/ResetPassword");
                //options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/ResetPasswordConfirmation");
            });

            services.AddHttpClient(); // For Texterra
            services.AddScoped<RssReadService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // or this services.AddHttpContextAccessor();
            services.AddScoped<PasswordHasher<WebUser>>();


            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRouting();

            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
