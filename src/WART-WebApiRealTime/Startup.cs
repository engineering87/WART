// (c) 2021 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using WART_Core.Enum;
using WART_Core.Middleware;

namespace WART_Api
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
            services.AddControllers();

            // add the Wart middleware service extension

            // default without authentication            
            //services.AddWartMiddleware();

            // with JWT authentication
            //services.AddWartMiddleware(hubType: HubType.JwtAuthentication, tokenKey: "dn3341fmcscscwe28419brhwbwgbss4t");

            // with Cookie authentication
            services.AddWartMiddleware(hubType: HubType.CookieAuthentication);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WART-Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WART-Api");
                c.RoutePrefix = string.Empty;
            });

            // use the Wart middleware builder extension
            // default without authentication
            //app.UseWartMiddleware();

            // with JWT authentication
            //app.UseWartMiddleware(HubType.JwtAuthentication);

            // with Cookie authentication
            app.UseWartMiddleware(HubType.CookieAuthentication);

            // multiple hub with authentication
            //var hubNameList = new List<string>
            //{
            //    "warthub",
            //    "warthub_clone"
            //};
            //app.UseWartMiddleware(hubNameList, HubType.JwtAuthentication);
        }
    }
}
