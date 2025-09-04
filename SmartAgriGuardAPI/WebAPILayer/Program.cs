using ApplicationLayer.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using InfrastructureLayer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerUI;
using FluentValidation.AspNetCore;
using FluentValidation;
using ApplicationLayer.Validators;
using ApplicationLayer.MappingProfiles;
namespace WebAPILayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;

                });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<SmartAgriGuardDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
                };
            });
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<ManagerRegisterDTOValidation>();
            builder.Services.AddAutoMapper(builder =>
            {
                builder.AddMaps(typeof(UserMapppingProfile).Assembly);
            });

            builder.Services.Configure<PasswordSettings>(
            builder.Configuration.GetSection("PasswordSettings"));
            builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            builder.Services.AddScoped<IJWTService, JWTService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IGreenhouseRepository, GreenhouseRepository>();
            builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();
            builder.Services.AddScoped<IPredictionRepository,PredictionRepository>();
            builder.Services.AddScoped<ISensorDataRepository, SensorDataRepository>();
            builder.Services.AddScoped<ISensorDataArchiveRepository, SensorDataArchiveRepository>();
            builder.Services.AddScoped<IPlantRepository,PlantRepository>();
            builder.Services.AddScoped<IPlantTypeRepository,PlantTypeRepository>();
            builder.Services.AddScoped<DeviceTokenRepository,DeviceTokenRepository>();
            builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddScoped<IFarmerPlantRepository,FarmerPlantRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<IGreenhouseService, GreenhouseService>();


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using(var scope = app.Services.CreateScope())
            {
                var userService = scope.ServiceProvider.GetService<IUserService>();
                DbInitializer.SeedAdmins(userService, builder.Configuration["Admin:FullName"], builder.Configuration["Admin:UserName"], builder.Configuration["Admin:Password"]).GetAwaiter().GetResult();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
