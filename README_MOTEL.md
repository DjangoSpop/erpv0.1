# نظام إدارة الموتيل - Motel Management System

نظام متكامل لإدارة موتيل/كامب في دهب مع واجهة إدارية باللغة العربية وقاعدة بيانات محمولة (SQLite).

## المميزات

### الوظائف الأساسية
- ✅ **إدارة الغرف**: إضافة، تعديل، حذف الغرف مع تتبع الحالة
- ✅ **إدارة النزلاء**: قاعدة بيانات كاملة للعملاء
- ✅ **نظام الحجوزات**: حجز الغرف مع منع التضارب
- ✅ **تسجيل الدخول/الخروج**: إدارة كاملة لدورة الإقامة
- ✅ **نظام الفواتير**: إصدار فواتير ضريبية مع حساب تلقائي
- ✅ **التنبيهات**: إشعارات بريد إلكتروني/SMS
- ✅ **التقارير**: لوحة تحكم + تقارير الإشغال والإيرادات
- ✅ **REST API**: نقاط نهاية API للتكامل المستقبلي

### التقنيات المستخدمة
- **.NET 8** - أحدث إصدار من ASP.NET Core
- **EF Core 8** - ORM مع SQLite
- **Clean Architecture** - بنية معمارية نظيفة (4 طبقات)
- **Bootstrap 5 RTL** - واجهة عربية متجاوبة
- **FluentValidation** - التحقق من صحة البيانات
- **Serilog** - تسجيل الأحداث
- **MailKit** - إرسال البريد الإلكتروني

## هيكل المشروع

```
/Motel.sln
/src
  /Motel.Domain          - الكيانات والقيم المعدودة
  /Motel.Application     - DTOs والواجهات والتحققات
  /Motel.Infrastructure  - DbContext والخدمات والتنفيذات
  /Motel.Web             - واجهة الويب والـ API
```

## متطلبات التشغيل

- **.NET 8 SDK** (أو أعلى)
- **أي محرر نصوص** (Visual Studio, VS Code, Rider)
- **متصفح ويب** حديث

## التثبيت والتشغيل

### الطريقة الأولى: باستخدام .NET CLI (الموصى بها)

1. **استنساخ المشروع:**
```bash
cd /path/to/project
```

2. **استعادة الحزم:**
```bash
cd src/Motel.Web
dotnet restore
```

3. **تطبيق قاعدة البيانات (سيتم تلقائياً عند التشغيل الأول):**
```bash
dotnet ef database update --project ../Motel.Infrastructure
```

4. **تشغيل التطبيق:**
```bash
dotnet run
```

5. **فتح المتصفح:**
```
http://localhost:5000
أو
https://localhost:5001
```

### الطريقة الثانية: باستخدام Visual Studio

1. افتح `Motel.sln`
2. اضبط `Motel.Web` كمشروع بدء التشغيل
3. اضغط F5 للتشغيل

## الإعدادات

### قاعدة البيانات

مسار قاعدة البيانات الافتراضي: `./AppData/motel.db`

لتغيير المسار، عدّل في `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=/your/custom/path/motel.db"
  }
}
```

### العلامة التجارية (Branding)

يمكنك تخصيص العلامة التجارية في `appsettings.json`:

```json
{
  "Branding": {
    "SiteName": "موتيل دهب - Dahab Motel",
    "LogoPath": "/images/logo.png",
    "PrimaryColor": "#2c3e50"
  }
}
```

### الضرائب

نسبة الضريبة الافتراضية: 14%

للتغيير:

```json
{
  "Invoice": {
    "DefaultTaxPercent": 14
  }
}
```

### البريد الإلكتروني

لتفعيل إرسال البريد الإلكتروني، قم بتعديل الإعدادات:

```json
{
  "Email": {
    "FromName": "موتيل دهب",
    "FromAddress": "noreply@dahabmotel.com",
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

## البيانات الأولية

يتضمن النظام بيانات تجريبية:
- **10 غرف** بأنواع مختلفة
- **3 نزلاء** للتجربة

## واجهات الإدارة

بعد تشغيل التطبيق، ستتمكن من الوصول إلى:

- **لوحة التحكم**: `/Admin/Dashboard`
- **الغرف**: `/Admin/Rooms`
- **النزلاء**: `/Admin/Clients`
- **الحجوزات**: `/Admin/Reservations`
- **الفواتير**: `/Admin/Invoices`
- **التقارير**: `/Admin/Reports`

## REST API

تتوفر نقاط النهاية التالية:

### الغرف
```
GET    /api/v1/rooms
GET    /api/v1/rooms/{id}
POST   /api/v1/rooms
PUT    /api/v1/rooms/{id}
DELETE /api/v1/rooms/{id}
```

### النزلاء
```
GET    /api/v1/clients
GET    /api/v1/clients/{id}
POST   /api/v1/clients
PUT    /api/v1/clients/{id}
DELETE /api/v1/clients/{id}
```

### الحجوزات
```
GET    /api/v1/reservations
GET    /api/v1/reservations/{id}
POST   /api/v1/reservations
POST   /api/v1/reservations/{id}/check-in
POST   /api/v1/reservations/{id}/check-out
```

### الفواتير
```
GET    /api/v1/invoices/{id}
POST   /api/v1/invoices/{id}/issue
POST   /api/v1/invoices/{id}/mark-paid
```

## المنطق التجاري

### حساب الليالي
```csharp
nights = Math.Max(1, (CheckOutDate - CheckInDate).TotalDays)
```

### حساب الفاتورة
```csharp
Subtotal = (NightlyRate × Nights) + ExtraCharges - DiscountAmount
TaxAmount = Subtotal × (TaxPercent / 100)
Total = Subtotal + TaxAmount
```

### فحص التضارب
الغرفة متاحة إذا:
```csharp
(requestedOut <= existingIn) || (requestedIn >= existingOut)
```

## إعادة الاستخدام

هذا النظام مصمم ليكون قابل لإعادة الاستخدام:

1. **نسخ المشروع** لموقع جديد
2. **تعديل العلامة التجارية** في `appsettings.json`
3. **حذف قاعدة البيانات** القديمة
4. **تشغيل التطبيق** لإنشاء قاعدة بيانات جديدة

يمكن استخدام نفس الكود لـ:
- موتيل
- هوستل
- كامب
- بيت ضيافة
- أي منشأة ضيافة

## استكشاف الأخطاء

### قاعدة البيانات لا تُنشأ
تأكد من أن مجلد `AppData` موجود أو سيتم إنشاؤه تلقائياً.

### خطأ في الترحيل (Migration)
```bash
cd src/Motel.Web
dotnet ef database update --project ../Motel.Infrastructure
```

### البريد الإلكتروني لا يُرسل
تأكد من إعدادات SMTP صحيحة. في التطوير، راجع السجلات (logs).

## السجلات (Logs)

يتم حفظ السجلات في: `logs/motel-YYYYMMDD.log`

## الترخيص

هذا المشروع مفتوح المصدر ويمكن استخدامه لأي غرض تجاري أو شخصي.

## الدعم

للإبلاغ عن مشاكل أو طلب ميزات، افتح issue في المستودع.

---

**تم التطوير بـ ❤️ باستخدام Clean Architecture و .NET 8**
