# 09 — Referanslar

Ürün geliştirme sürecinde başvurulacak dış kaynaklar, metodoloji referansları, yasal kaynaklar ve rakip analizi.

---

## Türkiye — Yasal ve Resmi Kaynaklar

### Vergi ve Mevzuat
- **Gelir İdaresi Başkanlığı (GİB)**: https://www.gib.gov.tr — yıllık gelir vergisi tarifesi, tebliğler
- **Resmî Gazete**: https://www.resmigazete.gov.tr — kanun, KHK, tebliğ yayınlanması
- **Sosyal Güvenlik Kurumu (SGK)**: https://www.sgk.gov.tr — SGK matrah, oranlar, prim teşvikleri
- **Çalışma ve Sosyal Güvenlik Bakanlığı**: https://www.csgb.gov.tr — asgari ücret, kıdem tazminatı tavanı
- **TÜİK** (Türkiye İstatistik Kurumu): https://www.tuik.gov.tr — TÜFE, ortalama ücret istatistikleri

### Önemli Kanunlar (Referans)
- **193 sayılı Gelir Vergisi Kanunu** — Madde 103: gelir vergisi tarifesi
- **5510 sayılı Sosyal Sigortalar ve Genel Sağlık Sigortası Kanunu** — SGK matrah, primler
- **4857 sayılı İş Kanunu** — kıdem tazminatı
- **6698 sayılı Kişisel Verilerin Korunması Kanunu (KVKK)** — veri güvenliği, anonimleştirme

### Vergi Yardımcı Kaynakları (Üçüncü Taraf)
- **Verginet**: https://www.verginet.net — pratik vergi tarifeleri (kullanıcı tarafından kaynak olarak verildi)
- **Vergi Konseyi**: https://vergikonseyi.gov.tr
- **Mali Hesap Uzmanları Derneği** (MHUD)
- **TÜRMOB**: https://www.turmob.org.tr

### 2026 Yılı Doğrulanması Gereken Resmi Veriler
- [ ] Gelir vergisi tarifesi (kullanıcıdan alındı, GİB ile karşılaştırılacak)
- [ ] SGK matrah tabanı ve tavanı (33.030 ve 247.725 — teyit)
- [ ] Asgari ücret (28.075,50 net / 33.030,00 brüt — teyit)
- [ ] Damga vergisi oranı (binde 7,59)
- [ ] Kıdem tazminatı tavanı (Maliye 6 ayda bir günceller)
- [ ] SGK işveren teşvik oranları (5510 5/I-i %5 indirimi)
- [ ] BES (Bireysel Emeklilik) işveren katkı limitleri

---

## Compensation Methodology — Akademik / Endüstri Standardı

### Job Evaluation Sistemleri
- **Hay Job Evaluation** — bilgi, problem çözme, sorumluluk üç boyutu
- **Mercer International Position Evaluation (IPE)** — 11 faktör, en yaygın kurumsal kullanımda
- **Korn Ferry Job Evaluation (Hay Group)**
- **Towers Watson Global Grading System (GGS)**

### Salary Structure Tasarım
- **Range spread**: tipik %40-60
- **Midpoint progression**: tipik %10-15
- Referans okuma:
  - WorldatWork — *Salary Structure Design* (rehber)
  - Milkovich, Newman, Gerhart — *Compensation* (Compensation alanında klasik akademik kitap)

### Pay Equity Metodolojisi
- **AB Pay Transparency Directive (2023/970)** — 2026'dan itibaren AB'de zorunlu, Türkiye'ye etkisi
- **OFCCP Compensation Compliance** (ABD) — pay equity analiz metodolojileri
- **Multivariate regression** — düzeltilmiş cinsiyet pay gap için standart yöntem

### Comp Cycle Best Practices
- **WorldatWork Total Rewards Model** — sabit + değişken + benefits + recognition + development
- **Mercer Compensation Planning Survey** — yıllık zam yüzdesi piyasa eğilimi

---

## Rakip Ürünler — Detaylı Analiz

### Kurumsal Global Rakipler

#### Workday Compensation
- **URL**: https://www.workday.com/en-us/products/total-rewards/compensation-management.html
- **Pazar**: Fortune 1000, kurumsal
- **Fiyat**: Yıllık 50K-500K USD (kullanıcı sayısına göre)
- **Güçlü**: Olgun, geniş entegrasyon, AI tahminleri
- **Zayıf (Türkiye)**: Türk vergi yapısı zayıf, fiyat yüksek, KVKK zorlukları, on-premise yok (cloud only)

#### SAP SuccessFactors Compensation
- **URL**: https://www.sap.com/products/hcm/compensation-management.html
- **Pazar**: SAP HCM müşterileri
- **Fiyat**: SAP lisans paketinin parçası, 30-100K USD/yıl
- **Güçlü**: SAP ile sıkı entegrasyon
- **Zayıf**: Karmaşık konfigürasyon, Türkçe lokal mevzuat zayıf

#### Oracle HCM Cloud Compensation
- **URL**: https://www.oracle.com/human-capital-management/compensation/
- Benzer profil — Oracle HCM müşterilerine satılıyor

#### Beqom
- **URL**: https://www.beqom.com
- **Odak**: Sales compensation, complex variable comp
- **Türkiye**: pazarda görünür değil

### Mid-Market Global
- **PayScale**: https://www.payscale.com — benchmark veri tabanı + komp planlama
- **CompTryx by Mercer**: salary data odaklı
- **Pave**: https://www.pave.com — startup/tech şirketler için

### Türkiye Yerel
- **Logo İK / Bordro Plus**: https://www.logo.com.tr — **bordro odaklı**, comp policy modülü zayıf
- **Mikro Bordro**: https://www.mikro.com.tr — bordro
- **Netsis İK**: https://www.netsis.com.tr — bordro
- **Bordro.io**: https://bordro.io — KOBİ bordrosu, SaaS
- **Paraşüt İK**: https://www.parasut.com — KOBİ bordrosu
- **Zirve İK**: bordro
- **Kuika** (low-code) — özelleştirilmiş İK çözümleri

**Sonuç**: Türkiye'de **comp policy odaklı, kurumsal, on-premise yerli yazılım yok**. Bu gerçek bir boşluk.

---

## Türk İK Profesyonel Toplulukları (Pazarlama / Network)

- **PERYÖN** (Türkiye İnsan Yönetimi Derneği): https://www.peryon.org.tr — en büyük İK derneği
- **TKYD** (Türkiye Kurumsal Yönetim Derneği)
- **HRDergi**: https://www.hrdergi.com — sektör yayını
- **HR Dergi**: yayın + etkinlik
- **PERYÖN Yıllık Konferansı** — comp policy etkinliği için ideal demo platformu

---

## Danışmanlık Firmaları (Tehdit + Fırsat)

Şu an bu firmalar müşterilerine "comp policy danışmanlığı" satıyor. Bizim ürünümüz onları destekleyebilir veya rakip olabilir.

- **Mercer Türkiye**: https://www.mercer.com.tr — sürvey + danışmanlık
- **Korn Ferry Türkiye**: https://www.kornferry.com — sürvey + danışmanlık
- **Willis Towers Watson Türkiye**: https://www.wtwco.com — sürvey + benefits
- **KPMG İK Danışmanlığı**: https://kpmg.com/tr/tr/home/services/people-services.html
- **Deloitte Human Capital**: https://www2.deloitte.com/tr — sürvey + transformation
- **Aon Türkiye**: https://www.aon.com — total rewards

**Strateji**: Bu firmalar potansiyel **partner** olabilir — onlar danışmanlık satarken biz yazılımı sağlarız. Doğru konumlandırmayla "Mercer'in tavsiye ettiği yerel yazılım" pozisyonu altın değerinde.

---

## Teknoloji Referansları

### .NET / Blazor
- **.NET 9 docs**: https://learn.microsoft.com/dotnet/
- **Blazor Server**: https://learn.microsoft.com/aspnet/core/blazor/
- **Entity Framework Core**: https://learn.microsoft.com/ef/core/
- **ASP.NET Core Identity**: https://learn.microsoft.com/aspnet/core/security/authentication/identity

### Bileşen Kütüphaneleri
- **Syncfusion Blazor**: https://www.syncfusion.com/blazor-components — kurumsal lisans, geniş bileşen seti
- **DevExpress Blazor**: https://www.devexpress.com/blazor/ — kurumsal, çok güçlü Grid
- **MudBlazor** (free, alternatif): https://mudblazor.com
- **Telerik UI for Blazor**: https://www.telerik.com/blazor-ui

### Excel / PDF
- **ClosedXML** (Excel, free): https://github.com/ClosedXML/ClosedXML
- **EPPlus** (Excel, ücretsiz/lisanslı): https://epplussoftware.com
- **QuestPDF** (PDF, modern): https://www.questpdf.com
- **iText 7** (PDF, kurumsal lisans): https://itextpdf.com

### Background Jobs
- **Hangfire**: https://www.hangfire.io
- **Quartz.NET**: https://www.quartz-scheduler.net (alternatif)

### Logging / Monitoring
- **Serilog**: https://serilog.net
- **Seq** (log viewer): https://datalust.co/seq
- **Application Insights**: Azure Monitor (cloud, opsiyonel)

### Test
- **xUnit**: https://xunit.net
- **FluentAssertions**: https://fluentassertions.com
- **Bogus** (test data): https://github.com/bchavez/Bogus
- **Playwright** (UI test): https://playwright.dev/dotnet
- **NBomber** (load test): https://nbomber.com

### Authentication
- **Windows Authentication / AD**: ASP.NET Core built-in
- **OpenID Connect / OAuth2**: https://learn.microsoft.com/aspnet/core/security/authentication/configure-oidc-web-authentication

---

## KVKK Uyum Kaynakları

- **KVKK Resmi**: https://www.kvkk.gov.tr
- **Kararlar ve İlke Kararları**: https://www.kvkk.gov.tr/Icerik/2089/Kurul-Kararlari
- **Veri Sorumluları Sicili (VERBIS)**: https://verbis.kvkk.gov.tr

### KVKK Uyum Checklist (Ürün Geliştirme)
- [ ] Veri sorumlusu kayıt (müşteri kendisi yapar, ürünümüz veri işleyici sıfatıyla yardımcı olur)
- [ ] Açık rıza akışları (gerekirse)
- [ ] Anonimleştirme/silme talebi prosedürü
- [ ] Veri ihracatı (data portability)
- [ ] Audit log saklama süresi
- [ ] Veri sızıntısı bildirim akışı (72 saat)
- [ ] DPO (Data Protection Officer) etkileşim arayüzü

---

## Çeviri / Terminoloji

Türkçe-İngilizce comp policy terimleri:

| EN | TR | Notlar |
|---|---|---|
| Compensation policy | Ücret politikası | Ürünün ana konusu |
| Job grade | Kademe | Türk kurumsal kullanımı: "kademe" yaygın |
| Salary band | Maaş bandı / Ücret aralığı | "Bant" kısaltma |
| Compa-ratio | Compa-ratio | Türkçesi yok, EN olarak kullanılır |
| Range penetration | Bant penetrasyonu | Veya "bant pozisyonu" |
| Merit increase | Performans bazlı zam | "Merit" Türk İK'sında EN kullanılır |
| Pay equity | Ücret eşitliği / İç adalet | İki kullanım da var |
| Pay gap | Ücret farkı | Cinsiyet bağlamında |
| Compression | Compression / Sıkışma | EN tercih edilir |
| Career band | Kariyer bandı | Yaygın |
| Job family | İş ailesi | Doğrudan çeviri |
| Total rewards | Toplam ödül paketi | Yaygın |
| Off-cycle adjustment | Dönem dışı ayarlama | |
| Promotion | Terfi | |
| Retention bonus | Tutma primi | |
| Sign-on bonus | İşe giriş primi | |
| Variable pay / STI | Değişken ücret / Prim | |
| LTI | Uzun vadeli teşvik / Hisse opsiyonu | |
| Calibration | Kalibrasyon | |

---

## Geliştirme Sürecinde Başvurulacak Mevcut Kaynaklar

### Mevcut Python Programı
- Konum: `C:\Users\okand\OneDrive\Desktop\Programlar\IsciMaliyet\`
- Hesap motoru: `utils/calculations.py` ⭐ ana referans
- Vergi sabitleri: `utils/constants.py` ⭐ 2025 değerleri
- Zam mantığı: `tabs/general_raise.py`, `tabs/total_cost_target_with_min.py`, vb.
- Lisanslama: `licensing.py`, `activation_codes.py`

### Bu Spesifikasyon Klasörü
- Tüm doküman: `C:\Users\okand\OneDrive\Desktop\Refleks360-UcretPolitikasi\`
- Bu klasör tek doğruluk kaynağıdır — kararlar değiştikçe güncellenir

---

## Kullanıcı Tarafından Sağlanan Veriler (Tarihçe)

| Tarih | Veri | Kaynak |
|---|---|---|
| 2026-04-29 | 2026 Asgari Ücret: Net 28.075,50 TL / Brüt 33.030,00 TL | Kullanıcı (Okan) |
| 2026-04-29 | 2026 G.V.K. Madde 103 — ücret gelirleri tarifesi (5 dilim) | Kullanıcı (Okan) |
| 2026-04-29 | Verginet kaynak adresi: https://www.verginet.net/dtt/1/GelirVergisiTarifesi_3804.aspx | Kullanıcı (Okan) |

Bu kayıt, hangi sayısal değerin nereden geldiğini izlemek için tutulur. Resmi olmayan kaynaklardan gelen değerler **GİB ile teyit edilmelidir**.

---

## Daimi Açık Sorular

Bu sorular ürün geliştirme süreci boyunca cevaplanması gereken stratejik sorulardır:

- [ ] İlk pilot müşteri kimdir? Hangi sektör, kaç çalışan, hangi karar verici?
- [ ] Lisans modeli: çalışan başına / kullanıcı başına / sabit yıllık?
- [ ] Geliştirme ekibi: solo geliştirici mi, kurulan ekip mi?
- [ ] Marka adı: "Refleks 360 Ücret Politikası" mı, yeni isim mi?
- [ ] Mercer/WTW/Korn Ferry partner ilişkisi kurulacak mı?
- [ ] Bordro yazılımları (Logo, Mikro) ile entegrasyon partner anlaşması olacak mı?
- [ ] AB Pay Transparency Directive Türkiye'ye yansır mı? Ne zaman?
