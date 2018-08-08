﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Utilization.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace AtriumWebApp.Web.Utilization
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options => options.Filters.Add(new RestrictAccessWithApp())).AddJsonOptions(
                options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddOptions();
            services.AddScoped<SharedContext>(_ => new SharedContext(Configuration.GetConnectionString("database")));
            services.AddScoped<PASRRContext>(_ => new PASRRContext(Configuration.GetConnectionString("database")));
            services.AddScoped<PELIContext>(_ => new PELIContext(Configuration.GetConnectionString("database")));
            services.AddScoped<SkilledChartingContext>(_ => new SkilledChartingContext(Configuration.GetConnectionString("database")));
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();
            services.Configure<AppSettingsConfig>(Configuration.GetSection("Config"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseResponseCompression();
            app.UseStaticFiles();

            app.UseSession();
            if (env.EnvironmentName == "Development")
            {
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "CEA_TST/Utilization/{controller=Home}/{action=Index}/{id?}");
                });
            }
            else
            {
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });
            }
        }
    }
}