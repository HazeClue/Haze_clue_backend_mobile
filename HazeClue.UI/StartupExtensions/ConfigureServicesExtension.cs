using HazeClue.Core.Domain.Entities;
using HazeClue.Infrastructure.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace HazeClue.UI.StartupExtensions
{
    /// <summary>
    /// Provides extension methods for configuring core application services for the HazeClue Web API.
    /// Includes setup for controllers, Swagger documentation, API versioning, and JWT authentication.
    /// </summary>
    public static class ConfigureServicesExtension
    {
        /// <summary>
        /// Configures the main services required by the HazeClue Web API.
        /// This method handles:
        /// <list type="bullet">
        /// <item><description>Registering controllers with JSON input/output format.</description></item>
        /// <item><description>Configuring JWT authentication for secure API access.</description></item>
        /// <item><description>Setting up Swagger with XML documentation and JWT security definition.</description></item>
        /// <item><description>Enabling API versioning using URL segments and versioned API explorer.</description></item>
        /// </list>
        /// </summary>
        /// <param name="services">The service collection where all application services are registered.</param>
        /// <param name="configuration">The application configuration source (e.g., appsettings.json).</param>
        /// <returns>The configured <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(new ProducesAttribute("application/json"));
                options.Filters.Add(new ConsumesAttribute("application/json"));
            });

            // JWT config moved below Identity

            #region Swagger Setting

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(swagger =>
            {
                swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml")); // bin/Debug/net8.0

                swagger.SwaggerDoc("v1", new OpenApiInfo() { Title = "HazeClue Web API", Version = "1.0" });
                //swagger.SwaggerDoc("v2", new OpenApiInfo() { Title = "HazeClue Web API", Version = "2.0" });
                
                //To Enable authorization using Swagger(JWT)
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your?valid token in the text input below.Example:Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                });
            });

            #endregion

            #region Dbcontext

            services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("CS"));
            });

            #endregion

            #region Identity

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredUniqueChars = 2;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 5;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders()
              .AddUserStore<UserStore<AppUser, IdentityRole, ApplicationDbContext, string>>()
              .AddRoleStore<RoleStore<IdentityRole, ApplicationDbContext, string>>();

            #endregion

            #region JWT

            services.AddAuthentication(Options =>
            {
                Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(Options =>
            {
                Options.SaveToken = true;
                Options.RequireHttpsMetadata = false; 
                Options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])) 
                };
            });

            #endregion

            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });


            return services;
        }
    }
}
