using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tsa.Submissions.Coding.WebApi.Authentication;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Tsa.Submissions.Coding.WebApi.Swagger;
using Tsa.Submissions.Coding.WebApi.Validators;

namespace Tsa.Submissions.Coding.WebApi;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<SubmissionsDatabase>(Configuration.GetSection(ConfigurationKeys.SubmissionsDatabaseSection));

        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true)
        };

        ConventionRegistry.Register("DefaultConventionPack", conventionPack, _ => true);

        var mongoClientSettings = MongoClientSettings.FromConnectionString(Configuration.GetConnectionString(ConfigurationKeys.MongoDbConnectionString));
        mongoClientSettings.ServerSelectionTimeout = new TimeSpan(0, 0, 0, 10);
        mongoClientSettings.ConnectTimeout = new TimeSpan(0, 0, 0, 10);

        services.Add(
            new ServiceDescriptor(typeof(IMongoClient),
                new MongoClient(mongoClientSettings)));

        var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

        // Add MongoDB Services
        const string servicesNamespace = "Tsa.Submissions.Coding.WebApi.Services";
        var mongoDbServiceTypes = assemblyTypes
            .Where(type => type.Namespace == servicesNamespace && type is
                { IsAbstract: false, IsClass: true, IsGenericType: false, IsInterface: false, IsNested: false })
            .ToList();

        var mongoEntityServiceInterfaceType = typeof(IMongoEntityService<>);
        var pingableServiceInterfaceType = typeof(IPingableService);

        foreach (var mongoDbServiceType in mongoDbServiceTypes)
        {
            var interfaceType = mongoDbServiceType
                .GetInterfaces()
                .SingleOrDefault(type =>
                    type.Name != mongoEntityServiceInterfaceType.Name &&
                    type.Name != pingableServiceInterfaceType.Name);

            if (interfaceType == null) continue;

            services.AddScoped(interfaceType, mongoDbServiceType);
        }

        // Add Pingable Services - Should match MongoDB Services
        var pingableServiceType = typeof(IPingableService);
        var pingableServices = assemblyTypes
            .Where(type => pingableServiceType.IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false })
            .ToList();

        foreach (var pingableService in pingableServices)
        {
            services.AddSingleton(typeof(IPingableService), pingableService);
        }

        // Add Redis Service
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = Configuration.GetConnectionString(ConfigurationKeys.RedisConnectionString);
            options.InstanceName = "Tsa.Submissions.Coding.WebApi";
        });

        services.AddSingleton<ICacheService, CacheService>();

        // Add Validators
        services.AddScoped<IValidator<ProblemModel>, ProblemModelValidator>();
        services.AddScoped<IValidator<TeamModel>, TeamModelValidator>();
        services.AddScoped<IValidator<TestSetModel>, TestSetModelValidator>();

        // Setup authentication and authorization
        var requireAuthenticatedUserPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        services
            .AddAuthentication(TsaAuthenticationOptions.DefaultScheme)
            .AddScheme<TsaAuthenticationOptions, TsaAuthenticationHandler>(TsaAuthenticationOptions.DefaultScheme, _ => { });

        services.AddAuthorizationBuilder()
            .AddPolicy("ShouldContainRole", options => options.RequireClaim(ClaimTypes.Role));

        // Setup Controllers
        services
            .AddControllers(configure => { configure.Filters.Add(new AuthorizeFilter(requireAuthenticatedUserPolicy)); })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        // Add Fluent Validation
        services.AddFluentValidationAutoValidation();

        // Add Swagger
        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            options.OperationFilter<ApiKeyHeaderFilter>();
        });
    }
}
