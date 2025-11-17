# Quick Start Guide - دليل البدء السريع

## English

### Prerequisites
- .NET 8 SDK installed
- Terminal/Command Prompt

### Run in 3 Steps

1. **Navigate to the Web project:**
```bash
cd src/Motel.Web
```

2. **Run the application:**
```bash
dotnet run
```

3. **Open your browser:**
```
http://localhost:5000
```

That's it! The database will be created automatically on first run.

### Default Data
- 10 sample rooms
- 3 sample clients
- Ready to create reservations

### Main Features
- Dashboard: `/Admin/Dashboard`
- Manage Rooms: `/Admin/Rooms`
- Manage Clients: `/Admin/Clients`
- Manage Reservations: `/Admin/Reservations`
- Invoices: `/Admin/Invoices`
- Reports: `/Admin/Reports`

---

## العربية

### المتطلبات
- تثبيت .NET 8 SDK
- Terminal/Command Prompt

### التشغيل في 3 خطوات

1. **انتقل إلى مشروع الويب:**
```bash
cd src/Motel.Web
```

2. **شغّل التطبيق:**
```bash
dotnet run
```

3. **افتح المتصفح:**
```
http://localhost:5000
```

هذا كل شيء! سيتم إنشاء قاعدة البيانات تلقائياً عند التشغيل الأول.

### البيانات الافتراضية
- 10 غرف تجريبية
- 3 نزلاء تجريبيين
- جاهز لإنشاء الحجوزات

### الصفحات الرئيسية
- لوحة التحكم: `/Admin/Dashboard`
- إدارة الغرف: `/Admin/Rooms`
- إدارة النزلاء: `/Admin/Clients`
- إدارة الحجوزات: `/Admin/Reservations`
- الفواتير: `/Admin/Invoices`
- التقارير: `/Admin/Reports`

---

## Troubleshooting / حل المشاكل

### .NET not found / .NET غير موجود
Install .NET 8 SDK from: https://dotnet.microsoft.com/download

### Port already in use / المنفذ مستخدم
Change the port in `Properties/launchSettings.json` or use:
```bash
dotnet run --urls "http://localhost:5555"
```

### Database errors / أخطاء قاعدة البيانات
Delete the `AppData` folder and restart the application.
احذف مجلد `AppData` وأعد تشغيل التطبيق.

---

**Need help? Check README_MOTEL.md for detailed documentation**
**تحتاج مساعدة؟ راجع README_MOTEL.md للتوثيق الكامل**
