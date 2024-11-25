using ECommerceDemo.Data;
using ECommerceDemo.Models.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.OpenApi.Models;
namespace ECommerceDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(7);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AllUsers", policy =>
                    policy.RequireRole("Admin", "Employee", "Child", "Worker", "Donator", "Member"));
            });

            // Add Swagger to the container
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Bearer token format. Enter 'Bearer {token}'"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new List<string>()
                    }
                });
            });

            builder.Services.AddEndpointsApiExplorer();

            var configuration = builder.Configuration;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
          .AddJwtBearer(options =>
          {
              options.SaveToken = true;
              options.RequireHttpsMetadata = false;
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])),
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidIssuer = configuration["Jwt:Issuer"],
                  ValidAudience = configuration["Jwt:Audience"],
                  RoleClaimType = ClaimTypes.Role,
                  ClockSkew = TimeSpan.Zero
              };
          });

            var app = builder.Build();

            // Enable Swagger middleware
            app.UseSwagger(); // Enable Swagger generation

            // Enable Swagger UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API v1"); // Define Swagger UI endpoint
                c.RoutePrefix = string.Empty; // Set Swagger UI to be served at the root
            });

            // Perform database migrations and seed initial data if needed.
            Task.Run(async () =>
            {
                using (var scope = app.Services.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    if (_context.Database.GetPendingMigrations().Any())
                    {
                        _context.Database.Migrate(); // Auto Migration
                    }

                    var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var adminRole = await _roleManager.FindByNameAsync("Admin");
                    if (adminRole == null)
                    {
                        var identityRole = new IdentityRole("Admin");
                        await _roleManager.CreateAsync(identityRole);
                    }

                    using (var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
                    {
                        var adminUser = await _userManager.FindByNameAsync("Admin");
                        if (adminUser == null)
                        {
                            var applicationUser = new ApplicationUser
                            {
                                UserName = "Admin",
                                Email = "admin@admin.com",
                                IsActive = true
                            };
                            var result = await _userManager.CreateAsync(applicationUser, "Admin123!");
                            if (result.Succeeded)
                            {
                                await _userManager.AddToRoleAsync(applicationUser, "Admin");
                            }
                        }
                    }
                }
            });
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
