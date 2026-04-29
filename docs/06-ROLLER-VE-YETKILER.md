# 06 — Roller ve Yetkiler (RBAC)

Çoklu kullanıcı ortamında kim ne yapabilir? Bu dokümanda standart roller, izinler ve özel kapsam (scope) kuralları tanımlı.

---

## Tasarım İlkeleri

1. **Role-Based Access Control (RBAC)** — kullanıcılar rollere atanır, izinler rollerde tanımlıdır
2. **Scope (Kapsam)** — bir rol global, şirket bazlı veya departman bazlı olabilir (örn. "X departmanının müdürü" sadece X'i görür)
3. **Field-Level Security** — bazı alanlar (TC, IBAN, doğum tarihi) sadece belirli rollerde görünür
4. **Least Privilege** — varsayılan olarak hiçbir izin yok; her şey explicit verilir
5. **Auditability** — kim hangi yetki ile ne yaptı, audit log'da

---

## Standart Roller (Sistem Tanımlı)

### 1. Sistem Yöneticisi (System Administrator)
**Kimdir**: IT departmanı, sistem yönetiyor — comp policy işine girmiyor.

**Yapabilir**:
- Kullanıcı/rol/yetki yönetimi
- Sistem ayarları (SMTP, AD, lisans)
- Vergi/SGK parametreleri
- Backup/restore
- Audit log görüntüleme

**Yapamaz**:
- Çalışan kişisel verisi görüntüleme (NetID, IBAN, doğum vb.)
- Maaş verisi görüntüleme
- Senaryo oluşturma/onaylama

> **KVKK ilkesi**: IT yöneticisi sistemi yönetebilmeli ama hassas İK verisine erişememelidir.

### 2. İK Direktörü (HR Director)
**Kimdir**: İK fonksiyonunun başı, comp policy'nin sahibi.

**Yapabilir**:
- Tüm çalışan verisi (görüntüleme + düzenleme)
- Tüm maaş verisi
- Kademe sistemi, maaş bantları yönetimi
- Tüm modülleri kullanma
- Senaryo oluşturma, onaya gönderme, onaylama (yetki seviyesine göre)
- Pay equity raporları
- Dashboard tüm veri

**Yapamaz**:
- Sistem ayarlarını değiştirme (sistem yöneticisi alanı)
- Yıllık vergi parametrelerini değiştirme (IT görevi)

### 3. İK Müdürü (HR Manager)
**Kimdir**: İK direktörünün altında, operasyonel yönetim.

**Yapabilir**:
- Çalışan veri girişi/güncelleme
- Senaryo oluşturma ve hesaplama
- Pay equity raporları (görüntüleme)
- Onay sürecinde adım — orta düzey

**Yapamaz**:
- Kademe sistemi değiştirme (sadece direktör)
- Maaş bantları belirleme (sadece direktör)
- Final onay (CFO/CEO görevi)

### 4. İK Uzmanı (HR Specialist)
**Kimdir**: Veri girişi, operasyonel destek.

**Yapabilir**:
- Çalışan veri girişi
- Excel import
- Standart raporları çekme
- Senaryo görüntüleme (oluşturma değil)

**Yapamaz**:
- Maaş değiştirme
- Senaryo onaylama
- Hassas alan (TC, IBAN) görüntüleme

### 5. Departman Müdürü (Department Manager)
**Kimdir**: İK dışı yönetici — kendi ekibi için zam önerisi yapar.

**Yapabilir** — sadece kendi departmanı için:
- Çalışan listesi görüntüleme (sadece ekibi)
- Performans skorları görme
- Merit matrix önerisi yapma (kendi ekibi)
- Kendi ekibinin maaş aralığını görme (bant aralığı, mutlak değil)

**Yapamaz**:
- Diğer departmanları görme
- Ekibinin **mutlak** maaş tutarlarını görme (sadece compa-ratio veya quartile gösterir)
- Kademe/bant yapısını değiştirme
- Senaryo final onayı

### 6. CFO / Mali İşler Direktörü
**Kimdir**: Bütçe sahibi, yüksek tutarlı zamları onaylar.

**Yapabilir**:
- Tüm dashboard ve özet rapor görme
- Bütçe etkisi analizi
- Onay süreci üst seviye (yüksek tutarlı zam, departman senaryosu)
- Senaryo görüntüleme

**Yapamaz**:
- Çalışan veri düzenleme
- Pozisyon/kademe yönetimi
- Bireysel personel işlemleri

### 7. CEO / Genel Müdür
**Kimdir**: Üst onay merci, stratejik bakış.

**Yapabilir**:
- Tüm raporları görme (read-only)
- En üst seviye onay (büyük bütçeli senaryolar, executive level değişiklikler)

**Yapamaz**:
- Veri girişi
- Operasyonel işlemler

### 8. Auditor / Denetçi
**Kimdir**: İç denetim, dış denetim, KVKK denetimi.

**Yapabilir**:
- Tüm audit log
- Tüm raporları okuma (read-only)
- Veri ihracatı (denetim raporu için)

**Yapamaz**:
- Hiçbir yazma işlemi

### 9. Çalışan (Employee)
**Kimdir**: Kendisi — sadece kendi verisini görür.

**Yapabilir**:
- Kendi profili (kişisel + iş bilgisi)
- Kendi maaş tarihçesi
- Kendi total rewards bildirimleri
- Kendi onay isteklerini takip

**Yapamaz**:
- Başka çalışanı görme
- Maaş değiştirme
- Hiçbir yönetim ekranına erişim

> **Not**: Çalışan rolü ilk versiyonda olmayabilir. Ücret bildirim mektubu PDF olarak e-posta ile gönderilir, çalışan girişi gerekmeyebilir. Faz 2-3'te eklenir.

---

## İzin (Permission) Kataloğu

İzinler `Modul.Eylem` formatında.

### Çalışan Modülü
- `Employee.View` — listele/oku
- `Employee.ViewSensitive` — TC, IBAN, doğum gibi hassas alanlar
- `Employee.ViewSalary` — maaş tutarı
- `Employee.ViewSalaryRange` — sadece bant aralığı (mutlak yok)
- `Employee.Create`
- `Employee.Update`
- `Employee.Delete` (soft delete)
- `Employee.Import` — Excel toplu yükleme
- `Employee.Export`
- `Employee.UpdateSalary` — bireysel maaş değişikliği
- `Employee.Anonymize` — KVKK silme

### Organizasyon
- `Department.View` / `Update` / `Create` / `Delete`
- `Position.View` / `Update` / `Create` / `Delete`
- `Location.View` / `Update`
- `JobGrade.View` / `Update`
- `JobFamily.View` / `Update`
- `SalaryBand.View` / `Update`

### Benchmark
- `Benchmark.View` / `Import` / `Update` / `Delete`
- `MarketIndex.View`

### Senaryo
- `Scenario.View`
- `Scenario.Create`
- `Scenario.Update` (sadece kendi oluşturduğu)
- `Scenario.UpdateAll` (tüm senaryolar)
- `Scenario.Submit` (onaya gönder)
- `Scenario.Apply` (uygulama — gerçek maaşlara işle)
- `Scenario.Delete`

### Onay
- `Approval.View`
- `Approval.ApproveLevel1` (departman müdürü düzeyi)
- `Approval.ApproveLevel2` (İK düzeyi)
- `Approval.ApproveLevel3` (CFO/CEO düzeyi)
- `Approval.Reject`
- `Approval.TemplateUpdate`

### Pay Equity
- `PayEquity.View`
- `PayEquity.Run` (analiz çalıştırma)
- `PayEquity.Export`

### Rapor
- `Report.RunStandard`
- `Report.RunCustom`
- `Report.GenerateLetter` (ücret bildirim mektubu)
- `Report.GenerateTotalRewards`
- `Report.Export`

### Yönetim
- `Admin.UserManagement`
- `Admin.RoleManagement`
- `Admin.SystemSettings`
- `Admin.TaxParameters`
- `Admin.AuditLog.View`
- `Admin.AuditLog.Export`
- `Admin.License`
- `Admin.Backup`

---

## Rol-İzin Matrisi (Özet)

| İzin | SysAdmin | İK Dir. | İK Müd. | İK Uzm. | Dep. Müd. | CFO | CEO | Auditor | Çalışan |
|---|---|---|---|---|---|---|---|---|---|
| Employee.View | ❌ | ✅ | ✅ | ✅ | ✅ (scope) | ✅ özet | ✅ özet | ✅ | ✅ self |
| Employee.ViewSensitive | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ self |
| Employee.ViewSalary | ❌ | ✅ | ✅ | ❌ | ❌ | ✅ özet | ✅ özet | ✅ | ✅ self |
| Employee.ViewSalaryRange | ❌ | ✅ | ✅ | ✅ | ✅ (scope) | ✅ | ✅ | ✅ | ❌ |
| Employee.Update | ❌ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Employee.UpdateSalary | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Employee.Anonymize | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| SalaryBand.Update | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| JobGrade.Update | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Benchmark.Import | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Scenario.Create | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Scenario.Apply | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Approval.ApproveLevel1 | ❌ | ❌ | ❌ | ❌ | ✅ (scope) | ❌ | ❌ | ❌ | ❌ |
| Approval.ApproveLevel2 | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Approval.ApproveLevel3 | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ |
| PayEquity.View | ❌ | ✅ | ✅ | ❌ | ❌ | ✅ | ✅ | ✅ | ❌ |
| Admin.UserManagement | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Admin.SystemSettings | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Admin.TaxParameters | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Admin.AuditLog.View | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ |

**Lejant**: ✅ Verilir | ❌ Yok | "scope" = sadece kendi kapsamında | "özet" = sadece toplu gösterim, bireysel yok | "self" = sadece kendisi

---

## Scope (Kapsam) Mekanizması

### Global Scope
Kullanıcı tüm veriyi görür (örn. İK Direktörü).

### Company Scope
Holding modunda — kullanıcı belirli şirket(ler)e atanmış.
```
WHERE Employee.CompanyId IN (user.allowed_company_ids)
```

### Department Scope
Kullanıcı belirli departman(lar)a atanmış. Hiyerarşik:
```
WHERE Employee.DepartmentId IN (user.allowed_dept_ids OR sub_departments)
```

Departman müdürü için scope **otomatik**: kendisi müdürü olduğu departman + alt departmanlar.

### Manager Scope
Departman müdürü değil ama "ekip yöneticisi" olan birisi sadece kendi raporlama hattı:
```
WHERE Employee.ManagerId = user.employee_id
   OR Employee.ManagerId IN (recursive_subordinates)
```

---

## Field-Level Security

Bazı alanlar `[SensitiveField]` attribute ile işaretli. Sadece `Employee.ViewSensitive` izni olanlar görür.

**Hassas alanlar**:
- `NationalId` (TC kimlik)
- `BirthDate`
- `IBAN` (banka)
- `Address`
- `EmergencyContact`
- `HealthInfo` (sağlık beyanları)

UI'da diğer roller bu alanları **maskelenmiş** görür: `***********`

---

## Onay Akışı Yetkileri

Onay matrisini özelleştirilebilir kılıyoruz:

### Tetikleyici × Threshold × Onaylayıcı

| Senaryo Tipi | Threshold | Onay Zinciri |
|---|---|---|
| Bireysel zam | < %15 | Departman Müdürü → İK Müdürü |
| Bireysel zam | %15-30 | Dep. Müd. → İK Müd. → İK Direktörü |
| Bireysel zam | > %30 | Dep. Müd. → İK Müd. → İK Direktörü → CFO |
| Toplu zam (departman) | < bütçe limiti | İK Müd. → İK Direktörü |
| Toplu zam (departman) | > bütçe limiti | İK Müd. → İK Direktörü → CFO |
| Toplu zam (şirket geneli) | herhangi | İK Direktörü → CFO → CEO |
| Yeni işe alım — band içi | - | İK Müd. → Dep. Müd. |
| Yeni işe alım — band üstü | - | İK Müd. → İK Dir. → CFO |
| Off-cycle (retention vb.) | herhangi | İK Müd. → İK Dir. → CFO |

Threshold'lar sistem ayarlarından düzenlenir, koda gömülü değil.

---

## Self-Service (Kendi Verisini Görme)

Çalışan rolünde:
- Kendi maaşını görür
- Kendi total rewards bildirimini görür/indirir
- Bant aralığı **görmez** (gizlilik politikası)
- Compa-ratio **görmez** (kafa karışıklığı önlemek için)

Bu davranış **şirketin şeffaflık politikasına göre** ayardan açılıp kapatılabilir:
- "Çalışan kendi compa-ratio'sunu görsün mü?" — toggle
- "Çalışan kendi bant aralığını görsün mü?" — toggle

---

## Logout / Oturum Yönetimi

- Oturum süresi: 30 dakika hareketsizlik (ayardan)
- Eş zamanlı oturum: kullanıcı başına en fazla 2 (ayardan)
- "Beni hatırla" — yok (kurumsal güvenlik)
- 2FA: AD entegrasyonu varsa gereksiz; yerel hesaplar için opsiyonel TOTP

---

## Rol Atama Akışı

1. Sistem Yöneticisi `Kullanıcı Yönetimi`'nden kullanıcı oluşturur (veya AD'den içe aktarır)
2. Kullanıcıya bir veya birden fazla rol atar
3. Her role scope (kapsam) atar
4. Audit log'a kayıt: "X kullanıcısı Y rolüne atandı"

---

## Acil Erişim (Break-Glass)

Kritik durumlarda (sistem yöneticisi yok, İK direktörü acil yetki gerekiyor):
- "Break-glass" rolü — sınırlı süreli (örn. 24 saat)
- Aktivasyonu özel onay + audit log + e-posta bildirimi
- Süresi sonunda otomatik kapanır

İlk versiyonda olmayabilir; faz 2.

---

## Test Senaryoları (Yetki)

Zorunlu test:
1. Departman müdürü başka departman çalışanını listede göremiyor (DB seviyesinde de)
2. İK uzmanı maaş alanını göremiyor (UI'da gizli + API çağrısında 403)
3. Çalışan kendi dışında kayıt çağrısı yaparsa 403
4. Field-level: hassas alanlar yetkisiz kullanıcıya boş döner
5. Audit log'a unauthorized erişim denemesi düşer
6. Senaryo onaylama threshold'a göre doğru kişiye gider
