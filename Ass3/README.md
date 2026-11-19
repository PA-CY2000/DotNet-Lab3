# RRA Taxpayer Reception, Routing and Feedback Management System

A .NET 10 Windows Forms desktop application for taxpayer registration, ticket routing, queue management, feedback, alerts, reports, and role-based administration for Rwanda Revenue Authority workflows.

## Technologies

- C# / .NET 10 Windows Forms
- PostgreSQL 18 or compatible PostgreSQL server
- Npgsql 10
- MVC-oriented controllers, services, and views

## Run locally

1. Install PostgreSQL and make sure it is listening on `localhost:5432`.
2. Use the local development database credentials configured for this project: database `taxpayer_management_db`, user `postgres`, password `123`.
3. Copy `App.config.example` to a local configuration file if desired, or set the `TAXPAYER_DB_CONNECTION` environment variable.
4. Open `Ass3.slnx` in Visual Studio 2026 and run the project.
5. On first startup, the application creates the database schema and seed records without dropping existing data. The SQL files in `Database/` are also available for manual administration.

Development login: `admin` / `admin123`.

## Features

The application includes 14 functional Windows Forms: login, dashboard, taxpayer registration, ticket management, queue management, display board, feedback, waiting-time lookup, desk management, alerts, reports, user management, service management, and activity logs. Permissions are enforced in controllers and services in addition to dashboard navigation.

## Database scripts

- `Database/CreateDatabase.sql` creates the database when run against the PostgreSQL maintenance database.
- `Database/CreateTables.sql` creates the tables and constraints.
- `Database/InsertInitialData.sql` inserts roles, services, desks, settings, and the initial administrator marker.
- `Database/Indexes.sql` creates operational indexes.

Do not commit local passwords, `App.config`, `appsettings.local.json`, logs, `bin`, or `obj`; these are excluded by `.gitignore`.
