using System;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Shared.Interfaces;
using SharedBinance.Broker;
using Utility.ExecutionContext;
using WebRouterApp.Core.Application.Services;
using WebRouterApp.Core.CopyEngineParts;
using WebRouterApp.Core.Data;
using WebRouterApp.Core.Infrastructure;
using WebRouterApp.Core.Infrastructure.ApiCallContextParts;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.EndpointHandlerParts;
using WebRouterApp.Core.Infrastructure.ApiRequestHandling.Validation;
using WebRouterApp.Core.Infrastructure.ConfigurationParts;
using WebRouterApp.Core.RecordingLoggerParts;
using WebRouterApp.Features.Auth.Application.Services;
using WebRouterApp.Features.Subscribers.Application.Services;
using WebRouterApp.Features.Trading.SignalR;
using WebRouterApp.Shared.Services;

namespace WebRouterApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

            services.AddDbContext(Configuration);

            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddSignalR();
            services.AddAllowSpecificOriginsPolicy();
            services.AddJwtAuthentication(Configuration);

            services.AddApiCallContext();
            services.AddApiRequestHandling();

            services.AddSingleton<CopyEngine>();

            services.AddSingleton(static _ =>
            {
                var logger = new ConsoleLogger();
                return new RecordingLogger(logger, new Worker(logger));
            });
            services.AddSingleton<ILogger>(static sp => sp.GetRequiredService<RecordingLogger>());

            services.AddTransient(typeof(Log<>));

            services.AddHelpers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // app.UseHttpsRedirection();

            app.UseStaticFiles();
            if (!env.IsDevelopment()) app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseCors(CorsPolicyName.AllowSpecificOrigins);

            app.UseAppExceptionHandler();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseApiCallContext();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");

                endpoints.MapHub<TradingHub>("tradingHub");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                    //spa.UseAngularCliServer(npmScript: "start");
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
            });
        }
    }

    internal static class StartupServiceCollectionExtensions
    {
        public static void AddSignalR(this IServiceCollection services)
        {
            services.AddSignalR(options => options.AddFilter<ApiCallContextHubFilter>());

            services.AddTransient<TradingHubContext>();
            services.AddSingleton<TradingMessageQueue>();
            services.AddHostedService<BufferingTradingMessageDispatcher>();
        }

        public static void AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSection = configuration.GetJwtSection();

            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var signingKey = jwtSection["SigningKey"];

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256);

            services.Configure<JwtOptions>(options =>
            {
                options.Issuer = issuer;
                options.Audience = audience;
                options.SigningCredentials = signingCredentials;
            });

            services.AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingCredentials.Key,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddSingleton<JwtGenerator>();

            services.Configure<RefreshTokenOptions>(configuration.GetRefreshTokenSection());
            services.AddSingleton<RefreshTokenGenerator>();
        }

        public static void AddAllowSpecificOriginsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    CorsPolicyName.AllowSpecificOrigins,
                    builder =>
                    {
                        builder
                            .WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });
        }

        public static void AddApiRequestHandling(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<Startup>(ServiceLifetime.Singleton);
            services.AddTransient<ValidatorFactory>();
            services.AddApiRequestHandlersFromAssemblyContaining<Startup>();
            services.AddEndpointHandler();
        }

        public static void AddApiCallContext(this IServiceCollection services)
        {
            services.AddScoped<ApiCallContext>();
            services.AddScoped<IApiCallContextReader>(sp => sp.GetRequiredService<ApiCallContext>());
        }

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TradeRouterDbContext>(options => options
                //.UseInMemoryDatabase(databaseName: "TradeRouter")
                //.ConfigureWarnings(_ => _.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseSqlite(configuration.GetConnectionString("DefaultConnection"))
            );
        }

        public static void AddHelpers(this IServiceCollection services)
        {
            services.AddTransient(typeof(Factory<>));
            services.AddSingleton(typeof(ServiceScopeFactory<>));
            services.AddTransient<ForEachSubscriber>();
            services.AddSingleton<TraderLock>();
        }
    }

    public static class CorsPolicyName
    {
        public static readonly string AllowSpecificOrigins = "AllowSpecificOriginsPolicy";
    }
}
