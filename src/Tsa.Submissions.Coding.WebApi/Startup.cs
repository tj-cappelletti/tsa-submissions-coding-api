using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Middleware;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.WebApi;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tsa.Submissions.Coding.WebApi v1");

                options.OAuthClientId("tsa.submissions.coding.api.swagger");
                options.OAuthAppName("TSA Coding Submissions Web UI");
                options.OAuthUsePkce();
            });

            // In a development setting, all containers use the ASP.NET development certificate
            // Since this is a self signed certificate, the root CA must be trusted
            // The Docker Compose mounts a volume in the container for the root CA certificate
            // This code block updates the trusted root CA so ASP.NET development certificates work
            var dockerContainer = Configuration["DOCKER_CONTAINER"] != null && Configuration["DOCKER_CONTAINER"] == "Y";

            if (dockerContainer)
            {
                var updateCaCertificatesProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        Arguments = "update-ca-certificates",
                        CreateNoWindow = true,
                        FileName = "/bin/bash",
                        UseShellExecute = false
                    }
                };

                updateCaCertificatesProcess.Start();
                updateCaCertificatesProcess.WaitForExit();
            }
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<SubmissionsDatabase>(Configuration.GetSection("SubmissionsDatabase"));

        if (Configuration["DOCKER_CONTAINER"] != null && Configuration["DOCKER_CONTAINER"] == "Y")
            services.AddCors();

        services.AddSingleton<ITeamsService, TeamsService>();

        var requireAuthenticatedUserPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = Configuration["Authentication:Authority"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //TODO: Find what static property this could pull from (likely in IdentityServer's libraries)
                    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                    ValidateAudience = false
                };
            });

        services.AddAuthorization(configuration => { configuration.AddPolicy("ShouldContainRole", options => options.RequireClaim(ClaimTypes.Role)); });

        services.AddControllers(configure => { configure.Filters.Add(new AuthorizeFilter(requireAuthenticatedUserPolicy)); });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Tsa.Submissions.Coding.WebApi", Version = "v1" });
            
            options.EnableAnnotations();

            //TODO: Make a special "Swagger" API client for this
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{Configuration["Authentication:Authority"]}/connect/authorize"),
                        TokenUrl = new Uri($"{Configuration["Authentication:Authority"]}/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "tsa.submissions.coding.read", "Display/read coding submissions" },
                            { "tsa.submissions.coding.create", "Create coding submissions" }
                        }
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });
    }
}
