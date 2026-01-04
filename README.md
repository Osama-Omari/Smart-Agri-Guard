========================================================================
             SMARTAGRIGUARD - SYSTEM ARCHITECTURE DOCUMENT
========================================================================

1. PROJECT OVERVIEW
-------------------
SmartAgriGuard is an enterprise-grade IoT Greenhouse Management System.
It provides real-time environmental monitoring, AI-driven growth 
predictions, and automated notification systems for farmers and managers.

2. ARCHITECTURAL LAYERS
-----------------------

[A] PRESENTATION LAYER (Web API)
    - Technology: ASP.NET Core 8.0
    - Role: Handles HTTP requests, JWT validation, and API versioning.
    - Security: Implements Role-Based Access Control (RBAC) via 
      Custom Claims (Admin, Manager, Farmer).

[B] APPLICATION LAYER (Logic & Contracts)
    - Role: Defines the core business interfaces and data contracts.
    - DTOs: Decouples the API from the Database schema using AutoMapper.
    - Validation: Ensures incoming data meets greenhouse business rules.

[C] INFRASTRUCTURE LAYER (Implementations)
    - Notification: Firebase Cloud Messaging (FCM) for push alerts.
    - Security: HMAC-SHA512 Password hashing with combined Salt/Pepper.
    - Storage: Physical file system management for greenhouse/plant images.
    - Reporting: Strategy Pattern implementation for Excel/PDF exports.
    - JWT: Short-lived access tokens with 15-minute expiration logic.

[D] DATA ACCESS LAYER (Persistence)
    - Technology: Entity Framework Core 
    - Pattern: Repository Pattern (Unit of Work capable).
    - Features: 
        - Eager Loading (Include/ThenInclude) for complex object graphs.
        - Data Tiering: Separate tables for "Hot" live data and "Cold" 
          archived telemetry.
        - Indexing: Composite indexes on PlantId and Timestamp for high 
          performance.

3. KEY FEATURES & BUSINESS LOGIC
--------------------------------

- TIMEZONE LOCALIZATION: The system uses the NodaTime/TimeZoneConverter 
  logic to ensure farmers see sensor timestamps in their local time, 
  regardless of where the server is hosted.

- MANY-TO-MANY RELATIONSHIPS: Robust management of Farmer-to-Plant 
  assignments through a dedicated join-table repository.

- DATA ARCHIVING: Automated logic to move aged sensor data from the 
  primary table to the archive table to maintain API speed.

4. DATABASE CONFIGURATION (EF CORE)
-----------------------------------

To ensure performance with millions of records, the following index
strategy is recommended for the Archive table:

  modelBuilder.Entity<SensorDataArchive>()
      .HasIndex(s => new { s.PlantId, s.Timestamp })
      .HasDatabaseName("IX_SensorDataArchive_Plant_Timestamp");

5. DEVELOPER SETUP
------------------
- Update 'appsettings.json' with valid ConnectionString.
- Place Firebase Service Account JSON in the configured path.
- Set a unique 'GlobalSalt' in the PasswordSettings section.
- Run 'dotnet ef database update' to initialize the schema.

========================================================================
Generated on: 2026-01-04
========================================================================
