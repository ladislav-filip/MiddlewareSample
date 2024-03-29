﻿using System;
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
                config.ClearProviders();
                config.AddFilter("Microsoft", LogLevel.Error)
                    .AddFilter("System", LogLevel.Warning);
                config.AddDebug();
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
            app.UseLanguage();
            
            // provede se pouze pokud je v URL obsaženo "/api", funguje to jako taková "vsuvka" mezi ostatní middlewary
            // po průchodu tímto middleware to pokračuje standardním způsobem
            app.UseWhen(context => context.Request.Path.StartsWithSegments(new PathString("/api")),a => 
            {
                a.UseToken();
            });
            
            // pokud je v URL /second, tak se spustí tato oddělená větev a hlavní se nepovádí 
            app.Map("/second", a =>
            {
                a.UseMiddleware<SecondBranchFirstMiddleware>();
                a.Run(SecondRun);
            });
            
            // jiná "větev" zpracování requestu, tento meddleware nevrací zpracování do původní/hlavní linie
            // je to stejná funkčnost jako "Map", ale je zde možné použít "predikát" do kterého lze vložit složitější podmínku
            app.MapWhen(context => context.Request.Path.StartsWithSegments(new PathString("/foo")) || context.Request.Query.ContainsKey("foo"),
                a =>
            {
               a.Run(FooRun); 
            });

            app.Run(async (context) =>
            {
                context.Response.Headers.Add("Content-Type", "text/html");
                await context.Response.WriteAsync($"Nazdar {CultureInfo.CurrentCulture.DisplayName}");
                await context.Response.WriteAsync($"<br/><a href='/api/'>API</a>");
                await context.Response.WriteAsync($"<br/><a href='/foo/'>FOO</a>");
                await context.Response.WriteAsync($"<br/><a href='/jinak?foo'>FOO jinak</a>");
                await context.Response.WriteAsync($"<br/><a href='/second/'>SECOND</a>");
            });
        }

        private async Task SecondRun(HttpContext context)
        {
            Console.WriteLine("Run second start...");
            context.Response.Headers.Add("Content-Type", "text/html");
            await context.Response.WriteAsync($"<h1 style='background-color:lime;'>Tady je SECOND</h1>");
            await context.Response.WriteAsync($"<br/><a href='/api/'>API</a>");
            await context.Response.WriteAsync($"<br/><a href='/foo/'>FOO</a>");
            await context.Response.WriteAsync($"<br/><a href='/'>Home</a>");
            Console.WriteLine("Run second end.");
        }
        
        private async Task FooRun(HttpContext context)
        {
            Console.WriteLine("Run FOO start...");
            context.Response.Headers.Add("Content-Type", "text/html");
            await context.Response.WriteAsync($"<h1 style='background-color:lime;'>Tady je FOO</h1>");
            await context.Response.WriteAsync($"<br/><a href='/api/'>API</a>");
            await context.Response.WriteAsync($"<br/><a href='/second/'>SECOND</a>");
            await context.Response.WriteAsync($"<br/><a href='/'>Home</a>");
            Console.WriteLine("Run FOO end.");
        }
    }
}