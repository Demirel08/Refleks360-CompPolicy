# Refleks 360 — Ücret Politikası Yönetim Sistemi

> 200+ çalışanlı Türk şirketleri için on-premise, çoklu kullanıcılı **ücret politikası (compensation policy)** yönetim platformu.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/) [![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4)](https://learn.microsoft.com/aspnet/core/blazor/) [![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927)](https://www.microsoft.com/sql-server) [![License](https://img.shields.io/badge/License-Proprietary-red)](LICENSE)

---

## Bu Repo Nedir?

Refleks 360, Türkiye'deki kurumsal şirketlerin İK departmanlarının **ücret politikasını** kurması ve yönetmesi için tasarlanan, müşterinin kendi sunucusuna kurulan bir web uygulamasıdır. Kademe (job grade) sistemi, maaş bantları, piyasa karşılaştırması, merit cycle yönetimi, pay equity analizi ve senaryo modelleme yapar.

> **Bordro değildir.** Bordro/payroll Logo, Mikro, Netsis vb. ile yapılır. Bu ürün ücret **politikası** ürünüdür.

---

## Durum: MVP v1.0.0 — yayına hazır

22 haftalık MVP planı **tamamlandı** (bkz. [docs/10-KARARLAR-VE-TAKVIM.md](docs/10-KARARLAR-VE-TAKVIM.md)). Aşağıdaki tüm modüller çalışır halde:

- **Kimlik + RBAC**: 8 sistem rolü, 19 izin, cookie auth, audit log
- **Organizasyon**: Şirket, lokasyon, departman, iş ailesi, kademe, pozisyon CRUD
- **Çalışan yönetimi**: Liste (Syncfusion Grid + arama/filtre/sayfalama), detay (sekmeli), CRUD, Excel import (önizleme + apply), KVKK anonimleştirme + veri ihrac
- **Maaş bantları**: CRUD, otomatik bant asistanı (range spread + mid progression), lokasyon kırılımı, compa-ratio + range penetration hesabı
- **Comp policy metrikleri**: Histogram, quartile, ortalama/medyan compa, bant dışı listesi, cinsiyet pay gap
- **Senaryolar**: Genel zam, departman/pozisyon bazlı zam, hedefli bütçe (mod A oransal + mod B min garantili), uygula akışı, PDF export
- **Simülasyon**: İşe alım + ayrılış + kıdem tazminatı yükümlülüğü
- **Dashboard**: 5 KPI kartı, departman maliyet grafiği, hızlı geçiş
- **Raporlama**: Çalışan listesi Excel, comp policy Excel, aylık ücret PDF, senaryo PDF
- **Vergi parametreleri**: Yıl bazlı yönetim, dilimler, aylık dönemler, yıl kopyalama
- **Audit log viewer**: Filtreli arama + Excel export
- **Dağıtım**: Self-contained Windows publish + PowerShell installer (Windows Service) + kurulum kılavuzu

**Testler:** 75/75 yeşil (50 Python regression + 22 hedefli unit + 3 placeholder)

### Geliştirme Önkoşulları

- Windows 10/11 veya Windows Server 2022
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Visual Studio 2022 Community](https://visualstudio.microsoft.com/) (veya Rider)
- [SQL Server 2022 Developer Edition](https://www.microsoft.com/sql-server/sql-server-downloads)
- Git 2.50+

### Klonlama + çalıştırma

```bash
git clone https://github.com/Demirel08/Refleks360-CompPolicy.git
cd Refleks360-CompPolicy

# user-secrets (geliştirme):
cd src/Refleks360.Web
dotnet user-secrets set "Syncfusion:LicenseKey" "<SENIN_KEY>"
dotnet user-secrets set "ConnectionStrings:Default" "Server=(local);Database=Refleks360_Dev;Integrated Security=true;TrustServerCertificate=true"

# DB:
dotnet ef database update --project ../Refleks360.Infrastructure --startup-project .

# Çalıştır:
dotnet run
# http://localhost:5064 — admin / Admin123!
```

### Production kurulum

`deploy/publish.ps1` → ZIP üretir → hedef sunucuya kopyala → `deploy/install.ps1` ile servisleştir. Tam akış: [docs/KURULUM-KILAVUZU.md](docs/KURULUM-KILAVUZU.md).

---

## Dokümantasyon

Ürünün tam spesifikasyonu `docs/` altında. Bir AI asistanı (veya yeni geliştirici) bu dosyaları sırayla okuduğunda projeyi anlayabilmelidir.

| # | Dosya | İçerik |
|---|---|---|
| 0 | [docs/README.md](docs/README.md) | Dokümantasyon giriş ve navigasyon |
| 1 | [docs/01-PROJE-VIZYONU.md](docs/01-PROJE-VIZYONU.md) | Vizyon, hedef pazar, iş modeli |
| 2 | [docs/02-TEKNIK-MIMARI.md](docs/02-TEKNIK-MIMARI.md) | Stack, dağıtım, güvenlik |
| 3 | [docs/03-MODULLER-VE-EKRANLAR.md](docs/03-MODULLER-VE-EKRANLAR.md) | Modül/sayfa detayları |
| 4 | [docs/04-VERI-MODELI.md](docs/04-VERI-MODELI.md) | Tablolar, ilişkiler, EF Core |
| 5 | [docs/05-HESAPLAMA-MOTORU.md](docs/05-HESAPLAMA-MOTORU.md) | Vergi/SGK/comp policy formülleri |
| 6 | [docs/06-ROLLER-VE-YETKILER.md](docs/06-ROLLER-VE-YETKILER.md) | RBAC matrisi |
| 7 | [docs/07-MEVCUT-PROGRAMDAN-MIRAS.md](docs/07-MEVCUT-PROGRAMDAN-MIRAS.md) | Python programdan miras |
| 8 | [docs/08-YAPILACAKLAR.md](docs/08-YAPILACAKLAR.md) | Faz bazlı TODO |
| 9 | [docs/09-REFERANSLAR.md](docs/09-REFERANSLAR.md) | Dış kaynaklar, rakipler |
| 10 | [docs/10-KARARLAR-VE-TAKVIM.md](docs/10-KARARLAR-VE-TAKVIM.md) | **Kararlar + 22 haftalık MVP takvimi** |

---

## Teknoloji Yığını

- **.NET 10** + **ASP.NET Core 10** + **Blazor Server** (interactive server-rendered)
- **SQL Server 2022** + **Entity Framework Core 10** (Code-First, Always Encrypted)
- **Syncfusion Blazor** (Community License) — UI bileşenleri
- **QuestPDF** + **Syncfusion XlsIO** — PDF/Excel raporlama
- **Hangfire** — arkaplan iş kuyruğu
- **Serilog** — yapısal loglama
- **MediatR** + **FluentValidation** + **Mapster** — uygulama katmanı
- **xUnit** + **FluentAssertions** + **Bogus** + **Playwright** — test yığını
- **WiX Toolset v4** — MSI installer

Detay: [docs/02-TEKNIK-MIMARI.md](docs/02-TEKNIK-MIMARI.md)

---

## Mimari Yapı (Hedef)

```
Refleks360-CompPolicy/
├─ src/
│  ├─ Refleks360.Domain/           # Entity'ler, value object'ler, domain servisler
│  ├─ Refleks360.Application/      # Use case'ler, MediatR handler'ları
│  ├─ Refleks360.Infrastructure/   # EF Core, dış servisler, Hangfire
│  └─ Refleks360.Web/              # Blazor Server UI + API endpoints
├─ tests/
│  ├─ Refleks360.Domain.Tests/
│  ├─ Refleks360.Application.Tests/
│  └─ Refleks360.Web.Tests/        # Playwright + integration
├─ docs/                           # Bu dokümantasyon
└─ build/                          # CI/CD scripts, WiX installer
```

Clean Architecture / Onion yaklaşımı. Domain bağımsız, Application Domain'e bağımlı, Infrastructure ve Web Application'a bağımlı.

---

## İlkeler (Değişmez)

1. **Veri müşterinin sunucusunda kalır.** SaaS değildir, on-premise web uygulamasıdır.
2. **Sektöre özel kod yazılmaz.** Her sektör için aynı program; benchmark verisini müşteri girer.
3. **Bordro yapmaz.** Politika ürünüdür, işlemci değildir.
4. **Sayısal doğruluk kritiktir.** Vergi/SGK hesaplamaları test fixture'ları ile garanti altında.
5. **Çoklu kullanıcı zorunludur.** RBAC ile rol bazlı erişim.
6. **KVKK uyumludur.** Audit log + kişisel veri silme desteği.

---

## Lisans

Proprietary — bkz. [LICENSE](LICENSE). İzinsiz kullanım, çoğaltma veya dağıtım yasaktır.

İletişim: okandemirel08@gmail.com
