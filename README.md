# CampusConnect — Smart Campus Lost & Found

An ASP.NET Core MVC mini-project for reporting lost/found campus items, searching listings, submitting claims, and administering returns.

## Features
- ASP.NET Core MVC architecture (Models, Views, Controllers)
- Entity Framework Core + MySQL (Pomelo provider)
- CRUD for item reports
- Registration/login using ASP.NET Core Identity
- Role-based Student/Admin access
- Data annotations and server-side validation
- Session state for last-login timestamp (demo example)
- Bootstrap responsive UI
- Search/filter listings
- Claim submission and admin review workflow
- Anti-forgery tokens on POST forms

## Requirements
- .NET 8 SDK
- MySQL Community Server
- Optional: VS Code and C# Dev Kit

## 1. Create a MySQL database/user
Sign in to MySQL as an administrator and run:

```sql
CREATE DATABASE campusconnect CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'campus_user'@'localhost' IDENTIFIED BY 'UseAStrongLocalPassword123';
GRANT ALL PRIVILEGES ON campusconnect.* TO 'campus_user'@'localhost';
FLUSH PRIVILEGES;
```

Change `ConnectionStrings:DefaultConnection` in `appsettings.json` to use the password you chose. For a real deployment, move secrets to environment variables or user-secrets and do not commit them.

## 2. Configure the demo administrator
Edit `SeedAdmin` in `appsettings.json` before the first run. Change the example password to a strong unique password. The app creates this admin account if it does not exist. Never use the demo credentials in a public deployment.

## 3. Run the project
From this directory:

```bash
dotnet restore
dotnet run
```

Open the local URL printed in the terminal. On first startup, the app creates the database tables using `EnsureCreated`. If you later change models, use EF Core migrations instead of relying on `EnsureCreated`.

## Database migrations (recommended for development)
After installing the EF tool:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

If `EnsureCreated` has already created the database tables, use a fresh development database before switching to migrations.

## Demo workflow
1. Register a student account.
2. Create a Lost or Found report.
3. Browse and filter listings.
4. Edit/delete your own report.
5. Submit a claim from a different account.
6. Log in with the configured administrator credentials and review claims or update item status.

## Notes
- This is an educational mini-project starter. Test all workflows in your local environment before evaluation.
- The app uses Bootstrap from a CDN, so the browser needs internet access for the styles unless Bootstrap is downloaded locally.
- Only admins can review claims and update report status. Students can edit/delete only their own reports.
- Do not use real sensitive personal information in demo data.
