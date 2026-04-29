# Refleks 360 — Ücret Politikası Yönetim Sistemi

> Türk kurumsal şirketleri (200+ çalışan) için **ücret politikası (compensation policy)** oluşturma, yönetme ve analiz aracı. On-premise web uygulaması.

---

## Bu Klasör Nedir?

Bu klasör, geliştirilecek yeni ürünün **tam spesifikasyon dokümantasyonudur**. Bir yapay zeka aracı (Claude, ChatGPT, Copilot vb.) veya bir geliştirici bu klasördeki dosyaları sırayla okuduğunda:

1. Ürünün **ne olduğunu** ve **kimin için olduğunu** anlamalı
2. **Teknik mimariyi** (dil, veritabanı, dağıtım modeli) bilmeli
3. **Her modülün/sayfanın** ne yaptığını detaylı görmeli
4. **Veri modelini** ve **hesaplamaları** kod yazacak kadar net anlamalı
5. **Yapılacaklar listesi** ile öncelikleri görmeli

---

## Dosya Sırası ve Okuma Yolu

| # | Dosya | İçerik | Kim okumalı |
|---|---|---|---|
| 0 | [README.md](README.md) | Bu dosya — giriş ve navigasyon | Herkes |
| 1 | [01-PROJE-VIZYONU.md](01-PROJE-VIZYONU.md) | Ürün vizyonu, hedef pazar, değer önerisi, iş modeli | Herkes |
| 2 | [02-TEKNIK-MIMARI.md](02-TEKNIK-MIMARI.md) | Teknoloji yığını, dağıtım, güvenlik, çoklu kullanıcı | Geliştirici |
| 3 | [03-MODULLER-VE-EKRANLAR.md](03-MODULLER-VE-EKRANLAR.md) | Her sayfa/modül detayı — UI/UX/işlevsellik | Geliştirici + Tasarımcı |
| 4 | [04-VERI-MODELI.md](04-VERI-MODELI.md) | Tablolar, ilişkiler, veritabanı şeması | Geliştirici + DBA |
| 5 | [05-HESAPLAMA-MOTORU.md](05-HESAPLAMA-MOTORU.md) | Tüm formüller, vergi tarifeleri, SGK, comp policy metrikleri | Geliştirici |
| 6 | [06-ROLLER-VE-YETKILER.md](06-ROLLER-VE-YETKILER.md) | RBAC matrisi, kullanıcı tipleri, yetki sınırları | Geliştirici + Güvenlik |
| 7 | [07-MEVCUT-PROGRAMDAN-MIRAS.md](07-MEVCUT-PROGRAMDAN-MIRAS.md) | Mevcut Python programdan ne taşınacak/atılacak | Geliştirici |
| 8 | [08-YAPILACAKLAR.md](08-YAPILACAKLAR.md) | Fazlara göre TODO listesi, öncelikler | Proje Yöneticisi |
| 9 | [09-REFERANSLAR.md](09-REFERANSLAR.md) | Dış kaynaklar, metodoloji referansları, rakipler | Herkes |
| 10 | [10-KARARLAR-VE-TAKVIM.md](10-KARARLAR-VE-TAKVIM.md) | Kapatılmış teknik kararlar + 22 haftalık MVP takvimi + Faz 2/3 milestone'ları | Herkes |

---

## Bir Cümlelik Ürün Tanımı

> **200+ çalışanlı Türk şirketlerinin İK departmanlarının, ücret politikası oluşturmak için kullanacağı; kademe sistemi, maaş bantları, piyasa karşılaştırması, merit cycle yönetimi ve pay equity analizi yapan, on-premise olarak müşterinin kendi sunucusuna kurulan, çoklu kullanıcılı web tabanlı yazılım.**

---

## Temel İlkeler (Değişmez Kurallar)

1. **Veri müşterinin sunucusunda kalır.** Hiçbir veri dışarı çıkmaz. SaaS değildir, on-premise web uygulamasıdır.
2. **Sektöre özel kod yazılmaz.** Program her sektör için aynıdır. Sektörel veriyi (benchmark, vb.) müşteri kendisi girer.
3. **Bordro yapmaz.** Bordro/payroll Logo, Mikro, Netsis vb. ile yapılır. Bu ürün ücret **politikası** ürünüdür, ücret **işlemcisi** değil.
4. **Sayısal doğruluk kritiktir.** SGK, vergi, asgari ücret hesaplamalarında hata kabul edilemez. Tüm vergi tarifeleri ve oranlar `Ayarlar` sayfasından düzenlenebilir olmalı, koda gömülmemeli.
5. **Çoklu kullanıcı zorunludur.** Tek kullanıcı senaryosu yoktur. Eş zamanlı erişim ve rol bazlı yetki temel mimaridir.
6. **KVKK uyumludur.** Tüm değişiklikler audit log'a düşer, kişisel veri silme talepleri desteklenir.

---

## Önceki Ürün

Bu ürün, mevcut **Refleks 360 Modül S — Ücret Maliyet Simülasyonu** (Python/PySide6 masaüstü uygulaması) ürününün bir devamı **değil**, yeniden konumlandırılmış halefidir. Mevcut programdan taşınacak/atılacak öğeler [07-MEVCUT-PROGRAMDAN-MIRAS.md](07-MEVCUT-PROGRAMDAN-MIRAS.md) içinde.

---

## Çalışma Notu

Bu dokümanlar yaşayan belgelerdir. Geliştirme sürecinde karar değişiklikleri olduğunda **mutlaka güncellenmelidir**. Yapay zeka asistanları her oturumun başında bu klasördeki dosyaları okuyarak güncel context'i edinmelidir.
