using API.GraphQL.Queries;
using Core.Interfaces;
using Core.Services;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Shared.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace API.Dependencies;
public static class Program
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddHttpContextAccessor();

        services.AddTransient<ICoreLogService, CoreLogService>();

        services.AddTransient<IAspNetUserService, AspNetUserService>();
        services.AddTransient<IAspNetUserRepo, AspNetUserRepo>();


        return services;
    }

    public static IServiceCollection AddJsonOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers().AddNewtonsoftJson(s =>
        {
            s.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            s.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            s.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        });

        return services;
    }

    public static IHostBuilder AddDependencies(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        hostBuilder.UseSerilog((context, configuration) => {
            configuration
            .ReadFrom.Configuration(context.Configuration)
            // .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            // .MinimumLevel.Override("System", minimumLevel: LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console();
        });

        return hostBuilder;
    }

    public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options => options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials();
        }));

        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtIssuer"],
                    ValidAudiences = new List<string> { configuration["JwtAudience"] },
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKeyResolver = (string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters) =>
                    {
                        var keys = new List<SecurityKey>();
                        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
                        keys.Add(signingKey);
                        return keys;
                    }
                };
                cfg.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

    public static IServiceCollection AddGrahQL(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddGraphQLServer()
            .AddAuthorization()
            .AddQueryType<Query>();

        return services;
    }

    public static IServiceCollection AddSwaggerAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API",
                Version = "v1",
                Description = "BoilerPlate Api",
                Contact = new OpenApiContact
                {
                    Name = "Osmel Crespo",
                    Email = "osmel.e.crespo@outlook.com",
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {{ new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }}, Array.Empty<string>() }});
        });

        return services;
    }

}
