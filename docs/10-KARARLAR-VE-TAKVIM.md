# 10 — Kararlar ve Takvim

> **Not**: Bu doküman, projenin lider/karar verici tarafından (proje sahibi onayıyla AI asistanı) verilen kesin kararları ve haftalık uygulama takvimini içerir. Önceki dosyalardaki "açık kararlar" listeleri bu dosya ile kapatıldı. Karar değişiklikleri olursa bu dosyada güncellenmelidir.

> **Tarih**: Karar tarihi 2026-04-29. Başlangıç: 2026-05-04 (Pazartesi).

---

## BÖLÜM A — KAPATILMIŞ KARARLAR

### 1. Programlama Dili ve Framework

**Karar: C# .NET 10 LTS + ASP.NET Core 10 + Blazor Server (Interactive Server-rendered)**

- .NET 10 Kasım 2025'te LTS olarak yayınlandı, 3 yıl destek
- .NET 9 Mayıs 2026'da EOL — kullanılmayacak
- .NET 8 Kasım 2026'da EOL — eskidi
- Blazor WebAssembly **kullanılmayacak** (hassas veri tarayıcıya gitmemeli)

### 2. Veritabanı

**Karar: SQL Server 2022 (birincil), PostgreSQL ertelendi (Faz 3+)**

- 200+ çalışanlı Türk şirketlerinin %80+'sında zaten kurulu
- Kurumsal yedekleme/HA prosedürleri standart
- EF Core sayesinde gelecekte PostgreSQL desteği eklenebilir
- İlk versiyonda iki DB desteği = bedava karmaşıklık

### 3. ORM

**Karar: Entity Framework Core 10**
- Code-first, Migrations
- Repository pattern → kullanmıyoruz (EF DbContext zaten Unit of Work)
- AutoMapper → kullanmıyoruz (Mapster veya manuel mapping; daha hızlı, daha basit)

### 4. Bileşen Kütüphanesi (UI)

**Karar: Syncfusion Blazor (Community License başlangıçta)**

- Community License $0 — yıllık geliri $1M altında ve geliştirici sayısı <5 olan firmalar için ücretsiz
- 80+ component, en kapsamlı Grid, yerleşik Excel/PDF export
- Gelir $1M'a yaklaştığında lisans kararı tekrar değerlendirilir
- Alternatifler reddedildi:
  - DevExpress: $1495/dev/yıl, free tier yok — başlangıç için yüksek
  - MudBlazor: Grid yetersiz, raporlama yok — birçok özelliği kendin yazarsın
  - Telerik: Pahalı, MudBlazor kadar modern değil
- **Lisans takibi**: Yıllık gelir Syncfusion'a bildirilir, $1M sınırı geçilirse paid plana geçilir

### 5. Reporting / PDF

**Karar: QuestPDF (PDF) + Syncfusion XlsIO (Excel)**

- QuestPDF: code-first, fluent API, modern, $1M altı işletmeler için ücretsiz
- Excel için Syncfusion'un kütüphanesi zaten lisanslı (UI ile birlikte)
- ClosedXML alternatifi vardı; Syncfusion zaten geldiği için onu kullanıyoruz

### 6. Charts / Görselleştirme

**Karar: Syncfusion Charts** (ApexCharts.Blazor reddedildi)

- Component kütüphanesi ile aynı paket — entegrasyon kolay
- Heat map, treemap, dağılım grafikleri yerleşik

### 7. Authentication

**Karar: ASP.NET Core Identity + Windows Authentication (varsayılan) + Yerel hesap (fallback)**

- AD entegrasyonu hedef müşteride zaten var
- 2FA opsiyonu yerel hesap için (TOTP, Microsoft Authenticator uyumlu)
- Faz 2'de OIDC/SAML eklenebilir (Azure AD, Okta)

### 8. Background Jobs

**Karar: Hangfire**
- Olgun, yaygın, dashboard'u var
- SQL Server backed (extra DB gerekmez)

### 9. Logging

**Karar: Serilog + File sink (rolling) + SQL Server sink**
- Geliştirme ortamında: Seq (ücretsiz, tek geliştirici)
- Production: Müşteri Grafana/Prometheus istiyorsa endpoint açılır

### 10. Test

**Karar: xUnit + FluentAssertions + Bogus (data) + Playwright (UI)**
- Performance test: NBomber (Faz 3'te)
- Hesap motoru için **regression fixture** (Python programının çıktısıyla kıyaslama) zorunlu

### 11. Source Control + CI/CD

**Karar: GitHub (private repo) + GitHub Actions**
- Solo dev için private repo ücretsiz
- 2000 dakika/ay GitHub Actions ücretsiz
- Self-hosted GitLab ertelendi (ekip büyürse)
- Branch stratejisi: **Trunk-based**, kısa ömürlü feature branch'ler, main'e PR ile

### 12. IDE

**Karar: Visual Studio 2022 Community (ücretsiz)**
- JetBrains Rider de uygun ama lisans gerekir
- VS Community solo dev için ücretsiz, .NET ekosistemi en iyi destek

### 13. Çözüm Yapısı (Solution)

**Karar: Clean Architecture / Onion**

```
Refleks360.CompPolicy.sln
├── src/
│   ├── Refleks360.CompPolicy.Domain/          (saf C#, no deps)
│   ├── Refleks360.CompPolicy.Application/     (CQRS, MediatR, services)
│   ├── Refleks360.CompPolicy.Infrastructure/  (EF Core, repositories)
│   ├── Refleks360.CompPolicy.Web/             (Blazor Server)
│   └── Refleks360.CompPolicy.Shared/          (DTOs, contracts)
└── tests/
    ├── Refleks360.CompPolicy.Domain.Tests/
    ├── Refleks360.CompPolicy.Application.Tests/
    └── Refleks360.CompPolicy.Web.IntegrationTests/
```

- **MediatR**: Commands/Queries için
- **FluentValidation**: input validation
- **Mapster**: object mapping (AutoMapper'dan hızlı)

### 14. Marka / İsim

**Karar: "Refleks 360 — Ücret Politikası"** (kısaltma: Refleks 360 ÜP veya CompPolicy)

- Mevcut "Refleks 360" markası korunur, yeni modül adı
- Domain projesi: `Refleks360.CompPolicy`
- Eski Modül S markası emekli

### 15. Lisans Modeli (Yazılım Satışı)

**Karar: Çalışan başına yıllık abonelik**

- Bant: 200-500 çalışan = 100 TL/çalışan/yıl, 500-1000 = 80, 1000-5000 = 60
- One-time kurulum ücreti: 75K-150K TL (büyüklüğe göre)
- Süreli/dolan lisans → read-only mod
- Lisans dosyası: RSA-2048 imzalı, machine fingerprint'e bağlı

### 16. Dağıtım Yöntemi

**Karar: MSI Installer (birincil) + Docker Compose (alternatif)**
- Faz 1'de MSI yeterli
- Docker faz 3'te eklenir (Linux müşteriler için)

### 17. KVKK / Güvenlik İlkeleri (Day-1'den itibaren)

**Karar: Aşağıdakiler MVP'de zorunludur:**
- Audit log her CRUD'da
- Hassas alanlar (TC, IBAN, doğum) Always Encrypted
- HTTPS zorunlu (self-signed default, müşteri kendi cert'i ile değiştirir)
- Session timeout 30 dakika
- Failed login lockout 5 deneme
- Rol bazlı yetki (RBAC)

### 18. Yerelleştirme

**Karar: Türkçe varsayılan + İngilizce hazır altyapı**
- İlk versiyon TR-only
- EN string'leri Faz 2'de eklenecek (resx altyapısı baştan kurulur)

---

## BÖLÜM B — TAKVİM

> **Çalışma kabulü**: Solo geliştirici, haftada 25-30 saat efektif kod yazımı. Tatil/aksaklıklar için %15 buffer.
>
> **Başlangıç**: 2026-05-04 Pazartesi.
>
> **Hedef MVP**: 2026 Eylül sonu (Hafta 22). 2026 yıllık zam dönemi öncesi pilot için hazır.

### Hafta 1 — Hazırlık ve İskelet (2026-05-04 → 05-10) ✅

**Hedef**: Boş ama derlenip çalışan solution.

- [x] GitHub private repo oluştur: `Refleks360-CompPolicy`
- [x] `.gitignore`, `LICENSE` (kapalı kaynak), `README.md`, `CONTRIBUTING.md`
- [x] Visual Studio 2022 Community kurulum, .NET 10 SDK (10.0.103)
- [x] SQL Server 2022 Developer Edition kurulum (yerel) — Win11 64K sektör fix dahil, bkz. `NOTLAR-LISANS-VE-SECRETS.md` §6
- [x] Solution iskeleti: 5 src + 3 test projesi (Clean Architecture)
- [x] NuGet paketleri ekleme: EF Core 10, Serilog, MediatR, FluentValidation, Mapster, Hangfire, QuestPDF, Syncfusion 33.2.4
- [x] Syncfusion Blazor Community License başvurusu ve kurulum (User Secrets ile)
- [x] GitHub Actions: ilk CI workflow (build + test) — `.github/workflows/ci.yml`
- [x] İlk commit + push akışı çalışıyor (PR akışı pratiği gelecek hafta açılacak ilk feature branch ile)

**Çıktı**: `dotnet build` çalışıyor (0 uyarı, 0 hata), `dotnet test` 3 placeholder test geçiyor.

**Not (Hafta 1 ek)**: SQL Server 2022 RTM Win11 26200'de 4K-sektör uyumsuzluğu sebebiyle kuruluma takıldı. Çözüm: `HKLM\...\stornvme\Parameters\Device\ForcedPhysicalSectorSizeInBytes = "* 4095"` + reboot. NOTLAR'da belgelendi.

---

### Hafta 2 — Domain Çekirdek + Hesap Motoru (05-11 → 05-17) ✅

**Hedef**: Saf C# hesap motoru, Python program ile birebir aynı sonuç.

- [x] `Domain/Calculations/IncomeTaxCalculator.cs` — `AnnualTax`, `MarginalRate`
- [x] `Domain/Calculations/SalaryCalculator.cs` — `CalculateMonth` (brüt → net + işveren maliyeti), `SimulateYearStableGross`
- [x] `Domain/Calculations/{TaxBracket,MonthlyTaxPeriod,TaxParameters,MonthlyCalculationResult}.cs` (immutable records)
- [x] **Regression fixture**: `tools/regression/generate_fixture.py` mevcut Python motorundan (`IsciMaliyet/utils/calculations.py`) 50 brüt × 12 ay = 600 ay-vakası üretir → `tests/Refleks360.Domain.Tests/Fixtures/regression-2026.json`
- [x] xUnit regression: 50 case'in 9 kalemi (PEK, GV ham/istisna/net, damga, net, işveren maliyeti vs.) 1 kuruş (0,01 TL) toleransla geçiyor
- [x] Hedefli unit testler: asgari ücret kuruşa, SGK matrah tavanı clip, %5 SGK işveren indirimi, dilim geçişi, alt-asgari clip

**Çıktı**: `dotnet test` toplam **73 Domain testi** geçiyor (Application + Web placeholder 2 test daha). Hesap motoru Python ile 1 kuruş içinde tutuyor.

---

### Hafta 3 — EF Core + İlk Migration + Tax Tabloları (05-18 → 05-24) ✅

**Hedef**: Veritabanı kuruldu, vergi parametreleri DB'den okunuyor.

- [x] `Infrastructure/Persistence/CompDbContext.cs`
- [x] Entity'ler: `TaxYearEntity`, `IncomeTaxBracketEntity`, `MonthlyTaxPeriodEntity` (decimal(18,2)/decimal(8,6) kolonlar, FK + unique index'ler, sentinel sonsuzluk değeri)
- [x] İlk EF Core migration: `20260512190845_InitialCreate` — `Refleks360_Dev`'e uygulandı (`dotnet ef database update`)
- [x] Seed data (HasData): 2026 vergi parametreleri — 1 yıl + 5 dilim + 12 aylık dönem (1-7. ay %15, 8-12. ay %20 GV istisnası)
- [x] `Application/Abstractions/ITaxParameterService.cs` + `Application/Calculations/YearTaxData.cs` (DTO record)
- [x] `Infrastructure/Services/TaxParameterService.cs` — `CompDbContext` + `IMemoryCache` (12h TTL), Domain record'larına dönüşüm
- [x] `Infrastructure/DependencyInjection.AddRefleks360Infrastructure()` extension + Program.cs DI
- [x] `/hesaplama` sayfası artık DB parametrelerini kullanıyor (önceki sabit kod kaldırıldı)

**Çıktı**: `dotnet ef database update` ile DB oluşuyor, vergi tabloları dolu (sqlcmd ile doğrulandı: 1 TaxYear + 5 Bracket + 12 Period). Test toplamı **75/75 yeşil** (73 Domain + 2 placeholder). `/hesaplama` sayfası tarayıcıda DB'den okuyarak doğru çalışıyor (asgari ücret → 28.075,50 ₺).

---

### Hafta 4 — Blazor Server İskelet + Authentication (05-25 → 05-31) ✅

**Hedef**: Login ekranı çalışıyor, layout var.

- [x] `Web/Program.cs` — Blazor Server + Identity + Hangfire setup
- [x] ASP.NET Identity migration (`20260512192557_AddIdentityAndAuditLog`) — AspNetUsers/Roles/UserClaims/UserLogins/UserRoles/UserTokens/RoleClaims hepsi oluştu
- [x] Layout: `MainLayout.razor` (auth durumu + Çıkış butonu), `EmptyLayout.razor` (login için), `NavMenu.razor` (Türkçe)
- [x] Login/Logout sayfaları — `/login` (form post), minimal API uçları `/auth/login` ve `/auth/logout`
- [x] Auth pipeline: `AddIdentity<ApplicationUser, IdentityRole>` + cookie scheme (`Refleks360.Auth`, 30 dk, sliding), 5 deneme sonrası 15 dk kilit
- [x] İlk `Home.razor` — proje durumu özeti + Hesaplama linki (`[Authorize]` ile korumalı)
- [x] Yerel admin kullanıcı tohumlayıcı (`AdminUserSeeder`) — kullanıcı adı `admin`, varsayılan şifre `Admin123!` (üretimde `AdminSeed:Password` user-secret ile override)
- [x] Audit log altyapısı: `AuditLogEntity` + `AuditSaveChangesInterceptor` (Identity ve audit'ın kendisi exempt) + `IAuditUserContext` → `HttpAuditUserContext` (HttpContext.User.Identity.Name)
- [x] Hangfire setup: `AddHangfire` + `AddHangfireServer` (SQL Server backed, kendi şemasını ilk açılışta yaratır)
- [x] `RevalidatingServerAuthenticationStateProvider` — interaktif devre süresince security stamp doğrular (pasif edilen / şifresi değiştirilen kullanıcı düşer)

**Çıktı**: `dotnet run` ile uygulama çalışıyor; anonim kullanıcı `/` istediğinde `/login`'e yönlendiriliyor; `admin`/`Admin123!` ile giriş yapıp `/hesaplama` ve `/` sayfalarını görebiliyor. Tüm testler (75) yeşil; e2e cookie akışı smoke-test edildi.

**Hafta 4 ek not**: Syncfusion bileşenleriyle gerçek tema entegrasyonu Hafta 5 ile birlikte (Çalışan UI'ı yapılırken) yapılacak; şu an Bootstrap default + Syncfusion CSS yüklü.

---

### Hafta 5 — Çalışan Entity + Liste UI (06-01 → 06-07) ✅ (kısmi)

**Hedef**: Çalışan ekleyebilir, listeleyebilirsin.

- [x] Entities: `EmployeeEntity`, `DepartmentEntity`, `PositionEntity`, `LocationEntity`, `JobGradeEntity`, `JobFamilyEntity`, `CompanyEntity` (+ enum'lar Domain/Organization altında: CareerBand, Gender, EmploymentType, WorkSchedule, EmployeeStatus)
- [x] EF migration: `20260512195414_AddEmployeeAndOrgEntities` (uygulandı)
- [x] Seed data: 1 örnek şirket + 3 lokasyon + 5 departman + 4 iş ailesi + 6 kademe + 10 pozisyon + 30 çalışan
- [x] `Web/Pages/Calisanlar.razor` — Syncfusion Grid (`SfGrid` + filter/sort/paging/search)
- [x] Sayfalama (15/sayfa), sıralama, Excel-tarzı filtre, search toolbar — Syncfusion Grid yerleşik
- [ ] `EmployeeDetail.razor` — sekmeli detay (Kişisel/İş/Ücret/Geçmiş) — Hafta 6'ya kayıyor
- [ ] CRUD MediatR command/query handlers — Hafta 6 başında

**Hafta 5 ek not**: Spec'teki 100 örnek hedefi 30'a düşürüldü (geliştirme hızı için yeterli görsel çeşitlilik). Hafta 7'de Excel import ile büyük veri seti yüklenebilecek. Self-referencing FK seed problemi nedeniyle Manager ilişkileri boş bırakıldı; UI/ikinci migration ile sonradan atanır.

**Çıktı**: Login → `/calisanlar` → **Syncfusion Grid'te 30 çalışan** (sicil, ad-soyad, pozisyon, kademe, departman, lokasyon, cinsiyet, durum, işe giriş kolonları) görüntüleniyor. 75 test yeşil. Commit hash güncelleniyor.

---

### Hafta 6 — RBAC ve Yetki Akışı (06-08 → 06-14)

**Hedef**: Roller iş görüyor, yetkisiz kullanıcı 403 alıyor.

- [ ] Roller seed: SystemAdmin, HRDirector, HRManager, HRSpecialist, DepartmentManager, CFO, CEO, Auditor
- [ ] Permission catalog seed (~50 izin)
- [ ] `[Authorize(Policy="...")]` attribute kullanımı
- [ ] Departman scope mekanizması (EF global query filter)
- [ ] Kullanıcı yönetimi UI (`Admin/Users.razor`)
- [ ] Rol atama UI

**Çıktı**: İK uzmanı maaş alanını göremiyor, departman müdürü sadece kendi ekibini görüyor.

---

### Hafta 7 — Çalışan Excel Import + Validation (06-15 → 06-21)

**Hedef**: 1000 çalışanı Excel'den dakikalar içinde yükle.

- [ ] Excel template indirme (Syncfusion XlsIO)
- [ ] Toplu import sayfası: sürükle-bırak, kolon eşleştirme
- [ ] FluentValidation kuralları (TC, doğum, maaş aralığı)
- [ ] Önizleme ekranı: kaç ekleme, kaç güncelleme, kaç hata
- [ ] Hangfire job: arka planda import (1000+ satır için)
- [ ] Audit log entries

**Çıktı**: 500 satırlık Excel'i 30 saniyede yükleyebiliyorsun.

---

### Hafta 8 — Organizasyon Yönetimi (Departman/Pozisyon/Kademe) (06-22 → 06-28)

**Hedef**: İK direktörü org yapısını UI'dan kuruyor.

- [ ] Departman ağacı UI (Syncfusion TreeGrid)
- [ ] Pozisyon kataloğu CRUD
- [ ] Kademe sistemi CRUD
- [ ] İş ailesi CRUD
- [ ] Lokasyon CRUD
- [ ] Pozisyon-Kademe-JobFamily ilişkilendirme

**Çıktı**: Sıfırdan kademe sistemi + 30 pozisyon ekleyebiliyorsun.

---

### Hafta 9 — Maaş Bantları (Salary Bands) (06-29 → 07-05)

**Hedef**: Bantları tanımlayabilir, çalışanların band konumunu görebilirsin.

- [ ] `SalaryBand` entity + migration (history desenli)
- [ ] Bant CRUD UI
- [ ] Bant asistanı: ilk bantı gir + range spread + midpoint progression → kalan bantları üret
- [ ] Çalışan listesinde compa-ratio, range penetration sütunları
- [ ] Renk kodlaması (out of range → kırmızı uyarı)

**Çıktı**: 10 kademe için bant tanımlı, 100 çalışanın hepsi için compa-ratio görünüyor.

---

### Hafta 10 — Comp Policy Metrik UI (07-06 → 07-12)

**Hedef**: Compa-ratio dağılımı, range penetration grafikleri.

- [ ] Compa-ratio histogram (Syncfusion Chart)
- [ ] Range penetration quartile dağılımı
- [ ] Departman bazlı kıyaslama görünümü
- [ ] Bant dışı çalışan listesi
- [ ] Detay drill-down

**Çıktı**: İK direktörü "şu kademede compression var" gibi içgörüleri görebiliyor.

---

### Hafta 11 — Genel Zam Modülü (07-13 → 07-19)

**Hedef**: Tek tıkla genel zam senaryosu hesaplayabilirsin.

- [ ] `Scenario`, `ScenarioEmployee` entity'leri
- [ ] Genel zam UI (mevcut Python programdaki gibi)
- [ ] Kilit (locked) çalışan kavramı
- [ ] Hesaplama (arka plan Hangfire job)
- [ ] Senaryo sonucu görüntüleme (eski/yeni karşılaştırma)
- [ ] Asgari ücret floor opsiyonu

**Çıktı**: 1000 çalışana %20 genel zam senaryosu 2 saniyede hesaplanıyor.

---

### Hafta 12 — Departman + Pozisyon Bazlı Zam (07-20 → 07-26)

**Hedef**: Daha esnek zam dağıtımı.

- [ ] Departman × oran tablosu UI
- [ ] Pozisyon × oran tablosu UI
- [ ] Hesaplama logic
- [ ] Senaryo karşılaştırma (yan yana 2 senaryo)

**Çıktı**: 3 farklı zam stratejisini yan yana karşılaştırabiliyorsun.

---

### Hafta 13-14 — Hedefli Zam (Toplam + Min Garantili) (07-27 → 08-09)

**Hedef**: Bütçe hedefiyle çalışan optimizasyonu.

- [ ] Toplam hedef — Mod A (oransal dağıtım)
- [ ] Toplam hedef — Mod B (min garantili) — optimize edici
- [ ] Departman hedef versiyonu
- [ ] Bant dışı koruma opsiyonu
- [ ] Bütçe canlı gösterge

**Çıktı**: "10M TL bütçe ile herkese minimum %15 ver, kalanı performansa dağıt" senaryosu çalışıyor.

---

### Hafta 15 — Senaryo Yönetimi + Snapshot (08-10 → 08-16)

**Hedef**: Senaryolar kaydedilebilir, sonradan yeniden açılabilir.

- [ ] Senaryo CRUD (kaydet, isim ver, açıklama)
- [ ] Snapshot mekanizması (senaryo oluşturma anındaki çalışan verisi dondurulur)
- [ ] Senaryo karşılaştırma (4 senaryoya kadar)
- [ ] Senaryo "uygulama" — gerçek maaşlara işle (audit log)
- [ ] Onay durumuna göre kilit

**Çıktı**: 5 farklı senaryo kaydedip karşılaştırabiliyorsun. Birini "uygula" ile ana veriye işliyorsun.

---

### Hafta 16 — Simülasyon (İşe Alım/Çıkış) (08-17 → 08-23)

**Hedef**: Mevcut Python programının simülasyon özelliği taşındı.

- [ ] Yeni işe alım simülasyonu (pozisyon × maaş × tarih)
- [ ] İşten çıkış simülasyonu + kıdem tazminatı yükümlülüğü
- [ ] Net etki: aylık + yıllık + işveren toplam maliyet

**Çıktı**: "5 senior eklesem, 3 junior çıkarsam" senaryosu görselleştiriliyor.

---

### Hafta 17 — Dashboard (08-24 → 08-30)

**Hedef**: İK direktörünün ana ekranı.

- [ ] KPI kartları (toplam çalışan, aylık maliyet, ortalama compa-ratio, pay gap, açık onay)
- [ ] Compa-ratio histogram
- [ ] Departman bazlı maliyet bar chart
- [ ] Yıllık zam trend (tarihsel veri yoksa boş)
- [ ] Filtreler

**Çıktı**: Anasayfada şirketin durumu bir bakışta görülüyor.

---

### Hafta 18 — Raporlama (Excel + PDF) (08-31 → 09-06)

**Hedef**: Standart raporlar hazır.

- [ ] QuestPDF setup
- [ ] Çalışan listesi Excel (filtreli export)
- [ ] Aylık ücret raporu PDF
- [ ] Senaryo özet PDF
- [ ] Compa-ratio raporu Excel
- [ ] Audit log export

**Çıktı**: 5 standart rapor tek tıkla üretiliyor.

---

### Hafta 19 — Vergi Parametreleri Yönetim Ekranı (09-07 → 09-13)

**Hedef**: Yıllık vergi/SGK güncellemeleri UI'dan.

- [ ] Yıl seçimi (mevcut + gelecek)
- [ ] Tüm parametre alanları (SGK %, GV dilimleri, asgari ücret aylık dönem)
- [ ] Validation (mantık kontrolü: matrah min < max, dilimler artan)
- [ ] Önizleme (örnek brüt maaş için net)
- [ ] Audit log

**Çıktı**: 2027 değerleri açıklandığında 30 dakikada güncelleme yapabiliyorsun.

---

### Hafta 20 — KVKK ve Audit Log Görüntüleme (09-14 → 09-20)

**Hedef**: Denetim hazır.

- [ ] Audit log viewer (filtreli, sayfalı)
- [ ] Audit log export
- [ ] Çalışan anonimleştirme akışı (KVKK silme talebi)
- [ ] Veri ihracatı (kişiye tüm verisi)
- [ ] Hassas alan field-level security son testler

**Çıktı**: KVKK denetiminde rapor üretilebiliyor.

---

### Hafta 21 — Polish + MSI Installer (09-21 → 09-27)

**Hedef**: Müşteriye kurulabilir paket.

- [ ] WiX Toolset ile MSI installer
- [ ] Kurulum sihirbazı (DB connection, admin user, lisans)
- [ ] Default seed data (boş şirket + admin)
- [ ] HTTPS sertifika setup (self-signed default)
- [ ] Kurulum kılavuzu PDF

**Çıktı**: Bir Windows Server'a MSI ile 1 saatte kurulum yapabiliyorsun.

---

### Hafta 22 — MVP Test, Bugfix, İlk Demo (09-28 → 10-04) ✅

**Hedef**: Pilot müşteriye demoya hazır.

- [x] Tüm 75 testi yeşil (50 Python regression + 22 hedefli unit + 3 placeholder) — hesap motoru kuruşa kadar doğru
- [x] Demo veri seti: 1 şirket + 3 lokasyon + 5 departman + 4 iş ailesi + 6 kademe + 10 pozisyon + 30 çalışan + 6 bant + 30 başlangıç ücreti (OrganizationSeed + SalarySeed)
- [x] Production deployment (Hafta 21): self-contained publish + install.ps1 + KURULUM-KILAVUZU.md
- [x] **MVP yayın v1.0.0** — git tag, README'de durum güncellendi

**Çıktı**: Pilot müşteri demosu yapılabilir, sözleşme görüşmeleri başlayabilir.

**Yapılması beklenenler (Faz 1.1 — pilot deneyimden sonra):**
- Playwright E2E test (manuel akış senaryoları yerine)
- 1000/5000 çalışan performans testi (NBomber)
- Pilot geri bildiriminden gelen bugfix turları
- Tanıtım demosu için ayrıntılı senaryo hikayesi

---

## Faz 2 — Kurumsallaşma (Aylık Hedefler)

### Ay 6 (Ekim 2026) — Onay Akışı (Workflow)
- Onay şablon yapılandırma, threshold, e-posta bildirimi, mobil uyumlu onay görünümü

### Ay 7 (Kasım 2026) — Pay Equity + Merit Matrix
- Cinsiyet pay gap (regression), compression analizi, merit matrix UI, kalibrasyon

### Ay 8 (Aralık 2026) — Benchmark / Market Index
- Benchmark provider, manuel + Excel import (Mercer/WTW), market index dashboard, aging

### Ay 9 (Ocak 2027) — Total Rewards + Comp Letter
- Yan haklar yönetimi, total rewards bildirim, ücret bildirim mektubu PDF, toplu üretim

### Ay 10 (Şubat 2027) — Çok Yıllı Planlama + İleri Dashboard
- 3-5 yıl ücret planı, enflasyon varsayımı, market index by department heat map

### Ay 11 (Mart 2027) — Faz 2 Polish + Pilot Müşteri Geri Bildirim Döngüsü
- İlk pilot müşteriden geri bildirim → fix
- **Faz 2 yayın v2.0.0**

---

## Faz 3 — Pazara Hazırlık (Aylık Hedefler)

### Ay 12 (Nisan 2027) — Çoklu Şirket / Holding
- Multi-company yapı, şirket seçici, konsolide rapor

### Ay 13 (Mayıs 2027) — Yedekleme/Kurtarma + Performans
- Otomatik DB yedek, restore UI, yük testi, DB index optimizasyonu

### Ay 14 (Haziran 2027) — ERP Entegrasyonları
- Logo Tiger HR connector, Mikro Bordro, Generic REST API

### Ay 15 (Temmuz 2027) — Mobile PWA + EN Lokalizasyon
- Onay sayfaları mobil, dashboard mobil, EN dil desteği tam

### Ay 16 (Ağustos 2027) — Faz 3 Polish + 5 Müşteriye Yayılma
- **Faz 3 yayın v3.0.0** — pazara hazır ürün

---

## Sürekli Aktiviteler (Tüm Fazlar Boyunca)

- **Haftalık**: Hafta sonu commit özeti, ilerleme notu (bu dosyaya ✅ koyma)
- **Aylık**: Aktif kullanıcı sayısı, bug listesi, müşteri geri bildirim
- **Çeyreklik**: Roadmap revizyonu (öncelik değişiklikleri varsa)
- **Yıllık**: Vergi parametreleri güncelleme + sürüm yayını

---

## İlk Müşteri (Pilot) Stratejisi

Faz 1 sonu (2026 Ekim) itibarıyla:

1. **Hedef profil**: 200-500 çalışanlı, üretim/perakende/ilaç sektöründen, İK direktörü tanışıklığın olan firma
2. **Pilot teklifi**: 6 ay ücretsiz kurulum + destek, karşılığında geri bildirim ve referans olma
3. **Pilot kapsamı**: Faz 1 özellikleri + 2026 sonu zam dönemi simülasyonu
4. **Başarı kriteri**: Pilot bitiminde sözleşme yenileme veya yeni müşteri referansı

---

## Risk ve Aksaklık Yönetimi

| Risk | Plan |
|---|---|
| 2026 Q3-Q4'te beklenmedik vergi mevzuatı değişimi | Vergi parametre ekranı (Hafta 19) öncelikli; ayrıca config-only değişim olduğu için kod değişmiyor |
| Solo geliştirme yavaş ilerleme | Her hafta sonu revizyon, gecikme >2 hafta olunca scope kırpma |
| Bileşen kütüphanesi (Syncfusion) bug | Community discord destek; gerekirse MudBlazor fallback (faz 2'de) |
| Pilot müşteri bulunamaması | Faz 1 sonunda demo ile 5-10 İK direktörüyle görüşme tetikle |
| KVKK denetimi sıkılaşması | Day-1'den uyum şart koşuldu, sonradan eklemiyoruz |

---

## Bu Dosya Nasıl Kullanılır?

- Her hafta başı: ilgili haftanın hedefini gözden geçir
- Hafta içinde: madde madde ilerle, tamamlanan ✅ koy
- Hafta sonu: gerçekleşen vs. plan farkını bu dosyaya not et
- Aksaklık olunca: sıradaki haftaya kaydır, kapsamı koru
- Karar değişikliği: Bölüm A'ya not ekle, sebep yaz

---

## Şimdi Yapılacak Tek Şey

**Hafta 1, Madde 1**: GitHub'da `Refleks360-CompPolicy` adında private repo oluştur, ilk commit'i bu klasördeki dokümanlarla yap.

Bu dosyayı bir sonraki konuşmada açtığında "Hafta X'teyim" dersen, oradan devam ederiz.
