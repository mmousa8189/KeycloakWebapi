using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace KeycloakWebapi
{
    public class Startup
    {

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            this.environment = environment;
        }

        public readonly IWebHostEnvironment environment;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "KeycloakWebapi", Version = "v1" });
            });


            // services.AddAuthorization(options =>
            //   {
            //       options.AddPolicy("user", policy => policy.RequireRole("user"));
            //   });

            services.AddAuthentication(options =>
                   {
                       options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                       options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                   }).AddJwtBearer(o =>
                   {
                       o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                       {
                           ValidAudiences = new string[] { "springboot-api", "Demo-Realm", "account" }
                       };
                       o.Authority = Configuration["Jwt:Authority"];
                       o.Audience = Configuration["Jwt:Audience"];

                       o.RequireHttpsMetadata = false;
                       o.Events = new JwtBearerEvents()
                       {
                           OnAuthenticationFailed = c =>
                           {
                               c.NoResult();

                               c.Response.StatusCode = 500;
                               c.Response.ContentType = "text/plain";
                               if (environment.IsDevelopment())
                               {
                                   return c.Response.WriteAsync(c.Exception.ToString());
                               }
                               return c.Response.WriteAsync("An error occured processing your authentication.");
                           }
                       };
                   });
            services.AddAuthorization(options =>
    {
        options.AddPolicy("User", policy => policy.RequireClaim("user_roles", "user"));
    });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "KeycloakWebapi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication(); // added
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
