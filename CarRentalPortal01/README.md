# 🚗 Car Rental Portal (ASP.NET Core MVC)

Bu proje, bir araç kiralama şirketinin envanterini yönetmek için geliştirilmiş web tabanlı bir yönetim portalıdır. 
Yöneticilerin araç eklemesine, listelemesine, güncellemesine ve silmesine (CRUD) olanak tanır.

## 🛠️ Kullanılan Teknolojiler

- **Framework:** ASP.NET Core 9.0 (MVC)
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core (Code-First)
- **Design Pattern:** Repository Pattern & Generic Repository
- **UI Template:** SB Admin 2 (Bootstrap 4 tabanlı)
- **Authentication:** Cookie-Based Authentication
- **Frontend:** Razor Views, HTML5, CSS3, JavaScript, jQuery

## ✨ Temel Özellikler

- **Secure Login:** Yönetici girişi için güvenli Cookie tabanlı kimlik doğrulama.
- **Vehicle Management:** Araçların plakası, markası, modeli ve günlük kira bedeliyle yönetilmesi.
- **Status Tracking:** Araçların müsaitlik durumunun anlık takibi.
- **Responsive Design:** Tüm cihazlarla uyumlu modern yönetici paneli.

## 🚀 Kurulum

1. Bu depoyu klonlayın: `git clone https://github.com/kullaniciadiniz/CarRentalPortal.git`
2. `appsettings.json` dosyasındaki `ConnectionStrings` bölümünü kendi yerel SQL Server bilgilerinizle güncelleyin.
3. **Package Manager Console**'u açın ve şu komutları çalıştırın:
   - `Update-Database`
4. Projeyi derleyin ve çalıştırın (F5).