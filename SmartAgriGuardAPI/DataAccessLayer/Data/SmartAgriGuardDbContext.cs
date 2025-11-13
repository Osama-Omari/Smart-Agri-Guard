using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Models;

namespace DataAccessLayer.Data
{
    public class SmartAgriGuardDbContext : DbContext
    {
        public SmartAgriGuardDbContext(DbContextOptions<SmartAgriGuardDbContext> options) : base(options)
        {
        }

        public DbSet<UserRole> UserRoles { get; set; }  

        public DbSet<User> Users { get; set; }

        public DbSet<Greenhouse> Greenhouses { get; set; }

        public DbSet<Plant> Plants { get; set; }

        public DbSet<SensorData> SensorData { get; set; }

        public DbSet<SensorDataArchive> SensorDataArchives { get; set; }

        public DbSet<FarmerPlant> FarmerPlants { get; set; }

        public DbSet<DeviceToken> DeviceTokens { get; set; }

        public DbSet<Prediction> Predictions { get; set; }

        public DbSet<Recommendation> Recommendations { get; set; }

        public DbSet<PlantType> PlantTypes { get; set; }

        public DbSet<PlantNotifications> PlantNotifications { get; set; }

        public DbSet<SystemReports> SystemReports { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");

                entity.HasKey(e => e.Id);

                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(e => e.Users)
                    .WithOne(e => e.UserRole)
                    .HasForeignKey(e => e.UserRoleId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<PlantNotifications>(entity =>
            {
                entity.ToTable("PlantNotifications");
                entity.HasKey(e => e.Id);
                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");
                entity.Property(e => e.TriggerType)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(500);
                entity.Property(e => e.NotificationDate)
                    .IsRequired();
                entity.Property(e => e.IsRead)
                    .IsRequired();
                entity.HasOne(e => e.Plant)
                    .WithMany(e=> e.PlantNotifications)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SystemReports>(entity =>
            {
                entity.ToTable("SystemReports");
                entity.HasKey(e => e.Id);
                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");
                entity.Property(e => e.ErrorType)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.IsRead)
                    .IsRequired();
                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(1000);
                entity.Property(e => e.ReportDate)
                    .IsRequired();
                entity.HasOne(e => e.Greenhouse)
                    .WithMany(e=>e.SystemReports)
                    .HasForeignKey(e => e.GreenhouseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);

                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.username)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.PasswordHash)
                    .IsRequired();
                entity.HasOne(e => e.UserRole)
                    .WithMany(e => e.Users)
                    .HasForeignKey(e => e.UserRoleId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Greenhouse)
                    .WithMany(e => e.Farmers)
                    .HasForeignKey(e => e.GreenhouseId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(e => e.ManagedGreenhouses)
                    .WithOne(e => e.Manager)
                    .HasForeignKey(e => e.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.FarmerPlants)
                    .WithOne(e => e.Farmer)
                    .HasForeignKey(e => e.FarmerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Greenhouse>(entity =>
            {
                entity.ToTable("Greenhouses");
                entity.HasKey(e => e.Id);

                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(300);
                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500);
                entity.HasOne(e => e.Manager)
                    .WithMany(e => e.ManagedGreenhouses)
                    .HasForeignKey(e => e.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.Plants)
                    .WithOne(e => e.Greenhouse)
                    .HasForeignKey(e => e.GreenhouseId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Farmers)
                    .WithOne(e => e.Greenhouse)
                    .HasForeignKey(e => e.GreenhouseId)
                    .OnDelete(DeleteBehavior.SetNull);

            });

            modelBuilder.Entity<Plant>(entity =>
            {
               entity.ToTable("Plants");
                entity.HasKey(e => e.Id);

                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Location)
                    .HasMaxLength(300);
                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500);
                entity.HasOne(e => e.Greenhouse)
                    .WithMany(e => e.Plants)
                    .HasForeignKey(e => e.GreenhouseId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.PlantType)
                    .WithMany(e => e.Plants)
                    .HasForeignKey(e => e.PlantTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.SensorData)
                    .WithOne(e => e.Plant)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Predictions)
                    .WithOne(e => e.Plant)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Recommendations)
                    .WithOne(e => e.Plant)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.FarmerPlants)
                    .WithOne(e => e.Plant)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.SensorDataArchives)
                    .WithOne(e => e.Plant)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<FarmerPlant>(entity =>
            {
                entity.ToTable("FarmerPlants");
                entity.HasKey(e => new { e.FarmerId, e.PlantId });
                entity.HasOne(e => e.Farmer)
                    .WithMany(e => e.FarmerPlants)
                    .HasForeignKey(e => e.FarmerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Plant)
                    .WithMany(e => e.FarmerPlants)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<SensorData>(entity =>
            {
               entity.ToTable("SensorData");

                entity.HasKey(e => e.Id);

                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");


                entity.Property(e => e.Timestamp).IsRequired();

                entity.HasOne(e => e.Plant)
                    .WithMany(e => e.SensorData)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);


            });

            modelBuilder.Entity<SensorDataArchive>(entity =>
            {
                entity.ToTable("SensorDataArchives");
                entity.HasKey(e => e.Id);

                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");


                entity.Property(e => e.Timestamp).IsRequired();
                entity.HasOne(e => e.Plant)
                    .WithMany(e => e.SensorDataArchives)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DeviceToken>(entity =>
            {
                entity.ToTable("DeviceTokens");
                entity.HasKey(e => e.Id);

                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");


                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(500);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.DeviceTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.DeviceType)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.DeviceModel)
                    .HasMaxLength(200);
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                entity.Property(e => e.LastUpdated);
                entity.Property(e => e.IsActive)
                    .IsRequired();

            });

            modelBuilder.Entity<Prediction>(entity =>
            {
                entity.ToTable("Predictions");
                entity.HasKey(e=>e.Id);

                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");


                entity.Property(e => e.PredictionDate).IsRequired();
                entity.HasOne(e => e.Plant)
                    .WithMany(e => e.Predictions)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e=>e.healthStatus).IsRequired().HasMaxLength(100);

            });

            modelBuilder.Entity<Recommendation>(entity =>
            {
                entity.ToTable("Recommendations");
                entity.HasKey(e => e.Id);

                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");


                entity.Property(e => e.RecommendationDate).IsRequired();
                entity.Property(e => e.advice)
                    .IsRequired()
                    .HasMaxLength(1000);
                entity.HasOne(e => e.Plant)
                    .WithMany(e => e.Recommendations)
                    .HasForeignKey(e => e.PlantId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e=>e.isCritical).IsRequired();
            });

            modelBuilder.Entity<PlantType>(entity =>
            {
                entity.ToTable("PlantTypes");
                entity.HasKey(e => e.Id);

                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");


                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);
                entity.HasMany(e => e.Plants)
                    .WithOne(e => e.PlantType)
                    .HasForeignKey(e => e.PlantTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


        }
    }
}
