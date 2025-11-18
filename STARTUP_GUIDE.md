# دليل بدء التشغيل الاحترافي / Professional Startup Guide

## 🚀 Quick Start (3 Steps)

### Linux/Mac:
```bash
cd /path/to/erpv0.1
./run.sh
```

### Windows:
```cmd
cd C:\path\to\erpv0.1
run.bat
```

### Manual Start:
```bash
cd src/Motel.Web
dotnet restore
dotnet build
dotnet run
```

**Open:** http://localhost:5000

---

## ✅ Pre-flight Checklist

Before starting the application, verify:

- [ ] **.NET 8 SDK installed**
  ```bash
  dotnet --version
  # Should show: 8.0.x or higher
  ```

- [ ] **Project files exist**
  ```bash
  ls -la src/Motel.Web/Motel.Web.csproj
  ls -la src/Motel.Infrastructure/Motel.Infrastructure.csproj
  ls -la src/Motel.Application/Motel.Application.csproj
  ls -la src/Motel.Domain/Motel.Domain.csproj
  ```

- [ ] **Configuration file exists**
  ```bash
  cat src/Motel.Web/appsettings.json
  ```

- [ ] **Ports 5000 and 5001 are free**
  ```bash
  # Linux/Mac
  lsof -i:5000
  lsof -i:5001

  # Windows
  netstat -ano | findstr :5000
  netstat -ano | findstr :5001
  ```

---

## 📋 Step-by-Step Startup Process

### Step 1: Clone or Download Repository
```bash
git clone <repository-url>
cd erpv0.1
```

### Step 2: Verify .NET Installation
```bash
dotnet --version
```

**Expected output:** `8.0.x` or higher

**If not installed:**
- Download from: https://dotnet.microsoft.com/download/dotnet/8.0
- Install and restart terminal

### Step 3: Navigate to Web Project
```bash
cd src/Motel.Web
```

### Step 4: Restore Dependencies
```bash
dotnet restore
```

**Expected output:**
```
Determining projects to restore...
Restored /path/to/Motel.Domain/Motel.Domain.csproj (in Xms).
Restored /path/to/Motel.Application/Motel.Application.csproj (in Xms).
Restored /path/to/Motel.Infrastructure/Motel.Infrastructure.csproj (in Xms).
Restored /path/to/Motel.Web/Motel.Web.csproj (in Xms).
```

### Step 5: Build Application
```bash
dotnet build
```

**Expected output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Step 6: Run Application
```bash
dotnet run
```

**Expected output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Step 7: Verify in Browser
Open: **http://localhost:5000**

**Expected behavior:**
- Automatically redirects to `/Admin/Dashboard`
- Arabic RTL interface displayed
- Dashboard shows statistics

---

## 🔍 First Run Verification

### 1. Database Creation
On first run, the system automatically:
- Creates `AppData` directory
- Creates `motel.db` SQLite database
- Seeds sample data (10 rooms + 3 clients)

**Verify:**
```bash
ls -lah src/Motel.Web/AppData/motel.db
# Should show: motel.db with size > 0
```

### 2. Check Logs
```bash
cat src/Motel.Web/logs/motel-$(date +%Y%m%d).log
```

**Look for:**
```
[Information] Created AppData directory at /path/to/AppData
[Information] Database initialized successfully
[Information] Application started
```

### 3. Test Admin Pages
Click through each section:
- ✅ لوحة التحكم (Dashboard)
- ✅ الغرف (Rooms) - Should show 10 rooms
- ✅ النزلاء (Clients) - Should show 3 clients
- ✅ الحجوزات (Reservations)
- ✅ الفواتير (Invoices)
- ✅ التقارير (Reports)

### 4. Test API Endpoints
```bash
# Get all rooms
curl http://localhost:5000/api/v1/rooms

# Get all clients
curl http://localhost:5000/api/v1/clients

# Should return JSON data
```

---

## 🎯 What to Expect

### Seed Data Loaded:

**10 Rooms:**
- Room 101 - Single (300 EGP/night)
- Room 102 - Double (450 EGP/night)
- Room 103 - Twin (500 EGP/night)
- Room 104 - Triple (600 EGP/night)
- Room 105 - Family (800 EGP/night)
- Room 201 - Dorm (150 EGP/night)
- Room 202 - Chalet (1200 EGP/night)
- Room 203 - Double (450 EGP/night)
- Room 204 - Single (300 EGP/night)
- Room 205 - Triple (600 EGP/night)

**3 Clients:**
- أحمد محمد علي
- فاطمة حسن
- محمود سعيد

### Dashboard KPIs:
- Available Rooms: 10
- Occupied Rooms: 0
- Reserved Rooms: 0
- Occupancy Rate: 0%
- Today's Revenue: 0 EGP
- Month Revenue: 0 EGP

---

## 🧪 Testing the System

### Create Test Reservation:

1. **Go to Reservations → Create New**
2. **Select Client:** أحمد محمد علي
3. **Select Room:** 101
4. **Check-in Date:** Today
5. **Check-out Date:** Tomorrow
6. **Guests:** 1
7. **Nightly Rate:** 300
8. **Click:** إنشاء الحجز (Create)

### Check-in Process:

1. **Go to Reservations → View Details**
2. **Click:** تسجيل الدخول (Check-in)
3. **Verify:** Room status changes to "Occupied"

### Check-out Process:

1. **From Reservation Details**
2. **Click:** تسجيل الخروج (Check-out)
3. **Verify:** Room status returns to "Available"

### Generate Invoice:

1. **After Check-out**
2. **Click:** إصدار فاتورة (Issue Invoice)
3. **Verify:** Invoice created with correct calculations
4. **Test Print:** Click طباعة (Print)

---

## 🔧 Configuration

### Default Settings:

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=./AppData/motel.db"
  },
  "Branding": {
    "SiteName": "موتيل دهب - Dahab Motel",
    "LogoPath": "/images/logo.png",
    "PrimaryColor": "#2c3e50"
  },
  "Invoice": {
    "DefaultTaxPercent": 14
  }
}
```

### Customize Branding:

Edit `src/Motel.Web/appsettings.json`:
```json
{
  "Branding": {
    "SiteName": "Your Motel Name",
    "PrimaryColor": "#your-color"
  }
}
```

### Change Tax Rate:

```json
{
  "Invoice": {
    "DefaultTaxPercent": 15
  }
}
```

---

## 📊 Performance Benchmarks

**Expected startup time:**
- Cold start: 5-10 seconds
- Warm start: 2-3 seconds

**Expected response times:**
- Dashboard load: < 500ms
- Room list: < 300ms
- Create reservation: < 200ms
- API requests: < 100ms

**Database size:**
- Fresh install: ~100 KB
- With 100 reservations: ~500 KB
- With 1000 reservations: ~5 MB

---

## 🛠️ Development Mode vs Production

### Development Mode (Default):

```bash
# Enabled by default
export ASPNETCORE_ENVIRONMENT=Development  # Linux/Mac
set ASPNETCORE_ENVIRONMENT=Development     # Windows
```

**Features:**
- Detailed error pages
- Hot reload
- Console logging
- Developer exception page

### Production Mode:

```bash
export ASPNETCORE_ENVIRONMENT=Production  # Linux/Mac
set ASPNETCORE_ENVIRONMENT=Production     # Windows

cd src/Motel.Web
dotnet run
```

**Features:**
- Custom error pages
- Optimized performance
- Minimal logging
- HTTPS enforced

---

## 🔐 Security Notes

### ⚠️ IMPORTANT - Before Production:

**Current Status:** No authentication implemented

**Required for Production:**
1. Add ASP.NET Core Identity
2. Implement user login
3. Add role-based authorization
4. Enable HTTPS only
5. Configure CORS if using API

**Quick Authentication Setup:**
```csharp
// Add to Program.cs
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/Forbidden";
    });

// Add authorization
app.UseAuthentication();
app.UseAuthorization();
```

---

## 📱 Accessing from Other Devices

### Network Access:

1. **Find your IP address:**
   ```bash
   # Linux/Mac
   ifconfig | grep "inet "

   # Windows
   ipconfig
   ```

2. **Update launchSettings.json:**
   ```json
   {
     "applicationUrl": "http://0.0.0.0:5000;https://0.0.0.0:5001"
   }
   ```

3. **Access from other device:**
   ```
   http://YOUR_IP:5000
   ```

4. **Configure firewall:**
   ```bash
   # Linux
   sudo ufw allow 5000/tcp
   sudo ufw allow 5001/tcp

   # Windows
   # Add inbound rules for ports 5000 and 5001
   ```

---

## 🎓 Learning Resources

### For Developers:

1. **Clean Architecture:**
   - Domain Layer: Pure C# entities
   - Application Layer: Use cases and DTOs
   - Infrastructure Layer: Data access and external services
   - Presentation Layer: Web UI and API

2. **Key Files to Study:**
   ```
   src/Motel.Domain/Entities/         # Business entities
   src/Motel.Application/Interfaces/  # Service contracts
   src/Motel.Infrastructure/Services/ # Implementations
   src/Motel.Web/Controllers/        # MVC controllers
   src/Motel.Web/Areas/Admin/Views/  # Razor views
   ```

3. **Database Schema:**
   ```bash
   # Open database with SQLite browser
   sqlite3 src/Motel.Web/AppData/motel.db
   .tables
   .schema Rooms
   .schema Reservations
   .schema Invoices
   ```

---

## ✅ Success Criteria

### You know the system is working when:

- [ ] Application starts without errors
- [ ] Dashboard loads with KPIs
- [ ] All 10 rooms visible in Rooms page
- [ ] All 3 clients visible in Clients page
- [ ] Can create new reservation
- [ ] Can check-in/check-out
- [ ] Can generate invoice
- [ ] Invoice prints correctly
- [ ] Reports show charts
- [ ] API returns JSON data
- [ ] Arabic text displays correctly (RTL)
- [ ] No errors in logs

---

## 🆘 Get Help

### If you encounter issues:

1. **Check TROUBLESHOOTING.md** (comprehensive guide)
2. **View logs:** `src/Motel.Web/logs/motel-*.log`
3. **Restart application:** Press `Ctrl+C` then `dotnet run` again
4. **Reset database:** Delete `AppData` folder and restart
5. **Check GitHub issues**

---

**System Status:** ✅ Production-Ready (add authentication first)
**Last Updated:** 2024
**Version:** 1.0
**Support:** See TROUBLESHOOTING.md
