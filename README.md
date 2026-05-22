#  SWD392 Grading Tool - Backend API

ASP.NET Core 8 Web API for grading SWD392 practical exam submissions.

---

## Setup

### 1. Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/SWD392-GradingTool.git
cd SWD392-GradingTool
```

### 2. Configure Connection String

Create file `appsettings.Development.json` in `src/SWD392.GradingTool.API/` with your SQL Server connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=GradingToolDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

> Replace the connection string with your own SQL Server configuration.

### 3. Create Database

Make sure EF Core tools are installed:

```bash
dotnet tool install --global dotnet-ef
```

Then run the migration to create the database:

```bash
cd src/SWD392.GradingTool.API
dotnet ef database update
```

### 4. Run

```bash
dotnet run
```

Open **https://localhost:7154/swagger** to access the API documentation.
