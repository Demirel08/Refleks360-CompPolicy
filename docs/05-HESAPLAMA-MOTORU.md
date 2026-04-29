# 05 — Hesaplama Motoru

> **KRİTİK**: Bu dosyadaki sayısal değerler ve formüller ürünün doğruluğunu belirler. Bir hatanın iş etkisi yüksek (yanlış brüt-net hesabı, yanlış vergi). Tüm sabitler **veritabanından okunur, koda gömülmez**. Bu dosya formüllerin nasıl çalışacağını ve referans değerleri belgeler.

---

## Sayısal Sabitler — 2026 (Kullanıcı tarafından sağlanan / mevcut programdan)

### Asgari Ücret 2026
- **Brüt aylık**: 33.030,00 TL
- **Net aylık**: 28.075,50 TL

### SGK Matrah Sınırları 2026 (taban × 7,5 kuralı)
- **SGK Matrah Min**: 33.030,00 TL (= brüt asgari ücret)
- **SGK Matrah Max**: 247.725,00 TL (= 33.030 × 7,5)

> **Doğrulama notu**: SGK matrah max her yıl SGK tarafından açıklanır. Brüt asgari ücret × 7,5 formülü genelde doğrudur ama yıllık olarak resmi rakam kontrol edilmelidir. Kullanıcıdan teyit alınacak.

### Gelir Vergisi Tarifesi 2026 — Ücret Gelirleri
Kaynak: 2026 takvim yılı G.V.K. Madde 103 (kullanıcı tarafından sağlandı).

| Üst sınır (yıllık kümülatif matrah) | Oran | Sabit tutar (önceki dilimlerin toplam vergisi) |
|---|---|---|
| 190.000 TL'ye kadar | %15 | 0 |
| 400.000 TL'ye kadar | %20 | 28.500 TL |
| 1.500.000 TL'ye kadar (ücret) | %27 | 70.500 TL |
| 5.300.000 TL'ye kadar (ücret) | %35 | 367.500 TL |
| 5.300.000 üstü | %40 | 1.697.500 TL |

> **Önemli**: Ücret dışı gelirlerde 27% dilimi 1.000.000 TL'ye kadar, 35% dilimi 1.500.000 - 5.300.000 arasıdır. Bu üründe sadece **ücret tarifesi** kullanılacaktır.

### Diğer Sabit Oranlar (mevcut programdan, 2026 için doğrulama gerekir)
- **SGK İşçi**: %14
- **İşsizlik İşçi**: %1
- **SGK İşveren**: %20,75
- **İşsizlik İşveren**: %2
- **SGK İşveren Teşvik İndirimi (5510 5/I-i)**: %5 (uygulanırsa)
- **Damga Vergisi**: binde 7,59 (%0,759)

### Asgari Ücret İstisnası (Yıllık Geçiş Mantığı 2026)
Yıl içinde kümülatif matrah arttıkça istisna oranı değişir:

- **Aylık brüt asgari ücret üzerinden hesaplanan vergi tutarı kadar GV istisnası** — bu istisna her ay hesaplanır:
  - Asgari ücretten elde edilen aylık matrah: 33.030 - (33.030 × 0,15) = 33.030 - 4.954,50 = 28.075,50 TL
  - Bu matrahın yıllık kümülatif değeri ay ay ilerler
  - 28.075,50 × 6 = 168.453 → 7. ayda kümülatif 196.528,50 olur (190K bracket geçilir)
  - **1-6. aylar**: %15 dilimde, istisna = 28.075,50 × %15 = **4.211,33 TL**
  - **7. ay**: bracket geçişi var, kısmi hesap (mevcut programdaki gibi)
  - **8-12. aylar**: %20 dilimde, istisna = 28.075,50 × %20 = **5.615,10 TL**

- **Damga istisnası**: brüt asgari ücret × damga oranı = 33.030 × 0,00759 = **250,70 TL/ay** (her ay sabit)

> Bu hesap mevcut Python programında doğru yapılıyor (`utils/calculations.py` → `monthly_components_given_gross_and_cum`). Yeni C# implementasyonu birebir aynı mantığı izlemeli.

---

## Net Hesaplama Algoritması (Brüt → Net)

### Girdi
- `gross` — aylık brüt
- `cumulative_taxable_prev` — yıl başından bu aya kadar olan kümülatif vergi matrahı
- `month_period` — o ayın geçerli SGK matrah, asgari ücret istisna parametreleri
- `tax_brackets` — gelir vergisi dilimleri (yıllık kümülatif)
- `tax_rates` — SGK/işsizlik oranları (config'den)

### Adımlar

```
1. PEK (Prim Esasına Esas Kazanç) = clip(gross, sgk_base_min, sgk_base_max)
2. SGK_isci = PEK × sgk_employee_rate (0.14)
3. Issizlik_isci = PEK × unemp_employee_rate (0.01)
4. Toplam_isci_kesintisi = SGK_isci + Issizlik_isci

5. Aylik_vergi_matrahi = max(0, gross − Toplam_isci_kesintisi)

6. Kumulatif_yeni = cumulative_taxable_prev + Aylik_vergi_matrahi
7. GV_yıllık_yeni = annual_income_tax(Kumulatif_yeni, tax_brackets)
8. GV_yıllık_eski = annual_income_tax(cumulative_taxable_prev, tax_brackets)
9. GV_ham = max(0, GV_yıllık_yeni − GV_yıllık_eski)

10. GV_istisna_matrahi = min(month_period.gv_exemption_amount, Aylik_vergi_matrahi)
11. GV_istisna = GV_istisna_matrahi × month_period.gv_exemption_rate
12. GV_son = max(0, GV_ham − GV_istisna)

13. Damga_ham = gross × stamp_tax_rate (0.00759)
14. Damga_istisna = min(month_period.stamp_exemption_amount, Damga_ham)
15. Damga_son = max(0, Damga_ham − Damga_istisna)

16. NET = gross − Toplam_isci_kesintisi − GV_son − Damga_son
```

### Yıllık vergi hesabı (annual_income_tax fonksiyonu)
```
def annual_income_tax(amount, brackets):
    tax = 0
    prev_limit = 0
    for limit, rate in brackets:
        portion = min(amount, limit) - prev_limit
        if portion > 0:
            tax += portion * rate
            prev_limit = limit
        if amount <= limit:
            break
    return tax
```

### Marjinal oran (find_marginal_rate)
```
def find_marginal_rate(cumulative, brackets):
    for limit, rate in brackets:
        if cumulative <= limit:
            return rate
    return brackets[-1].rate
```

---

## İşveren Maliyeti (Brüt → Toplam İşveren Yükü)

```
1. PEK = clip(gross, sgk_base_min, sgk_base_max)
2. Isveren_SGK_ham = PEK × 0.2075
3. Isveren_Issizlik_ham = PEK × 0.02

4. (Eğer SGK İndirim aktifse - 5510 5/I-i)
   SGK_indirim = PEK × 0.05
   Isveren_SGK = max(0, Isveren_SGK_ham − SGK_indirim)
ELSE
   Isveren_SGK = Isveren_SGK_ham

5. Isveren_toplam_yuku = gross + Isveren_SGK + Isveren_Issizlik_ham
```

---

## Comp Policy Metrikleri

### Compa-Ratio
**Tanım**: Çalışanın maaşının bant ortasına oranı.
```
compa_ratio = employee.gross_monthly / band.mid
```

**Yorumlama**:
- < 0,80: bandın çok altında (genelde yeni başlayan veya az deneyimli — eğer kıdemli ise underpaid)
- 0,80 - 0,95: bandın altı yarısı
- 0,95 - 1,05: bant ortası civarı (normal)
- 1,05 - 1,20: bandın üst yarısı (kıdemli, yüksek performans)
- > 1,20: bant üstü (genelde overpaid; istisna: az pozisyonlu üst kademe)

### Range Penetration (Bant Penetrasyonu)
**Tanım**: Çalışan bandın neresinde — 0 = min, 1 = max.
```
range_penetration = (employee.gross_monthly - band.min) / (band.max - band.min)
```

**Quartile** atama:
- 0,00 - 0,25 → Q1
- 0,25 - 0,50 → Q2
- 0,50 - 0,75 → Q3
- 0,75 - 1,00 → Q4
- < 0 veya > 1 → "out of range"

### Range Spread
**Tanım**: Bandın genişliği.
```
range_spread = (band.max - band.min) / band.min × 100  // %
```
Tipik: %40-60.

### Midpoint Progression
**Tanım**: İki ardışık kademe arasındaki mid sıçraması.
```
midpoint_progression = (next_grade.mid - this_grade.mid) / this_grade.mid × 100  // %
```
Tipik: %10-15.

### Market Index
**Tanım**: Şirketin medyanı / piyasa medyanı.
```
market_index = company_median(position) / market_p50(position)
```

**Şirket medyanı** = pozisyondaki tüm çalışanların maaş medyanı.

### Pay Gap (Cinsiyet)
**Ham gap**:
```
gap_raw = 1 - (avg_female_salary / avg_male_salary)
```

**Düzeltilmiş gap (regression)**:
- Bağımsız değişkenler: kademe, pozisyon, kıdem, lokasyon
- Bağımlı: ln(maaş)
- Cinsiyet katsayısı = düzeltilmiş gap

### Compression
**Tanım**: Aynı pozisyonda yeni alınan ile eski çalışanın maaş farkı.
```
compression_ratio = new_hire_salary / existing_avg_salary
```
- > 0,95 → compression var (yeni çalışan eski çalışana yakın veya üzerinde)

### Kıdem Tazminatı Yükümlülüğü (Severance Liability)
**Tanım**: Bilanço karşılığı için, her çalışanın 1 yıllık brütünün (yıl × tavan) toplamı.
```
seniority_years = (today - hire_date) / 365.25
severance_amount = min(employee.gross_monthly, severance_ceiling) * seniority_years
total_liability = sum(severance_amount for employee in employees)
```

> **Kıdem tavanı 2026**: Maliye Bakanlığı duyurur (genelde 6 ayda bir güncellenir). Ayarlardan girilecek.

---

## Asgari Ücret Etkisi

Şirketin asgari ücret altında veya yakın çalışanı varsa, asgari ücret her değiştiğinde otomatik güncelleme:

```
for employee in employees:
    if employee.gross_monthly < new_minimum_wage_gross:
        employee.gross_monthly = new_minimum_wage_gross
        # log audit
```

Kullanıcı bu otomatik güncellemeyi açıp kapatabilir. Toplu uyarı: "X çalışan asgari ücret altında, otomatik güncellensin mi?"

---

## Yıl İçi Vergi Geçişleri

Mevcut programdaki gibi, **aylık dönem (MonthlyTaxPeriod)** sistemi:

- Yılın belirli aylarında (örn. Temmuz'da AGİ değişimi geçmişte olduğu gibi) parametreler değişebilir
- Her ayın hesaplaması o aydaki geçerli parametreleri kullanır
- Kümülatif vergi matrahı yıl boyunca taşınır

Örnek (2025 program verisi — referans):
- Ocak-Temmuz: gv_exemption_rate = 0,15
- Ağustos-Aralık: gv_exemption_rate = 0,20

2026 için: kullanıcı her ay için doğru oranı `MonthlyTaxPeriod` tablosuna girer.

---

## Senaryo Hesaplama (Toplu)

### Genel zam senaryosu
```
for employee in scenario_employees where not is_locked:
    new_gross = old_gross * (1 + raise_percent / 100)
    if min_wage_floor and new_gross < new_minimum_wage_gross:
        new_gross = new_minimum_wage_gross
    calculate_net(new_gross) → cache
    calculate_employer_cost(new_gross) → cache
```

### Hedefli zam (toplam bütçe)
**Mod A (Standart)**: Tüm kilidi açık çalışanlar oransal zam alır, hedefe ulaşılır.
```
locked_total = sum(salary for employees if locked)
unlocked_total = sum(salary for employees if not locked)
target_unlocked = target_total - locked_total
raise_percent = (target_unlocked / unlocked_total) - 1
apply raise_percent to unlocked employees
```

**Mod B (Min garantili)**: Herkes en az `min_percent` alır.
```
1. Önce herkese min_percent uygula
2. Kalan bütçe = target - applied_total
3. Kalanı performans veya diğer kritere göre dağıt
```

### Merit Matrix Hesaplama
```
for employee in scenario_employees:
    perf = employee.performance_score
    quartile = calculate_quartile(employee, band)
    raise_percent = matrix[perf_band][quartile]
    new_gross = old_gross * (1 + raise_percent / 100)
```

---

## Performans Hedefleri

| İşlem | Beklenen süre |
|---|---|
| Tek çalışan brüt→net | <1 ms |
| 1000 çalışan toplu hesap | <500 ms |
| 5000 çalışan senaryo (full calculation) | <3 saniye |
| Pay equity regression (5000 kişi) | <5 saniye |

5000+ çalışanlı senaryolar **Hangfire arka plan işi** olarak çalışır, kullanıcı UI'ı kilitlenmez.

---

## Test Vakaları (Zorunlu)

Hesap motoru için zorunlu unit test senaryoları:

1. **Asgari ücret çalışanı** (33.030 brüt → 28.075,50 net) — kuruşa kadar tutmalı
2. **Asgari ücretin biraz üstü** (örn. 35.000) — istisna doğru uygulanmalı
3. **Yüksek maaşlı** (örn. 250.000) — SGK tavanı doğru clip'lenmeli
4. **Yıl içinde dilim geçişi** — Temmuz/Ağustos geçişlerinde GV doğru
5. **Asgari ücret değişimi** — yıl ortasında min ücret değişirse hesap doğru
6. **SGK indirimi açık/kapalı** — iki sonuç da doğru
7. **Engelli istisnası** (ileride eklenecek) — placeholder
8. **Yabancı para bazlı maaş** (TL'ye çevrilmiş) — döviz parametresi
9. **Bant kontrolü** — compa-ratio sınır değerleri
10. **Pay equity** — basit bir regression örneği

---

## Para Birimi ve Yuvarlama

- **İç hesaplamalar**: `decimal` (C#) — 4 ondalık, banker's rounding
- **Görüntüleme**: 2 ondalık, virgülle (Türkçe locale: 33.030,00 TL)
- **Toplam yuvarlama farkı**: tek tek yuvarlama yerine kümülatif tut, son adımda yuvarla
- **TL** birincil; USD/EUR opsiyonel sonraki fazlar için (kur tablosu gerekir)

---

## Tarih ve Zaman Dilimi

- Tüm tarihler **Europe/Istanbul** (UTC+3)
- DB'de UTC saklama; UI'da yerel zaman gösterimi
- Maaş `EffectiveDate` bir TARİHtir (saat değil)

---

## Bilinmeyenler / Sorulacaklar

- [ ] 2026 SGK matrah max kesin rakam (33.030 × 7,5 = 247.725 mi, başka mı?)
- [ ] 2026 damga oranı değişti mi (genelde sabit %0,759)
- [ ] 2026 SGK işveren teşvik oranı değişti mi
- [ ] 2026 kıdem tazminatı tavanı (Maliye duyurusu)
- [ ] AGİ tarihçesi nereden alınacak (eski hesaplar için)
