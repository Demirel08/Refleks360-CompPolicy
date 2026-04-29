# 01 — Proje Vizyonu

## Bir Cümlelik Tanım

**Refleks 360 Ücret Politikası**, 200+ çalışanlı Türk şirketlerinin İK departmanlarına; kademe sistemi kurma, maaş bantları tanımlama, piyasa karşılaştırması, yıllık zam (merit cycle) yönetimi ve pay equity analizi imkânı sunan, müşterinin kendi sunucusuna kurulan çoklu kullanıcılı web yazılımıdır.

---

## Pazarın Sorunu

Türkiye'de 200+ çalışanlı şirketlerin İK departmanlarının yıllık en büyük teknik problemi **ücret politikasını oluşturmak ve sürdürmektir**. Bu süreç şu an üç şekilde yürütülüyor:

1. **Excel ile** — hatalı, sürdürülemez, audit edilemez, çoklu kullanıcı yok, formül kopyala-yapıştır felaketi
2. **Pahalı global yazılımlarla** (Workday, SAP SuccessFactors) — yıllık 50K-500K USD + danışmanlık, Türk vergi/SGK yapısı zayıf, KVKK uyumsuz
3. **Danışmanlık firmalarına dışarıdan yaptırarak** (Mercer, Korn Ferry, KPMG) — proje başına 200K-2M TL, sürekli güncellenmiyor

**Boş alan**: Türkiye'de ücret politikası odaklı, yerel mevzuata uyumlu, kurumsal düzeyde olgun, on-premise yerli yazılım yoktur.

---

## Hedef Müşteri Profili

### Birincil hedef
- **Çalışan sayısı**: 200 - 5.000 arası
- **Sektör**: Üretim, perakende, finans, teknoloji, ilaç, lojistik (sektör fark etmez, yapı önemlidir)
- **Lokasyon**: Türkiye merkezli; çok lokasyonlu, holding yapılı şirketler tercih
- **İK olgunluğu**: Profesyonel İK direktörü/müdürü olan, yapılandırılmış zam dönemi yapan şirketler
- **Veri politikası**: Veri dışarı çıkmasın isteyen, on-premise tercih eden
- **Bütçe**: Yıllık yazılım bütçesi 100K-1M TL aralığında

### İkincil hedef (sonraki fazlar)
- 5.000+ çalışanlı holdingler — özelleştirme, çok şirket modülü
- Kamu/yarı kamu kurumları — özel kademe yapıları

### Hedef dışı
- 200 altı şirketler — bu ürünü hak edecek karmaşıklığa sahip değiller, mali yükü kaldıramazlar
- Bordro hizmeti arayanlar — biz bordro yapmıyoruz
- SaaS arayanlar — biz on-premise satıyoruz

---

## Değer Önerisi

| Müşterinin Derdi | Çözümümüz |
|---|---|
| "Excel'de zam dönemi kâbus oluyor" | Yapılandırılmış merit cycle akışı, çoklu kullanıcı eş zamanlı |
| "Piyasaya göre nerede olduğumuzu bilmiyoruz" | Benchmark import + market index dashboard |
| "Aynı işi yapan iki kişi farklı maaş alıyor" | Pay equity ve compression analizi |
| "Veriler dışarı çıkamaz" | %100 on-premise, müşterinin sunucusunda |
| "Workday/SAP çok pahalı ve kurması yıl sürüyor" | Türk şirketleri için sade, 4-6 haftada canlıya alınır |
| "Yabancı yazılım Türk vergi yapısını anlamıyor" | SGK, GV, asgari ücret istisnası, damga doğru hesap |
| "Toplu zam sonrası kıdem yükü ne olur?" | Yükümlülük simülasyonu yerleşik |
| "Kim hangi maaşı ne zaman değiştirdi?" | KVKK uyumlu audit log |

---

## İş Modeli

### Lisanslama
- **Yıllık abonelik** (recurring revenue) — sürüm güncellemesi + destek dahil
- **Çalışan başına yıllık fiyatlandırma** — 200-1000 çalışan için 80-150 TL/çalışan/yıl bandı (örnek; pazara göre netleşir)
- **Modül bazlı paketleme** (sonraki fazlar) — Comp Policy + Pay Equity + Bonus Management gibi katmanlar

### İlk yıl gelir kalemleri
1. **Kurulum ücreti** (one-time) — sunucu kurulumu, eğitim, parametre konfigürasyonu — 50K-150K TL
2. **Yıllık abonelik** — kullanıcı sayısı + çalışan sayısı çarpımıyla
3. **Özelleştirme/danışmanlık** (opsiyonel) — kademe sistemi kurma, mevzuat eğitimi

### Müşteri Edinme Maliyeti
- Kurumsal satış döngüsü: 3-9 ay
- Pilot proje → ana sözleşme yaygın bir desen
- Referans listesi kritik — ilk 3 müşteri "logo müşteri" olarak konumlandırılmalı

---

## Rekabet Konumu

### Global rakipler (yukarı pazarda)
- **Workday Compensation** — kurumsal, $$$$, Türkiye desteği zayıf
- **SAP SuccessFactors Compensation** — kurumsal, $$$$, Türk şirketlerinin ERP'si SAP olanlar
- **Oracle HCM Cloud Compensation** — kurumsal, $$$$
- **Beqom** — sales comp odaklı
- **PayScale** — orta ölçek, benchmark veri tabanı odaklı

### Yerli rakipler
- **Logo İK / Mikro İK / Netsis İK** → bordro odaklı, comp policy modülü ya yok ya çok zayıf
- **Bordro.io / Paraşüt İK** → KOBİ odaklı, kurumsal değil
- **Doğrudan rakip yok** — bu pazar boş

### Savunulabilir farklılıklar
1. **Yerel mevzuat** — SGK matrah, GV dilimleri, asgari ücret istisnası, damga, AGİ tarihçesi
2. **Türkçe UI ve destek** — kurumsal satışta belirleyici
3. **On-premise** — KVKK ve veri egemenliği isteyen şirketler için zorunluluk
4. **Logo/Mikro/Netsis entegrasyonu** — bordro yapılan sistemden veri çekmek
5. **Fiyat** — Workday/SAP'nin %5-10'u

---

## Ürün Felsefesi (Değişmez İlkeler)

1. **Bordro değil, politika ürünüyüz.** Bordro hesabı destekleyici fonksiyondur, ana özellik değildir. Bordro yapan ürünlerle rekabete girmeyiz, onlardan veri çekeriz.
2. **Sektörel veri içermeyiz.** Müşteri kendi benchmark verisini girer. Veri sağlayıcısı değiliz, araç sağlayıcısıyız.
3. **Sayısal doğruluk pazarlık konusu değildir.** Bir vergi dilimi yanlışsa ürün baştan reddedilir. Tüm sabitler kullanıcı tarafından düzenlenebilir, koda gömülmez.
4. **On-premise tek dağıtım modelimizdir.** SaaS sürümü gündem değildir.
5. **Çoklu kullanıcı temel mimaridir.** Tek kullanıcı modu yoktur.
6. **KVKK uyumu ürünün omurgasıdır.** Audit log, rol bazlı yetki, kişisel veri silme — sonradan eklenmez, ilk versiyondan vardır.
7. **"İK'nın tüm sayısal sorunları" hedefine direnç gösterilir.** Ürün dar ve derin tutulur. İzin takibi, performans yönetimi, eğitim takibi gibi konulara genişlemez.

---

## Başarı Kriteri (12 Aylık)

- 5-10 referans müşteri (her biri 200+ çalışanlı, marka değeri olan)
- Yıllık recurring revenue 5M TL eşiği
- Net Promoter Score (NPS) 40+
- İlk 3 müşteriden 1'i sözleşme yenilemeli (validation)
- Mercer/WTW/KPMG gibi danışmanlık firmalarından en az biri ürünü tavsiye etmeli
