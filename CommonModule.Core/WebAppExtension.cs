using System.Globalization;
using AutoMapper;
using CommonModule.Core.Auth;
using CommonModule.Core.Extensions;
using CommonModule.Interfaces;
using CommonModule.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerUI;
using IServiceScopeFactory = Microsoft.Extensions.DependencyInjection.IServiceScopeFactory;

namespace CommonModule.Core;

public static class WebAppExtension
{
    public static void AddDatabaseContext<TDataContext>(this WebApplicationBuilder builder, string dbName = "Database")
        where TDataContext : DbContext
    {
        builder.Services.AddDbContext<TDataContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString(dbName)));
    }

    public static void AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped(typeof(FilterBuilder<,>));
        builder.Services.AddScoped(typeof(IEntityValidator<>), typeof(EntityValidator<>));

        // Register Generic Repository with TDataContext
        builder.Services.AddScoped(typeof(IReadGenericRepository<,,>), typeof(GenericRepository<,,>));
        builder.Services.AddScoped(typeof(IGenericRepository<,,>), typeof(GenericRepository<,,>));

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IJwtTokenFactory, JwtTokenFactory>();

        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<ILocalizationService, LocalizationService>();
    }

    public static void AddGoogleAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });
    }

public static void AddSwagger(this WebApplicationBuilder builder)
{
    string version = builder.Configuration["Microservice:Version"];
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc(version,
            new OpenApiInfo { Title = builder.Configuration["Microservice:Title"], Version = version });
        c.AddSecurityDefinition("JwtHonk", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header using the JwtHonk scheme. \r\n\r\n 
                  Enter 'JwtHonk' [space] and then your token in the text input below.
                  \r\n\r\nExample: 'JwtHonk 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "JwtHonk"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "JwtHonk"
                    },
                    Scheme = "oauth2",
                    Name = "JwtHonk",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
        // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        // c.IncludeXmlComments(xmlPath);
    });
}
    public static void AddSwagger(this IApplicationBuilder app, WebApplicationBuilder builder)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
            c.OAuthClientId(builder.Configuration["Authentication:Google:ClientId"]);
            c.OAuthClientSecret(builder.Configuration["Authentication:Google:ClientSecret"]);
            c.OAuthUsePkce(); // Use PKCE for enhanced security
            c.OAuthScopes("openid", "profile", "email");
            c.OAuthConfigObject = new OAuthConfigObject()
            {
                ClientId = builder.Configuration["Authentication:Google:ClientId"],
                ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"],
                Realm = "JobPathfinder",
                AppName = "JobPathfinder",
                ScopeSeparator = " ",
                AdditionalQueryStringParams = new Dictionary<string, string> { { "prompt", "consent" } }
            };
        });
    }

    public static void AddCors(this WebApplicationBuilder builder)
    {
        string allowedHosts = builder.Configuration["AllowedHosts"];
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins",
                builder =>
                {
                    builder.WithOrigins(allowedHosts)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); // If cookies or authorization headers are needed
                });
        });
    }

    public static void AddMapper<T>(this WebApplicationBuilder builder)
        where T : Profile, new()
    {
        builder.Services.AddAutoMapper(config =>
        {
            config.AddProfile(new T());
        });
    }

    public static void AddJwt(this WebApplicationBuilder builder)
    {
        byte[] key = builder.Configuration["Authentication:Jwt:SecretKey"].StringToUtf8Bytes();

        builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
    }

    public static void UseTokenValidator(this IApplicationBuilder app)
    {
        // TODO DynamoDB token validation
        // Ensure Swagger UI is available
        app.Use(async (context, next) =>
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var tokenService = context.RequestServices.GetRequiredService<ITokenService>();
                var token = context.Request.Headers["Authorization"].ToString().Split(' ').Last();

                // TODO add refresh token if needed
                if (!await tokenService.IsTokenValidAsync(token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            await next();
        });
    }
}