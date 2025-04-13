using api_desafioo.tech.Context;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using api_desafioo.tech.Services;
using api_desafioo.tech.Configurations;
using FluentValidation;
using api_desafioo.tech.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using api_desafioo.tech.Helpers;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using StackExchange.Redis;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        JwtConfig.Initialize(builder.Configuration);
        SmtpConfig.Initialize(builder.Configuration);

        builder.Services.AddControllers();

        builder.Services.AddFluentValidationAutoValidation()
                        .AddFluentValidationClientsideAdapters();

        builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddTransient<IEmail, EmailService>();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
            {
                Title = "Desafioo.tech API",
                Version = "v1",
                Description = "API para o desafioo.tech"
            });

            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
            {
                Description = "Autorização JWT - Bearer JWT",
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "Jwt",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference()
                        {
                            Id = "Bearer",
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = JwtConfig.Issuer,
                ValidAudience = JwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtConfig.PrivateKey))
            };
        });

        builder.Services.AddTransient<TokenService>();

        if (!builder.Environment.IsEnvironment("Testing"))
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration["SqlServerConnectionString"]));

            var redisConnectionString = builder.Configuration["RedisConnectionString"];
            if (string.IsNullOrWhiteSpace(redisConnectionString))
            {
                throw new ArgumentNullException("RedisConnection", "Connection string for Redis is missing");
            }
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
        }

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentCorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            options.AddPolicy("ProductionCorsPolicy", builder =>
            {
                builder.WithOrigins("https://desafioo.tech", "https://www.desafioo.tech")
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter("Fixed", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 2
                })
            );

            options.OnRejected = async (context, CancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsJsonAsync(new { Message = "Limite de requisições atingido. Tente novamente mais tarde." });
            };
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseCors("DevelopmentCorsPolicy");
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("ProductionCorsPolicy");

        app.UseRateLimiter();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }
}