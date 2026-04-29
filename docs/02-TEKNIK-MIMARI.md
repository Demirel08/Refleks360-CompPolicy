# 02 — Teknik Mimari

## Stack Özeti

```
┌──────────────────────────────────────────────────────────────┐
│  Sunum Katmanı: Blazor Server (interactive server-rendered) │
│  ─ SignalR ile gerçek zamanlı güncelleme                    │
│  ─ Bileşen kütüphanesi: Syncfusion veya DevExpress Blazor   │
│  ─ Grafik: ApexCharts.Blazor                                │
└──────────────────────────────────────────────────────────────┘
                            ▲
                            │ DI / Mediator
                            ▼
┌──────────────────────────────────────────────────────────────┐
│  Uygulama Katmanı: ASP.NET Core 9 + MediatR                 │
│  ─ Authentication: ASP.NET Identity + Windows/AD/OIDC       │
│  ─ Authorization: Policy-based + Role-based (RBAC)          │
│  ─ Validation: FluentValidation                             │
│  ─ Background jobs: Hangfire (uzun hesap, e-posta, rapor)  │
└──────────────────────────────────────────────────────────────┘
                            ▲
                            │
                            ▼
┌──────────────────────────────────────────────────────────────┐
│  Domain Katmanı: Comp.Domain (saf C# class library)         │
│  ─ Hesap motoru: vergi, SGK, comp metrikleri                │
│  ─ Domain modelleri: Employee, Grade, Band, Scenario...     │
│  ─ Hiçbir framework bağımlılığı yok — test edilebilir       │
└──────────────────────────────────────────────────────────────┘
                            ▲
                            │
                            ▼
┌──────────────────────────────────────────────────────────────┐
│  Veri Katmanı: Entity Framework Core 9                      │
│  ─ Birincil DB: SQL Server (büyük şirketlerde standart)     │
│  ─ Alternatif: PostgreSQL (Docker dağıtımı için)            │
│  ─ Migrations: EF Core Migrations                           │
│  ─ Audit: EF interceptor + AuditLog tablosu                 │
└──────────────────────────────────────────────────────────────┘
```

---

## Dil ve Framework Seçimi

### Neden C# .NET 9?
- Windows Server'da en olgun ekosistem
- Türk kurumsal şirketlerin IT departmanlarının aşina olduğu stack
- DevExpress, Syncfusion gibi olgun grid/component ekosistemi
- Excel/PDF kütüphaneleri (ClosedXML, EPPlus, QuestPDF) endüstri standardı
- Async/await, LINQ, Records — modern dil özellikleri
- Long-term support (LTS): .NET 9 → .NET 10 LTS yolculuğu net

### Neden Blazor Server (Blazor WebAssembly değil)?
- **Server-rendered**: hassas ücret verisi tarayıcıya hiç gitmez
- C# ile hem backend hem frontend yazılır → tek dil, paylaşılan modeller
- SignalR ile gerçek zamanlı güncelleme bedava (eş zamanlı düzenleme bildirimi)
- WebAssembly'in ilk yükleme süresi (~3-5 saniye) iş uygulaması için olumsuz
- LAN içi kullanımda Server modu performans olarak Web'den iyi

### Neden ASP.NET Core 9 ve değil Node/Python?
- Statik tipli; kurumsal yazılımda hata yüzeyi düşer
- Microsoft uzun vadeli destek
- IIS/Kestrel ile Windows Server kurulumu kolay
- Performance: Python/Node'dan 5-20x hızlı

---

## Veritabanı

### Birincil seçim: SQL Server
- 200+ çalışanlı Türk şirketlerinin %80'inde mevcut
- Lisansları zaten alınmış (genelde mevcut ERP için)
- Kurumsal yedekleme/HA prosedürleri oturmuş
- MS SQL Always On, mirroring, log shipping olgun

### Alternatif: PostgreSQL
- Docker tabanlı dağıtım için
- Lisans maliyeti yok
- Linux sunucu tercih eden müşteriler için
- EF Core ile aynı kod tabanından desteklenir (provider değiştirilir)

### Sürüm gereksinimi
- SQL Server 2019+ (Always Encrypted gerekli)
- PostgreSQL 14+

---

## Dağıtım Modeli (Deployment)

### Standart kurulum mimarisi
```
[Müşteri Domain Controller / AD]
            ↓ SSO
[Müşteri Web Server (IIS/Kestrel)]  ←  [Müşteri SQL Server]
            ↑
   [İK kullanıcılarının tarayıcıları]
```

### Sunucu gereksinimi (yaklaşık)
- **Web sunucu**: Windows Server 2019+, 8 vCPU, 16 GB RAM, 100 GB disk
- **DB sunucu**: SQL Server 2019+, 8 vCPU, 32 GB RAM, 500 GB disk
- **Tek makinede de kurulabilir** (200-500 çalışanlı şirketler için yeterli)

### Kurulum paketi
- **MSI installer** veya **Docker compose** alternatifli
- Tek script ile DB oluşturma, IIS/Kestrel konfigürasyonu, sertifika kurulumu
- Hedef kurulum süresi: 1 günde canlıya
- Tipik proje süresi (kurulum + parametre + ilk veri yükleme): 2-4 hafta

### Güncelleme mekanizması
- Sürüm paketi indir → DB migration otomatik → IIS site yeniden başlat
- Müşteri internet erişimi gerektirmez (offline güncelleme paketi destekli)

---

## Güvenlik

### Kimlik doğrulama
- **Windows Authentication / Active Directory** — birincil yöntem
- **OIDC / SAML** — Azure AD, Okta entegrasyonu
- **Yerel kullanıcı (lokal hesap)** — opsiyonel fallback (AD'siz müşteriler için)

### Yetkilendirme
- **Role-Based Access Control (RBAC)** — bkz. [06-ROLLER-VE-YETKILER.md](06-ROLLER-VE-YETKILER.md)
- **Field-level security** — bordro alanı sadece bordrocular görebilir, müdür sadece kendi ekibini
- **Row-level security** — departman müdürü sadece kendi departmanı çalışanlarını görür

### Veri güvenliği
- **Hassas alanlar şifreli** — TC kimlik no, IBAN, doğum tarihi: SQL Server Always Encrypted
- **Veri tabanı seviyesinde TDE** (Transparent Data Encryption) — opsiyonel, müşteri seçer
- **HTTPS zorunlu** — kurulumda otomatik self-signed sertifika; müşteri Let's Encrypt veya kurum CA ile değiştirir
- **Audit log** — tüm değişiklikler timestamp + kullanıcı + eski değer + yeni değer ile

### KVKK uyumu
- Kişisel veri silme talebi (Right to be Forgotten) — anonim hale getirme akışı
- Veri ihracatı (Data Portability) — kişiye tüm verisini Excel olarak verme
- İşlenme amacı kayıtları
- Audit log 5 yıl saklama

---

## Çoklu Kullanıcı / Eş Zamanlılık

### Eş zamanlı düzenleme stratejisi
- **Optimistic concurrency** — `RowVersion`/`Timestamp` sütunu, çakışma → kullanıcıya sor
- **SignalR notification** — "Ahmet aynı kaydı düzenliyor" uyarısı
- **Read locks YOK** — okuma her zaman serbest

### Performans hedefleri
- 100 eş zamanlı kullanıcı altında <500ms ortalama yanıt süresi
- 5000 çalışanlı şirket için zam senaryosu hesaplama <3 saniye
- Liste sayfası ilk yükleme <1 saniye

### Önbellekleme
- Master data (kademe, departman, pozisyon) — `IMemoryCache`
- Kullanıcı oturumu boyunca lookup verileri
- Hesaplama sonuçları → `Hangfire` ile arka planda + DB'de saklı

---

## Background Jobs (Hangfire)

Aşağıdaki işler arka planda yürütülür:

| İş | Tetikleyici | Süre |
|---|---|---|
| Büyük zam senaryosu hesaplama | Manuel | 10sn-5dk |
| Aylık kıdem tazminatı yükü hesabı | Cron (her ay 1) | ~30sn |
| PDF/Excel rapor üretimi | Manuel | 5sn-2dk |
| E-posta bildirimleri (onay akışı) | Olay | <5sn |
| Audit log arşivleme | Cron (yıllık) | ~10dk |
| Backup (DB) | Cron (gece) | 30sn-30dk |

---

## Loglama ve İzleme

- **Serilog** — structured logging
- **Sink'ler**: Dosya (rolling) + SQL Server tablosu
- **Log seviyesi**: ayardan değiştirilebilir
- **Monitoring**: Müşteri Grafana/PRTG kullanıyorsa endpoint açık

---

## Test Stratejisi

| Test türü | Araç | Kapsam |
|---|---|---|
| Unit test | xUnit | Hesap motoru (Comp.Domain) — %100 coverage hedefi |
| Integration test | xUnit + WebApplicationFactory | API + DB |
| UI test | Playwright | Kritik akışlar (login, employee CRUD, scenario) |
| Performance test | NBomber | 100 eş zamanlı kullanıcı yükü |

**Hesap motoru testi pazarlık konusu değildir.** Her vergi dilimi geçişi, her SGK matrah sınırı, her asgari ücret değişimi için referans testleri olmalı.

---

## Geliştirme Ortamı

- **IDE**: Visual Studio 2022+ veya JetBrains Rider veya VS Code + C# Dev Kit
- **Source control**: Git (GitHub/Azure DevOps/Self-hosted GitLab)
- **CI/CD**: GitHub Actions veya Azure DevOps Pipelines
- **Branch model**: Trunk-based development veya GitFlow (ekip büyüklüğüne göre)
- **Code review**: PR zorunlu, en az 1 onay
- **Build artifact**: Tek bir MSI + Docker image

---

## Lisanslama (Yazılım İçi)

Mevcut Python programındaki `licensing.py` mantığı yeni ürüne taşınacak:

- **Lisans dosyası**: müşteri makinesine bağlı (machine fingerprint)
- **Çalışan sayısı limiti**: lisansa gömülü, kontrol veritabanı seviyesinde
- **Süresi dolan lisans**: read-only moda düşer (veri kaybı yok)
- **Online aktivasyon değil**: müşteri sunucusu internet görmez, lisans dosyası elden iletilir

---

## Versiyonlama

- **Semantic versioning**: MAJOR.MINOR.PATCH
- **Major**: kırıcı API/UI değişiklikleri (yıllık)
- **Minor**: yeni özellik (3 aylık)
- **Patch**: bug fix (gerektiğinde)
- **DB migration politikası**: geriye uyumlu en az 2 minor sürüm boyunca

---

## Kararlar Açık Listesi

İleride değişebilir, tartışılması gereken:

- [ ] Component kütüphanesi: Syncfusion mu DevExpress mi? (lisans maliyeti vs. özellik karşılaştırması)
- [ ] Self-hosted GitLab vs. GitHub Enterprise vs. Azure DevOps
- [ ] Reporting: QuestPDF (kod ile PDF) mu Telerik Reporting mi?
- [ ] Charts: ApexCharts (free) mu Syncfusion Charts (paid) mi?
- [ ] DB: SQL Server tek seçenek mi, PostgreSQL ilk versiyonda destekleyelim mi?
- [ ] Mobile: future scope; PWA mı native mi?
