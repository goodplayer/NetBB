using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetBB.Domain;
using NetBB.Domain.Infrastructures;
using NetBB.Infrastructure;
using NetBB.Infrastructure.EventBusComponents;
using NetBB.Infrastructure.Repositories;
using NetBB.Infrastructure.Session;
using NetBB.Sources.Components;
using NetBB.Sources.Session;
using NetBB.Sources.Utilities;
using NetBB.System.EventBus.Models;
using NetBB.System.EventBus.Services;

namespace NetBB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            BuildCommon(builder);
            BuildNetBBSystem(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // TODO customize error page for status code 400-599

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // cookie middleware
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict
            });
            // place session middleware here
            //app.UseSession();
            //app.UseMiddleware<AutoManageSessionViaAttribute>();

            // support razor web
            app.MapRazorPages();
            // support api
            app.MapControllers();

            app.Run();
        }

        private static void BuildCommon(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<LongIdGenerator>();

            // Data Protection configuration for session/authN/antiforgery
            // Reference: https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-8.0
            builder.Services.AddDataProtection()
                .SetApplicationName("NetBB")
                .PersistKeysToDbContext<DatabaseContext>()
                .SetDefaultKeyLifetime(TimeSpan.FromDays(30));

            // Session storage within the session period
            //TODO (what is it used for) Support idle time
            //TODO (what is it used for) Support expiration
            // Reference: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-8.0
            //builder.Services.AddDatabaseDistributedCache();
            //builder.Services.ConfigureApplicationCookie(options =>
            //{
            //    options.ExpireTimeSpan = TimeSpan.FromDays(1);
            //});
            //builder.Services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(30);
            //    options.Cookie.MaxAge = TimeSpan.FromDays(1);
            //    options.Cookie.Name = ".netbb.session";
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //});

        }

        private static void BuildNetBBSystem(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<ICommandDispatcher, MemoryCommandDispatcher>();

            // assemble componentes
            builder.Services.Scan(selector =>
            {
                selector
                    .FromAssemblies(
                        typeof(AssemblyDomain).Assembly
                        , typeof(AssemblyInfrastructure).Assembly
                        )
                    .AddClasses(classSelector => classSelector.AssignableTo(typeof(ICommandHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                    .AddClasses(classSelector => classSelector.AssignableTo(typeof(IRepository)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                    .AddClasses(selector => selector.AssignableTo(typeof(IEventHandler<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                    ;
            });

            // initialize database configure
            builder.Services.AddDbContextPool<DatabaseContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
            }, poolSize: builder.Configuration.GetSection("DatabasePool").GetValue<int>("PoolSize"));
            builder.Services.AddSingleton<ITransactionManager, TransactionManagerImpl>();

            /// About Authentication
            /// 1. Methods: cookie, 2FA, 3rd party, jwt, etc.
            /// 2. Token storage
            ///     2.1 cookie including identifier and expiration time - for client to remember
            ///     2.2 backend storage - support multiple device, auth each login(user+device+etc.), track auth lifecycle
            ///     2.3 operations of backend token management - a.Create b.Delete c.Renew
            //
            // Authentication via Cookie
            // TTL is supported while using SignInAsync
            // Support revoke cookie(login status)
            // Reference: https://learn.microsoft.com/zh-cn/aspnet/core/security/authentication/cookie?view=aspnetcore-8.0
            builder.Services.AddSingleton<ITicketStore, TicketStore>();
            builder.Services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<ITicketStore>((options, store) =>
                {
                    options.SlidingExpiration = true;
                    options.Cookie.Name = ".netbb.authn";
                    options.Cookie.HttpOnly = true;
                    // CookieAuthenticationEvents handles cookie auth events
                    // Note: add "builder.Services.AddScoped<NetBBCookieAuthEvents>();" to register instance
                    //options.EventsType = typeof(NetBBCookieAuthEvents); 
                    options.SessionStore = store;
                });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

        }
    }
}
