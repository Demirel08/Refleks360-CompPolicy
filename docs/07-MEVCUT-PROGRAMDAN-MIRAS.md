# 07 — Mevcut Programdan Miras

Mevcut **Refleks 360 Modül S** (Python/PySide6) programındaki kod ve mantığın yeni ürüne nasıl taşınacağı.

> Mevcut programın konumu: `C:\Users\okand\OneDrive\Desktop\Programlar\IsciMaliyet\`
>
> Toplam: ~12.000 satır Python kodu, 13 sekme, JSON tabanlı kalıcılık, masaüstü tek kullanıcılı.

---

## Genel Yaklaşım

**Yeni ürün sıfırdan yazılır.** Mevcut kod taşınmaz — Python → C# direkt çeviri yerine, mevcut programdaki **iş mantığı** referans alınarak C# olarak yeniden yazılır.

Aşağıdaki listede her mevcut modül için karar:
- **TAŞI** — bu mantık birebir alınacak, sadece C# yeniden yazımı
- **GENİŞLET** — mevcut özellik korunacak ama iyileştirilecek
- **KALDIR** — bu özellik comp policy ürününün dışında, yapılmayacak
- **BİRLEŞTİR** — başka modülle birleşecek
- **DÖNÜŞTÜR** — temelden yeniden konumlandırılacak

---

## Sekme Bazlı Karar Tablosu

| Mevcut Sekme | Dosya | Karar | Açıklama |
|---|---|---|---|
| Çalışan Girişi | [tabs/employee_entry.py](../Programlar/IsciMaliyet/tabs/employee_entry.py) | **GENİŞLET** | Çekirdek olarak kalır, kademe/pozisyon/performans alanları eklenir |
| Genel Zam | [tabs/general_raise.py](../Programlar/IsciMaliyet/tabs/general_raise.py) | **TAŞI** | Mantık aynen, "Merit Cycle" altında |
| Departman Zam | [tabs/dept_raise.py](../Programlar/IsciMaliyet/tabs/dept_raise.py) | **TAŞI** | Aynen |
| Pozisyon Zam | [tabs/position_raise.py](../Programlar/IsciMaliyet/tabs/position_raise.py) | **TAŞI** | Aynen |
| Toplam Hedef | [tabs/total_cost_target.py](../Programlar/IsciMaliyet/tabs/total_cost_target.py) | **BİRLEŞTİR** | "Toplam Hedef (Min)" ile tek sayfada toggle |
| Toplam Hedef (Min) | [tabs/total_cost_target_with_min.py](../Programlar/IsciMaliyet/tabs/total_cost_target_with_min.py) | **BİRLEŞTİR** | Yukarıdakiyle birleşir |
| Departman Hedef | [tabs/dept_cost_target.py](../Programlar/IsciMaliyet/tabs/dept_cost_target.py) | **BİRLEŞTİR** | "Departman (Min)" ile tek sayfada toggle |
| Departman (Min) | [tabs/dept_cost_target_with_min.py](../Programlar/IsciMaliyet/tabs/dept_cost_target_with_min.py) | **BİRLEŞTİR** | Yukarıdakiyle birleşir |
| Simülasyon | [tabs/simulation.py](../Programlar/IsciMaliyet/tabs/simulation.py) | **TAŞI** | İşe alım/çıkış simülasyonu — comp policy'de değerli |
| Nakit Akışı | [tabs/daily_cashflow.py](../Programlar/IsciMaliyet/tabs/daily_cashflow.py) | **KALDIR** | Treasury/finans işi, comp policy değil |
| Ödeme Takvimi | [tabs/payment_schedule.py](../Programlar/IsciMaliyet/tabs/payment_schedule.py) | **KALDIR** | Aynı sebep |
| Dashboard | [tabs/dashboard.py](../Programlar/IsciMaliyet/tabs/dashboard.py) | **DÖNÜŞTÜR** | Comp policy KPI'larıyla yeniden tasarım |
| Rapor | [tabs/report.py](../Programlar/IsciMaliyet/tabs/report.py) | **GENİŞLET** | Excel/PDF mantığı kalır, yeni rapor tipleri eklenir |
| Ayarlar | [tabs/settings.py](../Programlar/IsciMaliyet/tabs/settings.py) | **GENİŞLET** | Vergi parametreleri korunur, çoklu kullanıcı/RBAC eklenir |

---

## Hesap Motoru — En Kritik Miras

### Kaynak: [utils/calculations.py](../Programlar/IsciMaliyet/utils/calculations.py)

**TAŞI** — bu dosyadaki tüm fonksiyonlar C# karşılığına çevrilecek. Test edilmiş, çalışan, sayısal olarak doğru iş mantığı.

#### Taşınacak fonksiyonlar:
- `month_params(baseP, m, periods)` → C#: `TaxParameterService.GetMonthParams(year, month)`
- `clip_pek(base, pmin, pmax)` → C#: `decimal Clip(decimal value, decimal min, decimal max)`
- `annual_income_tax(amount, brackets)` → C#: `IncomeTaxCalculator.AnnualTax(decimal amount, List<TaxBracket>)`
- `find_marginal_rate(cum, brackets)` → C#: `IncomeTaxCalculator.MarginalRate(decimal cum, List<TaxBracket>)`
- `monthly_components_given_gross_and_cum(gross, cum_prev, Pm)` → C#: `SalaryCalculator.CalculateMonth(...)`

#### Taşınacak yapılar:
- 2025 vergi dilimleri (referans olarak 2026 değerleriyle değiştirilecek)
- Aylık dönem (PERIODS) yapısı → `MonthlyTaxPeriod` entity'si
- SGK matrah min/max kuralı

**Test stratejisi**: Mevcut Python programıyla aynı girdileri ver, C# çıktısının kuruşa kadar tuttuğunu doğrula. Bu **regression test** olarak kalmalı.

---

## Sabitler ve Konfigürasyon

### Kaynak: [utils/constants.py](../Programlar/IsciMaliyet/utils/constants.py)

**DÖNÜŞTÜR** — bu dosyadaki tüm değerler artık DB'de:

| Mevcut (Python) | Yeni (C# / DB) |
|---|---|
| `DEFAULT_PARAMS_CURRENT` | `TaxParameter` tablosu, Year=2026 |
| `DEFAULT_PARAMS_NEXT` | `TaxParameter` tablosu, Year=2027 |
| `DEFAULT_PERIODS_CURRENT` | `MonthlyTaxPeriod` tablosu, 12 satır |
| `DEFAULT_PERIODS_NEXT` | `MonthlyTaxPeriod` tablosu, 12 satır |
| `income_tax_brackets` | `IncomeTaxBracket` tablosu |
| `load_active_settings()` | EF Core ile sorgu, IMemoryCache |

**İlk seed verisi**: 2026 değerleri ([05-HESAPLAMA-MOTORU.md](05-HESAPLAMA-MOTORU.md)'de listelendi) DB seed migration'ı ile yüklenir.

---

## Veri Kalıcılığı

### Mevcut: JSON dosyaları
- `data.json` — çalışan listesi
- `dept_raise_settings.json` — departman zam ayarları
- `dept_target_settings.json` — departman hedef
- `dept_target_min_settings.json` — minimum hedef
- `general_raise_settings.json` — genel zam
- `position_raise_settings.json` — pozisyon zam
- `total_target_settings.json` — toplam hedef
- `total_target_min_settings.json` — toplam hedef min
- `settings.json` — vergi parametreleri

### Yeni: SQL Server (EF Core)
JSON yapıları → ilişkisel tablolara dönüşür. Bkz. [04-VERI-MODELI.md](04-VERI-MODELI.md).

**Migration aracı** (opsiyonel): Mevcut müşterinin Python verisini yeni sisteme taşıyacak bir komut satırı aracı yazılır:
- Python `data.json` oku → C# Migration tool → SQL Server'a yaz
- Sadece referans müşteriler için ihtiyaç olursa

---

## UI Bileşenleri

### Mevcut: PySide6 widgets
- `QTabWidget` (yatay sekme menüsü) → Blazor `NavMenu` + Layout
- `QTableWidget` (tablolar) → Syncfusion Blazor Grid veya DevExpress Blazor
- `QLineEdit`, `QComboBox` vb. → Blazor formlar
- `QChart` → ApexCharts.Blazor
- `tr_money_input.py` (Türkçe para girişi) → C# custom InputComponent

### Yeni: Blazor Server bileşenleri
Bileşen kütüphanesi (Syncfusion vs. DevExpress) kararı verildikten sonra UI yeniden çizilir. UI **görsel olarak** yeniden tasarlanır — Python uygulamasının görünümü değil, modern web standartları.

---

## Lisanslama

### Kaynak: [licensing.py](../Programlar/IsciMaliyet/licensing.py), [activation_codes.py](../Programlar/IsciMaliyet/activation_codes.py), [activate_yeni.py](../Programlar/IsciMaliyet/activate_yeni.py), [kod_uret.py](../Programlar/IsciMaliyet/kod_uret.py)

**DÖNÜŞTÜR** — temel mantık aynı (machine fingerprint, demo limiti, key validation) ama:
- **Demo limiti farklı**: 5 çalışan değil, 30 günlük tam fonksiyonel deneme
- **Lisans formatı**: çalışan sayısı + kullanıcı sayısı + bitiş tarihi imzalı
- **Aktivasyon dosyası**: müşteri sunucusunda — internet bağlantısı gerektirmez
- **Online lisans server'ı yok** — geliştirici manuel imzalı dosya üretip e-postayla gönderir

Mevcut programdaki güvenlik açığı bilinmeli (bkz. ilk inceleme): istemci tarafı kod kırılabilir. Yeni sistemde:
- Lisans imzalama: RSA-2048
- DB'de lisans kontrolü her oturum açılışında
- Süresi dolmuş lisans → read-only mod (mevcut programdaki "demo'ya düşme" mantığının kurumsal versiyonu)

---

## Bilinen Bug'lar (Düzeltilmesi Gereken)

Mevcut programdan tespit edilen — yeni ürünü etkilemez ama dökümante edelim:

1. [main.py:124-128](../Programlar/IsciMaliyet/main.py:124) — String birleştirme bug, satır boşlukları kaybolmuş
2. [main.py:137-139](../Programlar/IsciMaliyet/main.py:137) — Aynı tip bug, geçersiz aktivasyon kodu mesajı
3. [main.py:406](../Programlar/IsciMaliyet/main.py:406) — "Kullanım Kılavuuzu" tipo
4. [main.py:226](../Programlar/IsciMaliyet/main.py:226) — `on_tab_changed` 11 elif bloğu, dict-table refactor gerekir
5. [widgets/metric_card.py](../Programlar/IsciMaliyet/widgets/metric_card.py) — Boş dosya
6. [widgets/__init__.py](../Programlar/IsciMaliyet/widgets/__init__.py) — Boş dosya
7. [data.json](../Programlar/IsciMaliyet/data.json) — Boş dosya

Yeni ürün sıfırdan yazıldığı için bu bug'lar otomatik düzelir.

---

## Eski Müşterilere Geçiş Yolu

### Senaryo: Mevcut Modül S kullanıcısı yeni ürüne geçecek

1. **Veri ihracatı** — mevcut programdan Excel export
2. **Yeni sistem kurulum** — müşteri sunucusu
3. **Veri içe aktarma** — Excel import (yeni programın standart import özelliği)
4. **Parametre konfigürasyonu** — yıllık vergi parametreleri DB'ye girilir
5. **Eğitim** — 1-2 günlük kullanıcı eğitimi
6. **Pilot kullanım** — 1 ay paralel kullanım

Yeni ürün **mevcut Modül S'in alternatifi değildir, üst katmanıdır**. Mevcut müşteriler için fiyatlandırma upgrade yolu önerilebilir.

---

## Yeni Üründe OLMAYACAK Mevcut Özellikler

Bilinçli bırakılan özellikler:

- ❌ **Günlük nakit akışı** — finans modülü
- ❌ **Aylık ödeme takvimi** — finans modülü
- ❌ **Detaylı bordro hesabı (kişi başı)** — bordro yazılımları yapar
- ❌ **PDF puantaj raporu** — bordro
- ❌ **AGİ hesabı** (artık yürürlükte değil) — kaldı, yeni ürünün kapsamında değil
- ❌ **Tek kullanıcı modu** — yeni ürün çoklu kullanıcı doğmuştur
- ❌ **Splash screen** — masaüstü kavramı, web'de yok
- ❌ **Lokal aktivasyon kodu girişi** — kurumsal aktivasyon farklı

---

## Yeniden Kullanım Listesi

### Doğrudan referans kaynak olarak alınacak Python dosyaları
1. `utils/calculations.py` — formüller
2. `utils/constants.py` — vergi tarifeleri (referans değer)
3. `tabs/general_raise.py` — zam mantığı
4. `tabs/total_cost_target_with_min.py` — minimum garantili optimizasyon
5. `tabs/dept_cost_target_with_min.py` — departman bazlı min garantili
6. `tabs/simulation.py` — işe alım/çıkış simülasyonu
7. `tabs/report.py` — Excel/PDF üretim mantığı

### İhmal edilebilir Python dosyaları
- `tr_money_input.py` — Blazor'da yeniden yazılır
- `loading_dialog.py` — Blazor'un kendi loading bileşeni
- `fonts.py`, `styles.py`, `icons.py` — UI kavramları farklı, yeniden tasarım
- `app_paths.py` — masaüstü konsepti, web'de yok
- `installer/`, `build/`, `dist/`, `*.spec` — yeni build pipeline

---

## Performans Karşılaştırma Beklentisi

Mevcut Python programının performans şikayetleri büyük olasılıkla şu üç sebepten:

1. **UI thread'inde hesap** — bazı tab'larda calculation_worker kullanılmıyor olabilir
2. **JSON ile tüm veriyi her sefer yükleme** — DB ile çözülecek
3. **QTableWidget'a tek tek setItem** — Blazor Grid + virtualization ile çözülecek

Yeni üründe hedef:
- 5000 çalışanlı şirket için açılış: **<1 saniye** (Python'da muhtemelen 5-10 sn)
- Senaryo hesaplama: **<3 sn** (Python'da muhtemelen 15-30 sn)
- UI yanıt süresi: **<200 ms** her etkileşim
