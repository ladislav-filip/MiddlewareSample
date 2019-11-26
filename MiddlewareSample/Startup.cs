using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiddlewareSample.Infrastructure;
using MiddlewareSample.Middlewares;

namespace MiddlewareSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
//            var serviceProvider = services.BuildServiceProvider();
//            var logger = serviceProvider.GetService<ILogger<AnyClass>>();
//            services.AddSingleton(typeof(ILogger), logger);
            
            services.AddLogging(config =>
            {
                config.AddConsole();
            });

            // registrace vlastního validátoru tokenu
            services.AddSingleton<ITokenValidator, TokenValidator>();
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // https://www.devtrends.co.uk/blog/conditional-middleware-based-on-request-in-asp.net-core
            // https://thomaslevesque.com/2018/03/27/understanding-the-asp-net-core-middleware-pipeline/
            app.UseMiddleware<LanguageMiddleware>();

            app.UseWhen(
                context => context.Request.Path.StartsWithSegments(new PathString("/foo")),
                a => a.Use(async (context, next) =>
                {
                    Console.WriteLine("B (before)");
                    await next();
                    Console.WriteLine("B (after)");
                }));
            
            app.UseMiddleware<TokenMiddleware>(); 

            app.Run(async (context) =>
            {
                context.Response.Headers.Add("Content-Type", "text/html");
                await context.Response.WriteAsync($"Nazdar {CultureInfo.CurrentCulture.DisplayName}");
                await context.Response.WriteAsync($"<br/><a href='api/'>API</a>");
                await context.Response.WriteAsync($"<br/><a href='foo/'>FOO</a>");
            });
        }
    }
}