using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NSwag.Generation.Processors.Security;
using TeamServer.Controllers;

namespace TeamServer
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
            services.AddControllers().AddNewtonsoftJson(j =>
            {
                j.SerializerSettings.ContractResolver = new DefaultContractResolver();
                j.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            services.AddSwaggerDocument(c =>
            {
                c.PostProcess = d =>
                {
                    d.Info.Version = "v1";
                    d.Info.Title = "SharpC2 API";
                    d.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Julio Urena, Daniel Duggan, Adam Chester",
                        Url = "https://github.com/SharpC2/SharpC2"
                    };
                };
                c.DocumentProcessors.Add(new SecurityDefinitionAppender("Bearer", new NSwag.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = " JWT Authorization header using the Bearer scheme.",
                    Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header
                }));
                c.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));
            });

            services.AddAuthentication(a =>
            {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(j =>
            {
                j.RequireHttpsMetadata = false;
                j.SaveToken = true;
                j.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(AuthenticationController.JWTSecret),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
