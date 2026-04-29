# 03 — Modüller ve Ekranlar

Programın tüm modüllerinin/sayfalarının detaylı spesifikasyonu. Her modül için: amaç, kullanıcı, veri girişi, işlevsellik, UI elemanları, çıktılar.

---

## Üst Düzey Sayfa Haritası

```
┌─ Anasayfa (Dashboard)
│
├─ Çalışan Yönetimi
│  ├─ Çalışan Listesi
│  ├─ Çalışan Detay
│  └─ Toplu İçeri Aktarma (Excel/CSV)
│
├─ Organizasyon Yapısı
│  ├─ Departmanlar
│  ├─ Pozisyonlar
│  └─ Lokasyonlar
│
├─ Ücret Yapısı (Comp Architecture)
│  ├─ Kademe Sistemi (Job Grades)
│  ├─ Maaş Bantları (Salary Bands)
│  └─ İş Aileleri (Job Families)
│
├─ Piyasa Karşılaştırma (Market Benchmark)
│  ├─ Benchmark Verisi Girişi/Import
│  └─ Market Index Analizi
│
├─ Zam Yönetimi (Merit Cycle)
│  ├─ Genel Zam
│  ├─ Departman Bazlı Zam
│  ├─ Pozisyon Bazlı Zam
│  ├─ Hedefli Zam (Toplam Bütçe Hedefli)
│  ├─ Hedefli Zam — Departman Bazlı
│  └─ Merit Matrix (Performans × Pozisyon)
│
├─ Senaryo ve Simülasyon
│  ├─ Senaryo Oluşturma
│  ├─ Senaryo Karşılaştırma
│  ├─ İşe Alım/Çıkış Simülasyonu
│  └─ Çok Yıllı Planlama
│
├─ Pay Equity & Analiz
│  ├─ Cinsiyet Ücret Farkı (Gender Pay Gap)
│  ├─ İç Adalet (Compression)
│  ├─ Compa-Ratio Dağılımı
│  └─ Range Penetration
│
├─ Onay Akışı (Workflow)
│  ├─ Onay Bekleyen
│  ├─ Tamamlanan
│  └─ Onay Şablonları
│
├─ Raporlama
│  ├─ Standart Raporlar
│  ├─ Ücret Bildirim Mektubu (Comp Letter)
│  ├─ Toplam Ödül Bildirimi (Total Rewards)
│  └─ Özel Rapor Tasarımcısı
│
├─ Yönetim
│  ├─ Kullanıcı Yönetimi
│  ├─ Rol ve Yetki
│  ├─ Audit Log
│  ├─ Vergi/SGK Parametreleri
│  └─ Sistem Ayarları
│
└─ Yardım / Hakkında
```

---

## 1. Anasayfa (Dashboard)

### Amaç
İK direktörü/müdürü programa giriş yaptığında şirketin ücret durumunu **bir bakışta** görsün.

### Kullanıcı
İK Direktörü, İK Müdürü, CFO (read-only)

### KPI Kartları (üst şerit)
- **Toplam Çalışan**: 1.247
- **Toplam Aylık Ücret Maliyeti** (brüt + işveren payı): 87.5M TL
- **Ortalama Compa-Ratio**: 0.94 (piyasa medyanının %6 altında)
- **Pay Gap (Cinsiyet)**: %4.2 (kadın < erkek)
- **Açık Onay Bekleyen**: 12 kayıt
- **Bant Dışı Çalışan Sayısı**: 23 (range out)

### Grafikler
1. **Compa-Ratio dağılımı** — histogram
2. **Maaş bantlarına göre çalışan dağılımı** — kademe × Q1/Q2/Q3/Q4
3. **Departman bazlı ortalama maaş** — bar chart
4. **Yıllık ücret artışı trendi** — line chart (son 3-5 yıl)
5. **Market Index by Department** — heat map (yeşil/sarı/kırmızı)

### Filtreler
- Şirket (holding modunda)
- Departman
- Pozisyon
- Kademe
- Tarih aralığı

---

## 2. Çalışan Yönetimi

### 2.1 Çalışan Listesi

#### Sütunlar
- Sicil No
- Ad Soyad
- Departman
- Pozisyon
- Kademe (Grade)
- İşe Giriş Tarihi
- Brüt Maaş
- Compa-Ratio
- Range Penetration
- Performans Skoru (varsa)
- Son Zam Tarihi
- Durum (Aktif/Pasif/İzinli)

#### Özellikler
- Sayfalama (varsayılan 50/sayfa)
- Sütun bazlı arama, sıralama
- Çoklu seçim → toplu işlem (zam, departman değişikliği vb.)
- **Sütun göster/gizle** — kullanıcı tercihine göre kalıcı
- **Excel/CSV export** — filtrelenmiş veri
- **Renk kodlaması**: Compa-ratio < 0.80 kırmızı, > 1.20 sarı (uyarı)

#### Yetki
- İK uzmanı: tümünü görür ve düzenler
- Departman müdürü: sadece kendi departmanını
- Çalışan kendi: sadece kendi kaydını

### 2.2 Çalışan Detay

#### Sekmeler
1. **Kişisel Bilgiler** — TC, doğum, iletişim, adres (KVKK koruması)
2. **İş Bilgileri** — pozisyon, kademe, departman, lokasyon, müdür, işe giriş
3. **Ücret Bilgileri** — brüt, net, prim, yan haklar, toplam paket (Total Rewards)
4. **Geçmiş** — maaş değişiklikleri timeline (audit log filtresi)
5. **Performans** — performans skorları, kalibre edilmiş yıllık değerlendirme
6. **Belgeler** — sözleşme, ek protokoller, ücret bildirim mektupları

### 2.3 Toplu İçeri Aktarma

#### Format
- **Excel şablonu indir** → boş şablon
- Mevcut çalışanları **Excel olarak güncel haliyle export** edip → değişiklikleri yapıp → tekrar import
- Kolonlar: zorunlu/opsiyonel ayrımı, otomatik veri tipi tespiti

#### Doğrulama
- Boş zorunlu alan
- Geçersiz TC kimlik
- Olmayan departman/pozisyon → kullanıcıya "yeni eklensin mi?" sorusu
- Kademe-pozisyon eşleşmemesi
- Maaş aşırı sapma uyarısı (mevcuttan %50 farklı)

#### Çakışma Çözümü
- Sicil no ile eşleştir → mevcudu güncelle vs. yeni ekle
- Önizleme ekranı: ne değişecek, kaç satır eklenecek/güncellenecek
- Onay → işlem → audit log'a düşer

---

## 3. Organizasyon Yapısı

### 3.1 Departmanlar
- Hiyerarşik (parent-child) ağaç görünümü
- Departman müdürü ataması (yetki için kritik)
- Maliyet merkezi kodu (ERP entegrasyonu için)
- Aktif/pasif durumu

### 3.2 Pozisyonlar
- Pozisyon kataloğu — tek tek tanımlı (örn. "Senior Yazılım Geliştirme Mühendisi")
- Her pozisyon **bir** kademeye atanır
- **Job family** atanır (Mühendislik, Satış, İK, Mali, Operasyon, vb.)
- **Benchmark eşleştirme adı** — Mercer/WTW raporlarındaki standart pozisyon adı (örn. "Software Engineer III")
- Açıklama, sorumluluklar

### 3.3 Lokasyonlar
- Şehir, ofis, fabrika
- Bölgesel ücret farkı katsayısı (opsiyonel — örn. İstanbul %100, Ankara %95)

---

## 4. Ücret Yapısı (Compensation Architecture)

### 4.1 Kademe Sistemi (Job Grades)

#### Amaç
Şirketin tüm pozisyonlarını yapılandırılmış bir hiyerarşiye yerleştirmek.

#### Tanımlar
- **Kademe** (Grade): 1-15 arası numara veya isimlendirme (örn. "P1, P2, M1, M2, D1...")
- **Career band**: Entry / Professional / Manager / Director / Executive
- Her kademenin **iş değerleme skoru** (Hay/Mercer IPE puanı — opsiyonel)

#### Ekran
- Tablo görünüm: Kademe | İsim | Band | Min Skor | Max Skor | Pozisyon Sayısı
- Drag-drop ile yeniden sıralama
- Kademe düzenleme → tüm bağlı pozisyon ve maaş bandları etkilenir, uyarı verilir

### 4.2 Maaş Bantları (Salary Bands)

#### Amaç
Her kademe için min-mid-max ücret aralığı.

#### Veri Modeli
| Kademe | Min | Mid | Max | Range Spread % | Lokasyon |
|---|---|---|---|---|---|
| P1 | 35.000 | 45.000 | 55.000 | %57 | İstanbul |
| P2 | 50.000 | 65.000 | 80.000 | %60 | İstanbul |
| M1 | 80.000 | 105.000 | 130.000 | %63 | İstanbul |

- **Range spread** = (Max - Min) / Min × 100 — otomatik hesap
- **Midpoint progression** = bu kademe Mid / önceki kademe Mid → otomatik gösterim
- **Lokasyon farkı** — aynı kademenin İstanbul/Ankara/İzmir için farklı bandı olabilir

#### Bant oluşturma asistanı
- "İlk bantı gir, range spread ve midpoint progression yüzdelerini ver, kalan bantları otomatik üret"
- Manuel düzenleme her zaman mümkün

### 4.3 İş Aileleri (Job Families)
- Pozisyonların kategorize edilmesi
- Aynı job family içinde farklı kademe yapısı uygulanabilir (örn. Satış için ayrı bant)

---

## 5. Piyasa Karşılaştırma (Market Benchmark)

### 5.1 Benchmark Verisi Girişi

#### İki yöntem

**A) Manuel giriş (küçük müşteriler için)**
- Pozisyon adı seç → P25, P50, P75 değerlerini gir
- Kaynak (örn. "Mercer 2026 Q1 — Üretim — İstanbul")
- Tarih (verinin geçerli olduğu ay)
- Para birimi (TL)

**B) Excel import (Mercer/WTW/KPMG raporları)**
- Excel sürükle-bırak
- Kolon eşleştirme ekranı:
  - "A sütunu = Pozisyon adı"
  - "B sütunu = P25"
  - "C sütunu = P50"
  - "D sütunu = P75"
- Pozisyon adı eşleştirme: programdaki **benchmark eşleştirme adı** ile dış raporun adı eşleşir
- Önizleme → onay → kaydet

### 5.2 Market Index Analizi

#### Hesaplamalar
- **Market Index** = Şirket Medyanı / Piyasa Medyanı
  - <0.90 → "piyasanın çok altında, retention riski"
  - 0.90-1.10 → "piyasaya uyumlu"
  - >1.10 → "piyasanın üstünde, fazla ödeme olabilir"
- **Pozisyon bazlı index** — her pozisyon için ayrı
- **Kademe bazlı index**
- **Departman bazlı index**

#### Ekran
- Heat map: pozisyon × index değeri
- Drill-down: Index 0.85 olan bir pozisyona tıkla → ilgili çalışanların listesi → "kim ne kadar geride"

#### Aging
- Benchmark verisi eskiyince (örn. 6+ ay) **TÜFE/aylık enflasyon ile yaşlandırma**
- Kullanıcı yıllık enflasyon oranı girer → veri otomatik güncellenir

---

## 6. Zam Yönetimi (Merit Cycle)

> Mevcut programdaki zam sayfaları bu modülde toplanır ve genişletilir.

### 6.1 Genel Zam
- Tek oran tüm kilidi açık çalışanlara
- Mevcut programdan birebir taşınacak
- **Yenilik**: Zam sonrası bant dışına çıkanlar → uyarı

### 6.2 Departman Bazlı Zam
- Departman × Oran tablosu
- Kilidi açık çalışanlar zam alır

### 6.3 Pozisyon Bazlı Zam
- Pozisyon × Oran

### 6.4 Hedefli Zam (Toplam Bütçe)

#### İki mod (mevcut programdaki gibi)
- **Mod A — Standart**: tüm çalışanlar oransal zam alır, hedefe ulaşılır
- **Mod B — Minimum garantili**: herkes en az %X alır, kalan bütçe kalanlara dağıtılır

#### Yenilik
- Hedef olarak: **mutlak tutar** veya **mevcut maaşa göre %**
- Bant dışı çalışanlar (max üstü) zam almasın seçeneği

### 6.5 Hedefli Zam — Departman Bazlı
- Her departman için ayrı bütçe hedefi
- Yine A/B modu

### 6.6 Merit Matrix (Performans × Pozisyon)

#### Yapı
9 kutucuklu matris (3 performans × 3 pozisyon — esnek):

| | Bandın altı (Q1) | Bandın ortası (Q2-Q3) | Bandın üstü (Q4) |
|---|---|---|---|
| Yüksek performans (4-5) | %15 | %12 | %8 |
| Orta performans (3) | %10 | %8 | %5 |
| Düşük performans (1-2) | %3 | %2 | %0 |

- Hücreler kullanıcı tarafından düzenlenir
- Bütçe simülasyonu canlı: "Bu matrix toplam %X bütçe demek"
- Çalışan listesi otomatik dağıtılır → manuel düzeltme mümkün

#### Yetki
- Departman müdürü kendi ekibine matristen önerilen zamı düzenleyebilir
- İK kalibrasyon yapar
- Üst yönetim onaylar

---

## 7. Senaryo ve Simülasyon

### 7.1 Senaryo Oluşturma
- Mevcut çalışan veri snapshot'ı + zam senaryosu = **Scenario** kaydı
- İsim, açıklama, oluşturan, oluşturma tarihi
- Birden fazla senaryo kaydedilebilir

### 7.2 Senaryo Karşılaştırma
- Yan yana 2-4 senaryo
- Karşılaştırma metrikleri:
  - Toplam yıllık maliyet
  - Ortalama zam %
  - Bant dışı kalanlar
  - Departman dağılımı
  - Kıdem tazminatı yükümlülük artışı
  - Compa-ratio değişimi

### 7.3 İşe Alım/Çıkış Simülasyonu
- "X kişi alalım, Y pozisyondan, Z maaşla" → maliyet etkisi
- "X çalışanı çıkaralım" → tasarruf + tazminat
- Mevcut programda var, taşınacak

### 7.4 Çok Yıllı Planlama
- 3-5 yıllık ücret stratejisi
- Yıllık enflasyon, asgari ücret artışı varsayımı
- Kademe bazlı yıllık zam stratejisi
- Toplam bütçe trajectory'si

---

## 8. Pay Equity & Analiz

### 8.1 Cinsiyet Ücret Farkı (Gender Pay Gap)
- Pozisyon bazlı: aynı pozisyonda kadın/erkek ortalama maaş farkı %
- Genel: tüm çalışanlar bazında
- **Adjusted gap**: kademe, pozisyon, kıdem normalizasyonu sonrası
- AB Pay Transparency Directive uyumlu rapor formatı

### 8.2 İç Adalet — Compression
- Yeni işe alınanın eski çalışandan fazla kazanması
- Tablo: çalışan | kıdem | maaş | aynı pozisyon ortalama | fark
- Threshold ayarlı uyarı

### 8.3 Compa-Ratio Dağılımı
- Histogram: kaç çalışan hangi compa-ratio aralığında
- Departman/kademe filtreli

### 8.4 Range Penetration
- Çalışanların bant içinde nerede olduğu (Q1, Q2, Q3, Q4)
- Sağlıklı dağılım: %25-25-25-25 hedef
- Sapma analizi

---

## 9. Onay Akışı (Workflow)

### 9.1 Şablon
Yapılandırılabilir onay zincirleri:
- Tek seviye: Departman Müdürü → İK Müdürü
- Çift seviye: + CFO
- Üç seviye: + CEO (örn. yüksek tutarlı zamlar için)

### 9.2 Tetikleyici
- Bireysel zam önerisi
- Toplu zam dönemi (merit cycle)
- Yeni işe alım maaş onayı
- Off-cycle ayarlama (retention bonus, market adjustment)
- Threshold üstü onay (örn. %20 üzeri zam)

### 9.3 Akış Ekranı
- Onay bekleyen kart listesi
- Detay: kim, ne, ne kadar, gerekçe
- Onayla / Reddet / Geri gönder seçenekleri
- Yorum ekleme
- E-posta bildirimi
- Mobil-uyumlu görünüm (yöneticiler dışarıdayken onaylasın)

---

## 10. Raporlama

### 10.1 Standart Raporlar
- Aylık ücret raporu
- Yıllık zam dağılımı
- Departman maliyet raporu
- Compa-ratio raporu
- Pay equity raporu
- Onay süreci özet
- Audit log özet

### 10.2 Ücret Bildirim Mektubu (Compensation Letter)
- Çalışana özel PDF
- Yeni maaş, zam tutarı, zam oranı, geçerlilik tarihi
- Yan haklar, prim potansiyeli, total rewards
- Yönetici imza alanı
- **Toplu üretim** — zam sonrası tüm çalışanlara tek tıkla
- E-posta entegrasyonu

### 10.3 Toplam Ödül Bildirimi (Total Rewards Statement)
- Yıllık özet — çalışana yıl sonu verilir
- Sabit + değişken + yan haklar parasal değer
- "Sana yıllık X TL değerinde ödül paketi verdik" mesajı

### 10.4 Özel Rapor Tasarımcısı
- Sürükle-bırak rapor builder (sonraki faz)
- Excel template uploading

---

## 11. Yönetim

### 11.1 Kullanıcı Yönetimi
- Kullanıcı ekle/düzenle/devre dışı
- AD eşleştirme
- Rol ataması
- Departman/lokasyon kapsamı
- Son giriş zamanı, başarısız giriş denemeleri

### 11.2 Rol ve Yetki
- Önceden tanımlı roller (bkz. [06-ROLLER-VE-YETKILER.md](06-ROLLER-VE-YETKILER.md))
- Custom rol oluşturma
- Modül × CRUD matrisi

### 11.3 Audit Log
- Filtrelenebilir tablo: kullanıcı, tarih, modül, eylem, eski değer, yeni değer
- Export
- Saklama süresi (5 yıl varsayılan)

### 11.4 Vergi/SGK Parametreleri
**KRİTİK**: bkz. [05-HESAPLAMA-MOTORU.md](05-HESAPLAMA-MOTORU.md). Tüm sayısal sabitler bu ekrandan düzenlenir, koda gömülmez.

- Yıl seçimi (mevcut yıl + gelecek yıl)
- SGK işçi/işveren oranları
- İşsizlik sigortası oranları
- Damga vergisi oranı
- Gelir vergisi dilimleri (yıllık tarife)
- Asgari ücret (brüt/net)
- SGK matrah min/max
- Asgari ücret istisnası tutarı
- Aylık dönem geçişleri (yıl içinde değişen değerler için)

### 11.5 Sistem Ayarları
- Şirket bilgileri (logo, isim, vergi no)
- Para birimi (TL varsayılan, USD/EUR opsiyonel)
- Mali yıl başı (genelde Ocak)
- Yıllık merit cycle başlangıç ayı
- E-posta SMTP ayarları
- AD/LDAP konfigürasyonu
- Yedekleme zamanlaması
- Lisans bilgisi

---

## 12. Yardım

- Kullanım kılavuzu (mevcut programdaki gibi rich text)
- Klavye kısayolları
- Sıkça sorulan sorular
- İletişim/destek talebi açma
- Hakkında (sürüm, lisans bilgisi)
