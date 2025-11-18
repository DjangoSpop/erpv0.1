# دليل حل المشاكل / Troubleshooting Guide

## مشاكل شائعة وحلولها / Common Issues and Solutions

---

### 1. خطأ: ".NET SDK not found" / Error: ".NET SDK not found"

**المشكلة / Problem:**
```bash
bash: dotnet: command not found
```

**الحل / Solution:**

#### Windows:
1. قم بتحميل .NET 8 SDK من: https://dotnet.microsoft.com/download
2. شغّل المثبّت
3. أعد تشغيل Command Prompt
4. تحقق: `dotnet --version`

#### Linux/Mac:
```bash
# Ubuntu/Debian
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version 8.0

# Mac (using Homebrew)
brew install --cask dotnet-sdk
```

---

### 2. خطأ: "The type or namespace name 'FluentValidation' could not be found"

**المشكلة / Problem:**
```
CS0246: The type or namespace name 'FluentValidation' could not be found
```

**الحل / Solution:**
```bash
cd src/Motel.Web
dotnet restore
dotnet build
```

---

### 3. خطأ: "Database migration failed" / خطأ في قاعدة البيانات

**المشكلة / Problem:**
```
An error occurred while migrating the database
```

**الحل / Solution:**

#### Option 1: حذف قاعدة البيانات وإعادة إنشائها
```bash
# احذف المجلد
rm -rf src/Motel.Web/AppData

# شغّل التطبيق من جديد
cd src/Motel.Web
dotnet run
```

#### Option 2: إعادة إنشاء الجداول يدوياً
```bash
cd src/Motel.Web
dotnet ef database drop --project ../Motel.Infrastructure --force
dotnet ef database update --project ../Motel.Infrastructure
```

---

### 4. خطأ: "Port 5000 is already in use" / المنفذ مستخدم

**المشكلة / Problem:**
```
System.IO.IOException: Failed to bind to address http://127.0.0.1:5000
```

**الحل / Solution:**

#### Windows:
```cmd
# إيقاف العملية التي تستخدم المنفذ
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# أو استخدم منفذ آخر
dotnet run --urls "http://localhost:5555"
```

#### Linux/Mac:
```bash
# إيقاف العملية التي تستخدم المنفذ
lsof -ti:5000 | xargs kill -9

# أو استخدم منفذ آخر
dotnet run --urls "http://localhost:5555"
```

---

### 5. خطأ: "Could not find a part of the path './AppData/motel.db'"

**المشكلة / Problem:**
المسار غير موجود أو الصلاحيات غير كافية

**الحل / Solution:**
```bash
# إنشاء المجلد يدوياً
cd src/Motel.Web
mkdir -p AppData

# على Windows
mkdir AppData

# تأكد من الصلاحيات (Linux/Mac)
chmod 755 AppData
```

---

### 6. خطأ: "Serilog configuration error" / خطأ في Serilog

**المشكلة / Problem:**
```
Unrecognized property 'Using' in Serilog configuration
```

**الحل / Solution:**

تحقق من وجود حزم Serilog:
```bash
cd src/Motel.Web
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Console
```

---

### 7. خطأ: "Arabic text appears as boxes" / النصوص العربية تظهر كمربعات

**المشكلة / Problem:**
الخطوط العربية غير معروضة بشكل صحيح

**الحل / Solution:**

1. تأكد من استخدام متصفح حديث (Chrome, Firefox, Edge)
2. تأكد من Bootstrap RTL:
   ```html
   <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.rtl.min.css">
   ```
3. تأكد من خاصية RTL في HTML:
   ```html
   <html lang="ar" dir="rtl">
   ```

---

### 8. خطأ: "Authentication failed" / فشل المصادقة

**المشكلة / Problem:**
لا يمكن الوصول إلى لوحة الإدارة

**الحل / Solution:**

**ملاحظة هامة:** النظام الحالي **بدون مصادقة** - يمكن الدخول مباشرة!

للإنتاج، يجب إضافة مصادقة:
```csharp
// في Program.cs
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Admin/Login";
    });
```

---

### 9. خطأ: "Connection refused" / رفض الاتصال

**المشكلة / Problem:**
```
ERR_CONNECTION_REFUSED when accessing http://localhost:5000
```

**الحل / Solution:**

1. **تحقق من تشغيل التطبيق:**
   ```bash
   # يجب أن ترى
   info: Microsoft.Hosting.Lifetime[14]
         Now listening on: http://localhost:5000
   ```

2. **تحقق من جدار الحماية:**
   - Windows: أضف استثناء لـ dotnet.exe
   - Linux: `sudo ufw allow 5000/tcp`

3. **استخدم HTTPS بدلاً من HTTP:**
   ```
   https://localhost:5001
   ```

---

### 10. خطأ: "Validation not working" / التحقق لا يعمل

**المشكلة / Problem:**
نماذج الإدخال لا تظهر رسائل الخطأ

**الحل / Solution:**

1. **تحقق من jQuery:**
   ```html
   <!-- يجب أن يكون موجوداً قبل validation scripts -->
   <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
   ```

2. **تحقق من asp-validation-for:**
   ```html
   <span asp-validation-for="FullName" class="text-danger"></span>
   ```

3. **تحقق من @section Scripts:**
   ```html
   @section Scripts {
       @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
   }
   ```

---

## الأوامر المفيدة / Useful Commands

### تنظيف المشروع / Clean Project
```bash
dotnet clean
dotnet build
```

### إعادة بناء قاعدة البيانات / Rebuild Database
```bash
cd src/Motel.Web
rm -rf AppData
dotnet run
```

### عرض السجلات / View Logs
```bash
# Linux/Mac
tail -f src/Motel.Web/logs/motel-*.log

# Windows
type src\Motel.Web\logs\motel-*.log
```

### تحديث الحزم / Update Packages
```bash
dotnet list package --outdated
dotnet add package <PackageName> --version <Version>
```

### إنشاء ملف النشر / Create Publish
```bash
cd src/Motel.Web
dotnet publish -c Release -o ../../publish
```

---

## فحص صحة النظام / System Health Check

### Windows PowerShell:
```powershell
# 1. Check .NET
dotnet --version

# 2. Check port availability
Test-NetConnection -ComputerName localhost -Port 5000

# 3. Check database file
Test-Path "src\Motel.Web\AppData\motel.db"

# 4. Check log files
Get-ChildItem "src\Motel.Web\logs" -Filter "motel-*.log"
```

### Linux/Mac Bash:
```bash
# 1. Check .NET
dotnet --version

# 2. Check port availability
nc -zv localhost 5000

# 3. Check database file
ls -lah src/Motel.Web/AppData/motel.db

# 4. Check log files
ls -lah src/Motel.Web/logs/
```

---

## طلب المساعدة / Getting Help

### قبل طلب المساعدة، جمّع المعلومات التالية:

1. **نسخة .NET:**
   ```bash
   dotnet --version
   ```

2. **نظام التشغيل:**
   ```bash
   # Linux/Mac
   uname -a

   # Windows
   systeminfo | findstr /B /C:"OS Name" /C:"OS Version"
   ```

3. **رسائل الخطأ الكاملة:**
   ```bash
   # من السجلات
   cat src/Motel.Web/logs/motel-$(date +%Y%m%d).log
   ```

4. **خطوات إعادة إنتاج المشكلة**

---

## اختبار سريع / Quick Test

### تحقق من أن كل شيء يعمل:

```bash
# 1. Navigate to project
cd /path/to/erpv0.1

# 2. Run quick test
./run.sh   # Linux/Mac
run.bat    # Windows

# 3. Open browser
# http://localhost:5000

# 4. Check these URLs:
# http://localhost:5000/Admin/Dashboard  ✓
# http://localhost:5000/Admin/Rooms      ✓
# http://localhost:5000/Admin/Clients    ✓
# http://localhost:5000/api/v1/rooms     ✓
```

### إذا فتحت جميع الصفحات بنجاح، النظام يعمل! ✅

---

## المراجع / References

- .NET Documentation: https://docs.microsoft.com/dotnet
- Entity Framework Core: https://docs.microsoft.com/ef/core
- ASP.NET Core MVC: https://docs.microsoft.com/aspnet/core/mvc
- Bootstrap RTL: https://getbootstrap.com

---

**آخر تحديث / Last Updated:** 2024
**الإصدار / Version:** 1.0
