using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Surveys.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Surveys
{
    /// <summary>
    /// Klasa konfiguruj¹ca œrodowisko aplikacji i sam¹ aplikacjê na starcie programu.
    /// </summary>
    public class Startup
    {
        public static bool testing = false;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Funkcja dodaj¹ca i konfiguruj¹ca dynamicznie us³ugi rozszerzaj¹ce aplikacjê
        /// </summary>
        /// <param name="services">kolekcja us³ug u¿ywanych standardowo przez ASP.NET Core</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        options =>
        {
            options.LoginPath = new PathString("/Home/Login/");
            options.AccessDeniedPath = new PathString("/Home/Forbidden/");
        });
            services.AddDbContext<surveyContext>(options =>
            {
                if (testing) options.UseInMemoryDatabase("survey");//options.UseSqlite(CreateInMemoryDatabase());
                else
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddControllersWithViews();

            //for claim-based authorisation
            services.AddAuthorization(options =>
            {
                {
                    options.AddPolicy("Admin", policy =>
                    {
                        policy.RequireClaim(ClaimTypes.Role, "Admin");
                    });
                    options.AddPolicy("Owner", policy =>
                    {
                        policy.RequireClaim(ClaimTypes.Role, "Owner");
                    });
                    options.AddPolicy("User", policy =>
                    {
                        policy.RequireClaim(ClaimTypes.Role, "User");
                    });
                }
            });
            services.AddScoped<IAuthService, AuthService>();
        }
        /// <summary>
        /// Funkcja konfiguruj¹ca ¿¹dania HTTP.
        /// </summary>
        /// <param name="app">klasa buduj¹ca aplikacjê</param>
        /// <param name="env">klasa reprezentuj¹ca œrodowisko aplikacji internetowej</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestLocalization();
            var currentCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            currentCulture.NumberFormat.NumberDecimalSeparator = ".";
            //currentCulture.NumberFormat.NumberGroupSeparator = " ";
            currentCulture.NumberFormat.CurrencyDecimalSeparator = ".";

            Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentCulture;
            app.UseAuthentication();
            //if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else
            {
                //  app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
