using ApplicationLayer.Interfaces;
using ApplicationLayer.MappingProfiles;
using ApplicationLayer.Validators;
using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using InfrastructureLayer.BackgroundServices;
using InfrastructureLayer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using WebAPILayer.Filters;
namespace WebAPILayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;

                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


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
            builder.Services.AddScoped<IDeviceTokenRepository,DeviceTokenRepository>();
            builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddScoped<IFarmerPlantRepository,FarmerPlantRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<IGreenhouseService, GreenhouseService>();
            builder.Services.AddScoped<IPlantService, PlantService>();
            builder.Services.AddScoped<IPlantTypeService, PlantTypeService>();
            builder.Services.AddScoped<IFarmerPlantService, FarmerPlantService>();
            builder.Services.AddScoped<ISensorDataArchiveService, SensorDataArchiveService>();
            builder.Services.AddScoped<ISensorDataService, SensorDataService>();
            builder.Services.AddHostedService<DeviceTokenCleanupService>();
            builder.Services.AddScoped<IReportServcie, ReportServcie>();
            builder.Services.AddScoped<PdfReportStrategy>();
            builder.Services.AddScoped<ExcelReportStrategy>();
            builder.Services.AddScoped<INotificationService, FirebaseNotificationService>();





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
