using System;
using System.Linq;
using System.Runtime.InteropServices;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Services;
using CourseSchedulingSystem.Services.TestingAuthentication;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseSchedulingSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            CurrentEnvironment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment CurrentEnvironment { get; }

        public bool IsTestingEnvironment => CurrentEnvironment.IsEnvironment("Testing");

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var authenticationBuilder = services.AddAuthentication(options =>
            {
                // If the application is running in a testing environment, use the TestingAuthentication scheme
                if (IsTestingEnvironment)
                {
                    options.DefaultScheme = TestingAuthenticationDefaults.IdentityFallbackScheme;
                    options.DefaultAuthenticateScheme = TestingAuthenticationDefaults.IdentityFallbackScheme;
                    options.DefaultChallengeScheme = TestingAuthenticationDefaults.IdentityFallbackScheme;
                }
            });

            // If the application is running in a testing environment, use the TestingAuthentication scheme
            if (IsTestingEnvironment)
            {
                authenticationBuilder.AddTesting<Guid, ApplicationUser, ApplicationRole>();
            }

            // Add WS-Federation auth
            authenticationBuilder.AddWsFederation(AuthenticationConstants.WinthropScheme, "Sign in with Winthrop",
                options =>
                {
                    options.MetadataAddress = Configuration.GetValue<string>("WsFederation:MetadataAddress");
                    options.Wtrealm = Configuration.GetValue<string>("WsFederation:Wtrealm");
                });

            services.AddMvc(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddRazorPagesOptions(options =>
                {
                    options.AllowAreas = true;
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");

                    options.Conventions.AuthorizeFolder("/Manage");
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            // Needed for ASP.NET Core Identity with custom UI
            services.AddSingleton<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStatusCodePagesWithReExecute("/Errors/{0}");
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}