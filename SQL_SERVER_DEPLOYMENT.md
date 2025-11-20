# SQL Server Deployment Guide - Hotel Management ERP Module

## Overview
This guide explains how to deploy the Motel/Hotel Management System to SQL Server for production use.

---

## Prerequisites

- **SQL Server** (2019 or later) OR **SQL Server Express** OR **LocalDB**
- **.NET 8.0 SDK** installed
- **Entity Framework Core Tools** installed: `dotnet tool install --global dotnet-ef`

---

## Quick Start - LocalDB (Windows Only)

**LocalDB comes with Visual Studio and is perfect for development.**

### Step 1: Use LocalDB Connection
The application is pre-configured for LocalDB. In `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MotelManagement;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

### Step 2: Create Database with Migrations
```bash
cd src/Motel.Web
dotnet ef database update --project ../Motel.Infrastructure
```

### Step 3: Run Application
```bash
dotnet run
```

**That's it!** Application will run at: http://localhost:5000

---

## Production Deployment - SQL Server

### Step 1: Update Connection String

Edit `appsettings.json` (or better, use `appsettings.Production.json`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=MotelManagement;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

**Replace:**
- `YOUR_SERVER` - Your SQL Server instance (e.g., `localhost`, `192.168.1.100`, `sql.yourcompany.com`)
- `YOUR_USER` - SQL Server username
- `YOUR_PASSWORD` - SQL Server password

### Step 2: Create Database

**Option A: Using Migrations (Recommended)**
```bash
cd src/Motel.Web
dotnet ef database update --project ../Motel.Infrastructure
```

**Option B: Manual Script**
```bash
# Generate SQL script
dotnet ef migrations script --project ../Motel.Infrastructure --output setup.sql

# Then run setup.sql in SQL Server Management Studio or Azure Data Studio
```

### Step 3: Verify Database
Check that these tables were created:
- `Clients`
- `Rooms`
- `Reservations`
- `Invoices`
- `NotificationLogs`
- `__EFMigrationsHistory`

### Step 4: Deploy Application

**For IIS:**
```bash
dotnet publish -c Release -o ./publish
# Copy ./publish folder to IIS web server
```

**For Windows Service:**
```bash
dotnet publish -c Release
sc create MotelManagement binPath="path\to\Motel.Web.exe"
sc start MotelManagement
```

**For Docker:**
```bash
docker build -t motel-management .
docker run -d -p 80:8080 motel-management
```

---

## Database Schema Overview

### Core Tables

#### Clients
```sql
CREATE TABLE Clients (
    Id uniqueidentifier PRIMARY KEY,
    FullName nvarchar(200) NOT NULL,
    NationalIdOrPassport nvarchar(50) NOT NULL,
    Phone nvarchar(20) NOT NULL,
    Email nvarchar(100) NULL,
    Address nvarchar(500) NULL
);
```

#### Rooms
```sql
CREATE TABLE Rooms (
    Id uniqueidentifier PRIMARY KEY,
    Number nvarchar(20) NOT NULL UNIQUE,
    Type int NOT NULL, -- Enum: Single=0, Double=1, etc.
    BaseNightlyRate decimal(18,2) NOT NULL,
    Capacity int NOT NULL,
    Status int NOT NULL, -- Enum: Available=0, Occupied=1, etc.
    Notes nvarchar(1000) NULL
);
```

#### Reservations
```sql
CREATE TABLE Reservations (
    Id uniqueidentifier PRIMARY KEY,
    ClientId uniqueidentifier NOT NULL,
    RoomId uniqueidentifier NOT NULL,
    CheckInDate date NOT NULL,
    CheckOutDate date NOT NULL,
    Guests int NOT NULL,
    NightlyRate decimal(18,2) NOT NULL,
    DiscountAmount decimal(18,2) NOT NULL DEFAULT 0,
    ExtraCharges decimal(18,2) NOT NULL DEFAULT 0,
    Status int NOT NULL, -- Enum: Pending=0, Confirmed=1, etc.
    ActualCheckInTime datetime2 NULL,
    ActualCheckOutTime datetime2 NULL,
    Notes nvarchar(1000) NULL,
    CreatedAtUtc datetime2 NOT NULL,

    CONSTRAINT FK_Reservations_Clients FOREIGN KEY (ClientId) REFERENCES Clients(Id),
    CONSTRAINT FK_Reservations_Rooms FOREIGN KEY (RoomId) REFERENCES Rooms(Id)
);
```

#### Invoices
```sql
CREATE TABLE Invoices (
    Id uniqueidentifier PRIMARY KEY,
    Serial nvarchar(50) NOT NULL UNIQUE,
    ReservationId uniqueidentifier NOT NULL,
    Subtotal decimal(18,2) NOT NULL,
    TaxPercent decimal(5,2) NOT NULL,
    TaxAmount decimal(18,2) NOT NULL,
    Total decimal(18,2) NOT NULL,
    Status int NOT NULL, -- Enum: Draft=0, Issued=1, Paid=2, etc.
    PaymentMethod int NULL, -- Enum: Cash=0, Card=1, etc.
    IssuedAtUtc datetime2 NOT NULL,
    PaidAtUtc datetime2 NULL,

    CONSTRAINT FK_Invoices_Reservations FOREIGN KEY (ReservationId) REFERENCES Reservations(Id)
);
```

#### NotificationLogs
```sql
CREATE TABLE NotificationLogs (
    Id uniqueidentifier PRIMARY KEY,
    ReservationId uniqueidentifier NOT NULL,
    Type nvarchar(50) NOT NULL, -- 'Email' or 'SMS'
    Recipient nvarchar(200) NOT NULL,
    Subject nvarchar(200) NULL,
    Message nvarchar(MAX) NOT NULL,
    SentAtUtc datetime2 NOT NULL,
    Success bit NOT NULL,
    ErrorMessage nvarchar(MAX) NULL
);
```

### Indexes
- `IX_Rooms_Number` (Unique)
- `IX_Invoices_Serial` (Unique)
- `IX_Clients_Phone`
- `IX_Clients_NationalIdOrPassport`
- `IX_Reservations_RoomId_CheckInDate_CheckOutDate`
- `IX_NotificationLogs_ReservationId`
- `IX_NotificationLogs_SentAtUtc`

---

## Seed Data

The system automatically seeds:

### 10 Rooms:
- **101** - Single (1 guest) - 300 EGP/night
- **102** - Double (2 guests) - 450 EGP/night
- **103** - Twin (2 guests) - 500 EGP/night
- **104** - Triple (3 guests) - 600 EGP/night
- **105** - Family (4 guests) - 800 EGP/night
- **201** - Dorm (6 guests) - 150 EGP/night
- **202** - Chalet (5 guests) - 1200 EGP/night
- **203** - Double (2 guests) - 450 EGP/night
- **204** - Single (1 guest) - 300 EGP/night
- **205** - Triple (3 guests) - 600 EGP/night

### 3 Sample Clients:
- أحمد محمد علي - 01012345678
- فاطمة حسن - 01023456789
- محمود سعيد - 01098765432

---

## Migrations Management

### Create New Migration
```bash
cd src/Motel.Web
dotnet ef migrations add MigrationName --project ../Motel.Infrastructure
```

### Apply Migrations
```bash
dotnet ef database update --project ../Motel.Infrastructure
```

### Rollback Migration
```bash
dotnet ef database update PreviousMigrationName --project ../Motel.Infrastructure
```

### Remove Last Migration
```bash
dotnet ef migrations remove --project ../Motel.Infrastructure
```

### Generate SQL Script
```bash
dotnet ef migrations script --project ../Motel.Infrastructure --output migrations.sql
```

---

## Connection String Examples

### Windows Authentication (Recommended for internal networks)
```json
"DefaultConnection": "Server=localhost;Database=MotelManagement;Trusted_Connection=true;MultipleActiveResultSets=true"
```

### SQL Server Authentication
```json
"DefaultConnection": "Server=localhost;Database=MotelManagement;User Id=motel_user;Password=SecurePassword123!;TrustServerCertificate=true;MultipleActiveResultSets=true"
```

### Azure SQL Database
```json
"DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Database=MotelManagement;User ID=yourusername;Password=yourpassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

### LocalDB (Development)
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MotelManagement;Trusted_Connection=true;MultipleActiveResultSets=true"
```

### SQL Server Express (Named Instance)
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=MotelManagement;Trusted_Connection=true;MultipleActiveResultSets=true"
```

---

## Troubleshooting

### Error: "Cannot open database"
**Solution:** Ensure SQL Server is running and connection string is correct.
```bash
# Test connection with:
sqlcmd -S localhost -U your_user -P your_password
```

### Error: "Login failed for user"
**Solution:** Check username/password, or use Windows Authentication.

### Error: "A network-related error"
**Solution:**
1. Enable TCP/IP in SQL Server Configuration Manager
2. Restart SQL Server service
3. Check firewall allows port 1433

### Error: "Could not find migrations assembly"
**Solution:**
```bash
dotnet ef database update --project ../Motel.Infrastructure --startup-project ../Motel.Web
```

### Error: "The ConnectionString property has not been initialized"
**Solution:** Verify `appsettings.json` has `ConnectionStrings:DefaultConnection` entry.

---

## Performance Optimization for Production

### 1. Enable Connection Pooling (Default: ON)
Already enabled with `MultipleActiveResultSets=true`

### 2. Add Indexes for Common Queries
```sql
-- Speed up room availability searches
CREATE INDEX IX_Reservations_Dates ON Reservations(CheckInDate, CheckOutDate, RoomId);

-- Speed up invoice searches
CREATE INDEX IX_Invoices_Status_IssuedAt ON Invoices(Status, IssuedAtUtc DESC);

-- Speed up client lookups
CREATE INDEX IX_Clients_Email ON Clients(Email) WHERE Email IS NOT NULL;
```

### 3. Enable Query Statistics Logging
In `appsettings.Production.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### 4. Use Read-Only Connections for Reports
Add a second connection string for reporting:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;ApplicationIntent=ReadWrite",
    "ReportingConnection": "Server=...;ApplicationIntent=ReadOnly"
  }
}
```

---

## Backup Strategy

### Automated Daily Backup (SQL Server Agent)
```sql
USE master;
GO
BACKUP DATABASE MotelManagement
TO DISK = 'C:\Backups\MotelManagement_Full.bak'
WITH FORMAT, INIT, NAME = 'Full Backup of MotelManagement';
GO
```

### Transaction Log Backup (Every Hour)
```sql
BACKUP LOG MotelManagement
TO DISK = 'C:\Backups\MotelManagement_Log.trn'
WITH FORMAT, INIT;
GO
```

### Restore from Backup
```sql
USE master;
GO
RESTORE DATABASE MotelManagement
FROM DISK = 'C:\Backups\MotelManagement_Full.bak'
WITH REPLACE;
GO
```

---

## Security Best Practices

1. **Use Strong Passwords** - Minimum 12 characters, mixed case, numbers, symbols
2. **Limit User Permissions** - Grant only necessary permissions to app user
3. **Enable SSL/TLS** - Use `Encrypt=True` in connection string
4. **Regular Updates** - Keep SQL Server patched
5. **Audit Logging** - Enable SQL Server audit for compliance
6. **IP Restrictions** - Limit connections to app server IPs only

### Create Application User (Recommended)
```sql
USE master;
GO
CREATE LOGIN motel_app WITH PASSWORD = 'YourSecurePassword123!';
GO

USE MotelManagement;
GO
CREATE USER motel_app FOR LOGIN motel_app;
GO

-- Grant necessary permissions
ALTER ROLE db_datareader ADD MEMBER motel_app;
ALTER ROLE db_datawriter ADD MEMBER motel_app;
GO
```

---

## Monitoring

### Check Database Size
```sql
SELECT
    database_name = DB_NAME(database_id),
    size_mb = CAST(SUM(size) * 8. / 1024 AS DECIMAL(10,2))
FROM sys.master_files
WHERE database_id = DB_ID('MotelManagement')
GROUP BY database_id;
```

### Active Connections
```sql
SELECT
    program_name,
    login_name,
    COUNT(*) as connection_count
FROM sys.dm_exec_sessions
WHERE database_id = DB_ID('MotelManagement')
GROUP BY program_name, login_name;
```

### Slow Queries
```sql
SELECT TOP 10
    total_elapsed_time / execution_count / 1000 as avg_elapsed_ms,
    execution_count,
    SUBSTRING(st.text, (qs.statement_start_offset/2)+1,
        ((CASE qs.statement_end_offset
            WHEN -1 THEN DATALENGTH(st.text)
            ELSE qs.statement_end_offset
        END - qs.statement_start_offset)/2) + 1) AS statement_text
FROM sys.dm_exec_query_stats AS qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) AS st
WHERE st.text LIKE '%Motel%'
ORDER BY avg_elapsed_ms DESC;
```

---

## Deployment Checklist

- [ ] SQL Server installed and running
- [ ] Database created with migrations
- [ ] Seed data verified (10 rooms, 3 clients)
- [ ] Connection string configured
- [ ] Application published to production
- [ ] Backup strategy implemented
- [ ] Application user created (not using sa!)
- [ ] Firewall rules configured
- [ ] SSL/TLS enabled
- [ ] Monitoring enabled
- [ ] Test reservation created successfully
- [ ] Test invoice generated successfully

---

## Support

For issues or questions:
1. Check application logs in `logs/` folder
2. Check SQL Server error log
3. Review this deployment guide
4. Contact system administrator

---

**Version:** 1.0
**Last Updated:** 2024-11-19
**Database Schema Version:** 1.0 (InitialCreate migration)
