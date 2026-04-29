# 08 — Yapılacaklar (TODO Roadmap)

Fazlara bölünmüş geliştirme planı. Her faz sonu **yayınlanabilir bir sürüm** üretmeli.

> **Genel ilke**: Her özellik bir sonraki ile zincirlenmiş değil. Faz sonu mutlak değil — öncelikler değişebilir. Bu liste yaşayan bir belgedir.

---

## Faz 0 — Hazırlık (1-2 hafta)

Kod yazımı başlamadan kararların netleştirilmesi.

### Karar Verilecekler
- [ ] Bileşen kütüphanesi seçimi: **Syncfusion Blazor mu DevExpress Blazor mu**? Lisans maliyeti, grid özellikleri, mobil destek karşılaştırması
- [ ] Reporting motoru: **QuestPDF (kod ile) mi Telerik Reporting mi**?
- [ ] Source control: **GitHub Enterprise / Azure DevOps / Self-hosted GitLab** karşılaştırması
- [ ] Logging stack: **Serilog + ELK / Seq / Application Insights**
- [ ] DB sürümü: **SQL Server 2022** standart mı, **PostgreSQL** ilk versiyondan mı?
- [ ] Hangi sürüm .NET kullanılacak (LTS olan .NET 9 önerilir)

### Hazırlık İşleri
- [ ] Geliştirme ortamı kurulumu (lisans, IDE, repo, CI)
- [ ] Coding standards dokümantasyonu
- [ ] Architecture Decision Records (ADR) yapısı
- [ ] PR/code review akışı
- [ ] İlk müşteri (pilot) ile pre-sales görüşmeleri
- [ ] 2026 vergi parametrelerinin **resmi olarak teyidi** (kullanıcıdan alındı: 2026 GV tarifesi ve asgari ücret)

---

## Faz 1 — MVP (3-4 ay) — "İçe Doğru Çekirdek"

Hedef: kullanılabilir, satılabilir minimum üründe ne olmalı.

### 1.1 Çekirdek Altyapı
- [ ] Solution yapısı: `Comp.Web` (Blazor), `Comp.Application`, `Comp.Domain`, `Comp.Infrastructure`, `Comp.Tests`
- [ ] EF Core ilk migration: temel tablolar
- [ ] Authentication: ASP.NET Identity + Windows Auth opsiyonu
- [ ] RBAC framework (rol, izin, scope)
- [ ] Audit log interceptor
- [ ] Layout: NavMenu, Header, Footer, Theme
- [ ] Localization: TR varsayılan, EN hazırlığı
- [ ] Hata yönetimi, global exception handler
- [ ] Loglama (Serilog → dosya + DB)
- [ ] Health check endpoint
- [ ] Background job altyapısı (Hangfire)

### 1.2 Hesap Motoru (Comp.Domain)
- [ ] `IncomeTaxCalculator` (Python'dan port)
- [ ] `SalaryCalculator` (brüt → net + işveren maliyeti)
- [ ] Vergi parametre repository (DB'den oku, cache)
- [ ] 2026 değerleri seed data
- [ ] **Birim test seti** — Python sonuçlarıyla kıyaslama (regression)
- [ ] Aylık dönem geçişi (1-12 ay parametre değişimi)
- [ ] SGK indirim teşvikleri toggle
- [ ] Asgari ücret istisnası dinamik

### 1.3 Çalışan Yönetimi
- [ ] Çalışan CRUD (UI + API)
- [ ] Çalışan listesi (sayfalama, sıralama, arama, filtre)
- [ ] Çalışan detay (sekmeli görünüm)
- [ ] Excel import (toplu yükleme + doğrulama)
- [ ] Excel export
- [ ] Hassas alan maskeleme (TC, IBAN)
- [ ] KVKK anonimleştirme akışı

### 1.4 Organizasyon Yapısı
- [ ] Departman ağacı (hiyerarşik)
- [ ] Pozisyon kataloğu
- [ ] Lokasyon yönetimi
- [ ] Kademe sistemi
- [ ] İş ailesi

### 1.5 Maaş Bantları
- [ ] Bant CRUD
- [ ] Bant geçmişi (history)
- [ ] Bant asistanı (toplu üretim — range spread + midpoint progression ile)
- [ ] Compa-ratio canlı hesap
- [ ] Range penetration

### 1.6 Zam Yönetimi (Mevcut Programdan Taşınan)
- [ ] Genel zam
- [ ] Departman bazlı zam
- [ ] Pozisyon bazlı zam
- [ ] Hedefli zam (toplam — A modu)
- [ ] Hedefli zam (toplam — B modu, min garantili)
- [ ] Hedefli zam (departman bazlı)

### 1.7 Senaryo
- [ ] Senaryo CRUD
- [ ] Senaryo hesaplama (arka plan)
- [ ] Senaryo karşılaştırma (yan yana)
- [ ] Senaryo uygulama (gerçek maaşlara işle, audit log)

### 1.8 Dashboard
- [ ] KPI kartları (5-6 metric)
- [ ] Compa-ratio histogram
- [ ] Departman bazlı ortalama maaş
- [ ] Bant dağılımı görünümü

### 1.9 Raporlama
- [ ] Standart Excel rapor (çalışan listesi)
- [ ] Standart PDF rapor (özet)
- [ ] Export izleme (audit)

### 1.10 Yönetim
- [ ] Kullanıcı yönetimi UI
- [ ] Rol atama UI
- [ ] Vergi parametreleri yıllık ekran
- [ ] Audit log viewer
- [ ] Sistem ayarları

### 1.11 Kurulum
- [ ] MSI installer (Windows Server)
- [ ] DB initial migration script
- [ ] Lisans dosyası mekanizması
- [ ] İlk müşteri kurulum kılavuzu

**Faz 1 çıktısı**: Pilot müşteriye kurulabilir, temel comp policy işlevleri çalışan ürün.

---

## Faz 2 — Kurumsallaşma (3-4 ay) — "Onay ve Analiz"

Hedef: Büyük şirketin gerçekten kullanabileceği seviyeye çıkar.

### 2.1 Onay Akışı (Workflow)
- [ ] Onay şablonu yapılandırma
- [ ] Threshold kuralları
- [ ] Onay bekleyen ekranı
- [ ] Onay geçmişi
- [ ] E-posta bildirimi
- [ ] Mobil uyumlu onay görünümü

### 2.2 Pay Equity
- [ ] Cinsiyet pay gap (ham + düzeltilmiş)
- [ ] Compression analizi
- [ ] Compa-ratio dağılım analizi
- [ ] Range penetration dağılımı
- [ ] AB Pay Transparency uyumlu rapor formatı

### 2.3 Merit Matrix
- [ ] 9-kutucuk matrix UI (esnek satır/sütun)
- [ ] Performans skoru girişi
- [ ] Otomatik dağıtım + manuel override
- [ ] Bütçe canlı simülasyon
- [ ] Departman bazlı kalibrasyon

### 2.4 Benchmark / Market Index
- [ ] Benchmark provider tanımı
- [ ] Manuel benchmark girişi
- [ ] Excel import (Mercer/WTW formatı)
- [ ] Pozisyon eşleştirme UI
- [ ] Market index hesaplama
- [ ] Market index dashboard (heat map)
- [ ] Benchmark aging (TÜFE/enflasyon ile yaşlandırma)

### 2.5 Total Rewards
- [ ] Yan haklar tanımı (catalog)
- [ ] Çalışan başına yan hak ataması
- [ ] Total rewards hesaplama
- [ ] Çalışana özel total rewards bildirim PDF'i

### 2.6 Ücret Bildirim Mektubu (Comp Letter)
- [ ] PDF şablonu (özelleştirilebilir)
- [ ] Toplu üretim
- [ ] E-posta gönderimi
- [ ] Çalışan portalı (basit) — kendi mektubunu görür

### 2.7 İşe Alım/Çıkış Simülasyonu
- [ ] Yeni işe alım — bant kontrolü
- [ ] İşe alım onay akışı entegrasyonu
- [ ] Çıkış simülasyonu — kıdem tazminatı yükümlülüğü
- [ ] Bordro tasarrufu hesabı

### 2.8 Çok Yıllı Planlama
- [ ] 3-5 yıllık ücret planı UI
- [ ] Enflasyon, asgari ücret artışı varsayımları
- [ ] Trajectory görselleştirme

### 2.9 İleri Dashboard
- [ ] Market index by department heat map
- [ ] Yıllık zam trend grafiği
- [ ] Pay gap özet
- [ ] Bant dışı çalışan uyarıları

### 2.10 İleri Yetkilendirme
- [ ] Field-level security (hassas alanlar)
- [ ] Dynamic scope (manager hierarchy)
- [ ] Break-glass acil yetki

**Faz 2 çıktısı**: 200+ çalışanlı şirkette tam fonksiyonel çalışan, kurumsal satışa uygun ürün.

---

## Faz 3 — Pazara Hazırlık (2-3 ay) — "Cila ve Ölçek"

Hedef: Çok müşterili pazara çıkış.

### 3.1 Çoklu Şirket / Holding
- [ ] Multi-company entity yapısı
- [ ] Şirket seçici UI
- [ ] Konsolide raporlama

### 3.2 Lokal Bant
- [ ] Lokasyon bazlı bant farkları
- [ ] Bölgesel index (İstanbul %100, vs.)

### 3.3 Yedekleme ve Kurtarma
- [ ] Otomatik DB yedek
- [ ] Yedek restore UI
- [ ] Veri ihracatı (full export)

### 3.4 Performans Optimizasyonu
- [ ] Yük testi (100+ eş zamanlı kullanıcı)
- [ ] DB index optimizasyonu
- [ ] Cache stratejisi
- [ ] Lazy loading liste sayfaları

### 3.5 Mobile Responsive (PWA)
- [ ] Onay sayfaları mobil
- [ ] Dashboard mobil okunur
- [ ] PWA install support

### 3.6 ERP Entegrasyonları
- [ ] Logo Tiger HR connector (read employee data)
- [ ] Mikro Bordro connector
- [ ] Generic REST API
- [ ] Excel sync

### 3.7 Özel Rapor Builder
- [ ] Sürükle-bırak rapor tasarımcısı
- [ ] Özel SQL view tanımlama
- [ ] Rapor şablon kaydetme/paylaşma

### 3.8 Bildirim Sistemi
- [ ] In-app bildirim merkezi
- [ ] E-posta tercih ayarları
- [ ] SMS opsiyonu (Türk operatörler)

### 3.9 Yardım ve Eğitim
- [ ] In-app yardım metinleri
- [ ] Video tutorial linkleri
- [ ] PDF kullanım kılavuzu (otomatik üretilen)

### 3.10 Yerelleştirme
- [ ] EN tam destek (yabancı yöneticisi olan şirketler)
- [ ] Tarih/sayı format Türkçe locale doğrulaması

**Faz 3 çıktısı**: 5+ müşteriye yayılabilir, satış ekibinin demoyu rahat verdiği ürün.

---

## Faz 4 — Diferansiyasyon (esnek)

Pazar geri bildirimine göre öncelik sıralanır.

### 4.1 AI Önerileri
- [ ] Çalışan retention skoru tahmini
- [ ] Önerilen zam (LLM-destekli açıklamalı)
- [ ] Anomali tespiti (compression uyarısı)

### 4.2 Bonus / STI Yönetimi
- [ ] STI plan tanımı (hedef yüzdesi by grade)
- [ ] Performans metric × hedef × dağıtım
- [ ] Sales commission yapıları

### 4.3 LTI / Hisse Opsiyonu
- [ ] Vesting schedule
- [ ] Hisse değerleme
- [ ] Cliff/grant senaryoları

### 4.4 Promotion Planner
- [ ] Terfi adayı listesi
- [ ] Terfi etkisi simülasyonu
- [ ] Career path görselleştirme

### 4.5 Çalışan Self-Service
- [ ] Çalışan portali (kendi profili, total rewards)
- [ ] Maaş simülasyonu (kendisine)
- [ ] Yan hak tercihi (flexible benefits)

### 4.6 Ek ERP Bağlayıcıları
- [ ] SAP HCM
- [ ] Oracle HCM
- [ ] Workday import (geçiş yapan müşteriler için)

### 4.7 Mobil Native
- [ ] iOS (.NET MAUI)
- [ ] Android (.NET MAUI)
- [ ] Sadece onay + dashboard, tam özellik değil

---

## Bilinen Riskler

| Risk | Etki | Önlem |
|---|---|---|
| Vergi mevzuatı yıllık değişir | Yüksek | Tüm sabitler ayardan, yıllık güncellemeyi otomatize et |
| Bileşen kütüphanesi lisans maliyeti | Orta | Faz 0'da net karar, alternatif olarak free yedek (MudBlazor) |
| İlk müşteri uzun karar süreci (6-9 ay) | Yüksek | Faz 1 sonunda demoya hazır olmak şart |
| Workday/SAP Türkiye'de güçlenir | Orta | Yerel mevzuat ve fiyat avantajını koru |
| Geliştirici bulamama (C#/.NET ekosistemi Türkiye'de) | Orta | Senior C# geliştirici Türkiye'de bol; junior eğitilebilir |
| KVKK uyum eksikliği | Yüksek | Faz 1'den itibaren audit log + RBAC + encryption şart |
| Performans hedefleri tutmaz | Orta | Faz 1 sonunda yük testi; mimari erken doğrula |

---

## Bitirme Kriteri (Definition of Done)

Her özellik için:
- [ ] Kod yazıldı, code review geçti
- [ ] Unit test yazıldı (kritik mantık için)
- [ ] Integration test geçti (API + DB)
- [ ] UI manuel test edildi (golden path + 1-2 edge case)
- [ ] Audit log kayıtları doğru atılıyor
- [ ] Yetki kontrolü doğru (yetkisiz kullanıcı 403)
- [ ] Localization (TR) tam
- [ ] Performans hedefi tutuyor
- [ ] Dokümantasyon güncellendi (bu klasördeki MD dosyaları)

---

## Şu An Yapılacak Bir Sonraki Şey

1. **Faz 0 kararlarını netleştir**: bileşen kütüphanesi, repo platformu, .NET sürümü
2. **Pilot müşteri profili belirle**: hangi şirket, hangi sektör, hangi kişiyle konuşulacak
3. **Hesap motoru için "regression test fixture"**: mevcut Python programıyla 50-100 farklı brüt değer için net hesap çıkar, JSON olarak kaydet — yeni C# kodun bu fixture'a karşı test edilecek
4. **2026 SGK matrah max ve kıdem tazminatı tavanı**: resmi rakamları teyit et
5. **Solution iskeleti**: ilk commit'i yap, "Hello World Blazor + EF + first migration"
