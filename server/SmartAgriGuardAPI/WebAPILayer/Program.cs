using ApplicationLayer.Interfaces;
using ApplicationLayer.MappingProfiles;
using ApplicationLayer.Validators;
using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using InfrastructureLayer.AI.Interfaces;
using InfrastructureLayer.AI.Services;
using InfrastructureLayer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAPILayer.JsonConverter;
namespace WebAPILayer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Set the QuestPDF license for the reporting engine
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var builder = WebApplication.CreateBuilder(args);

            // --- 1. CONTROLLERS & JSON CONFIGURATION ---
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;

                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartAgriGuard API", Version = "v1" });

                // 1. Define the Security Scheme
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                // 2. Add the Security Requirement
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[]{ }
                    }
                });

                // Includes XML comments from the code in the Swagger UI
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                option.IncludeXmlComments(xmlPath);
            });

            // --- 3. BACKGROUND PROCESSING (HANGFIRE) ---
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHangfireServer();

            // --- 4. DATA PERSISTENCE (ENTITY FRAMEWORK) ---
            builder.Services.AddDbContext<SmartAgriGuardDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers()
             .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new RequestUtcDateTimeOffsetConverter());
             });

            // --- 5. SECURITY & AUTHENTICATION (JWT) ---
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
                };
            });
            // --- 6. DEPENDENCY INJECTION (SERVICES & REPOSITORIES) ---
            // Register AutoMapper and FluentValidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<ManagerRegisterDTOValidation>();
            builder.Services.AddAutoMapper(builder =>
            {
                builder.AddMaps(typeof(UserMapppingProfile).Assembly);
            });
            // Core Repositories & Services
            builder.Services.Configure<PasswordSettings>(
            builder.Configuration.GetSection("PasswordSettings"));
            builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            builder.Services.AddScoped<IJWTService, JWTService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IGreenhouseRepository, GreenhouseRepository>();
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
            builder.Services.AddScoped<IReportServcie, ReportServcie>();
            builder.Services.AddScoped<PdfReportStrategy>();
            builder.Services.AddScoped<ExcelReportStrategy>();
            builder.Services.AddScoped<INotificationService, FirebaseNotificationService>();
            builder.Services.AddScoped<IPlantNotificationsRepository, PlantNotificationsRepository>();
            builder.Services.AddScoped<ISystemReportsRepository, SystemReportsRepository>();
            builder.Services.AddScoped<IPlantScheduleRepository, PlantScheduleRepository>();
            builder.Services.AddScoped<IPlantNotificationJob, PlantNotificationJob>();
            builder.Services.AddScoped<IPlantScheduleService, PlantScheduleService>();
            builder.Services.AddScoped<IDataCleanupJob, DataCleanupJob>();
            builder.Services.AddSingleton<IPlantHealthModel, TomatoHealthModel>();
            builder.Services.AddScoped<IPlantHealthService,PlantHealthService>();





            var app = builder.Build();

            // --- 7. STARTUP TASKS & RECURRING JOBS ---
            using (var scope = app.Services.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                var userService = scope.ServiceProvider.GetService<IUserService>();

                // Seed Admin User
                DbInitializer.SeedAdmins(userService!, builder.Configuration["Admin:FullName"]!, builder.Configuration["Admin:UserName"]!, builder.Configuration["Admin:Password"]!).GetAwaiter().GetResult();

                //var notificationService = scope.ServiceProvider.GetService<INotificationService>();
                //var adminId = Guid.Parse("BF5E5D61-47F8-4B08-90E6-1E83B77BCDC2");
                //await notificationService!.NotifyAdminTest(adminId);



                // Register Global Maintenance Jobs
                recurringJobManager.AddOrUpdate<IDataCleanupJob>(
                "DataCleanupJob",
                job => job.ClearReadNotificationsAndReports(),
                Cron.Daily
                );

                recurringJobManager.AddOrUpdate<IDataCleanupJob>(
                    "Weekly-Token-Cleanup",
                    job => job.RemoveInactiveDeviceTokens(),
                    Cron.Weekly
                    );

                recurringJobManager.AddOrUpdate<IDataCleanupJob>(
                    "Process-SensorData-lifecycle",
                    job => job.ProcessSensorDataLifecycle(),
                    Cron.Daily);
            }

            // --- 8. MIDDLEWARE PIPELINE ---
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseHangfireDashboard();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
