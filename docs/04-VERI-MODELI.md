# 04 — Veri Modeli

EF Core entities ve ilişkileri. Şema oluşturma EF Migrations ile yapılır.

> Bu dosya **ürünün veri yapısının özeti**. Detaylı tablo şeması (kolon türü, boyut, index'ler) geliştirme aşamasında migration dosyalarında ortaya çıkacak.

---

## Çekirdek Entities

### Company (Şirket)
Holding yapısında birden fazla şirket olabilir.
- `Id`, `Name`, `TaxNo`, `LegalName`
- `BaseLocationId` (varsayılan lokasyon)
- `FiscalYearStartMonth` (genelde 1 — Ocak)
- `Currency` (TL, USD, EUR)
- `IsActive`

### Location (Lokasyon)
- `Id`, `Name`, `City`, `Country`
- `RegionalIndexPercent` (örn. 95 = İstanbul'un %95'i)
- `CompanyId`

### Department (Departman)
- `Id`, `Name`, `ParentDepartmentId` (hiyerarşi)
- `ManagerEmployeeId` (FK, departman müdürü)
- `CostCenterCode` (ERP entegrasyonu için)
- `CompanyId`, `IsActive`

### JobFamily (İş Ailesi)
- `Id`, `Name` (örn. "Mühendislik", "Satış")
- `Description`

### JobGrade (Kademe)
- `Id`, `Code` (örn. "P3", "M1"), `Name` (örn. "Senior Profesyonel")
- `CareerBand` (enum: Entry, Professional, Manager, Director, Executive)
- `EvaluationScoreMin`, `EvaluationScoreMax` (Hay/Mercer IPE puanları, opsiyonel)
- `OrderIndex` (sıralama için)

### Position (Pozisyon)
- `Id`, `Title` (örn. "Senior Yazılım Geliştirme Mühendisi")
- `JobGradeId` (FK, kademe)
- `JobFamilyId` (FK)
- `BenchmarkMatchName` (Mercer/WTW raporundaki standart isim)
- `Description`, `Responsibilities`
- `IsActive`

### SalaryBand (Maaş Bantı)
- `Id`, `JobGradeId`, `LocationId` (lokasyon bazlı bant olabilir)
- `Min`, `Mid`, `Max` (TL)
- `EffectiveDate` (geçerlilik başlangıcı — bant değişikliklerini izlemek için)
- `EndDate` (null = halen geçerli)
- **History tablosu**: SalaryBandHistory aynı kolonlar + ChangedAt + ChangedBy

### Employee (Çalışan)
Ana entity. Hassas alanlar **Always Encrypted**.
- `Id`, `EmployeeNumber` (sicil no, unique)
- `FirstName`, `LastName`
- `NationalId` (TC kimlik, ENCRYPTED)
- `BirthDate` (ENCRYPTED)
- `Gender` (enum: Male, Female, NotSpecified)
- `Email`, `Phone`
- `Address` (opsiyonel)
- `HireDate`, `TerminationDate` (null = aktif)
- `EmploymentType` (enum: Permanent, Fixed-term, Intern)
- `WorkSchedule` (enum: FullTime, PartTime)
- `PositionId`, `DepartmentId`, `LocationId` (FK)
- `ManagerId` (FK to Employee — line manager)
- `Status` (enum: Active, OnLeave, Terminated)
- `IsLocked` (zam donduğu çalışanlar için)
- `Notes`

### EmployeeSalary (Çalışan Ücret)
History desenli — her değişiklik yeni satır.
- `Id`, `EmployeeId`
- `GrossMonthly` (brüt aylık)
- `NetMonthly` (hesaplanan, cache)
- `EmployerCost` (hesaplanan, cache)
- `EffectiveDate` (yeni maaşın başlangıcı)
- `EndDate` (null = halen geçerli; sonraki satır geldiğinde dolu)
- `Reason` (enum: NewHire, AnnualMerit, Promotion, MarketAdjustment, Retention, Other)
- `ChangePercent`, `ChangeAmount` (gösterim için)
- `ScenarioId` (eğer senaryo kaynaklı ise FK)
- `ApprovalId` (FK to Approval, eğer onay süreci varsa)
- `CreatedBy`, `CreatedAt`

### EmployeePerformance (Performans)
- `Id`, `EmployeeId`, `Year`, `Cycle` (Q1/Q2/Annual)
- `Score` (1-5, ondalıklı)
- `CalibratedScore` (kalibrasyon sonrası)
- `Notes`

### EmployeeBenefit (Yan Haklar)
- `Id`, `EmployeeId`
- `BenefitType` (enum: HealthInsurance, PrivatePension, Lunch, Transport, Phone, Car, Education, Other)
- `MonthlyValue` (parasal değer)
- `Description`
- `EffectiveDate`, `EndDate`

---

## Benchmark / Market Data

### BenchmarkProvider (Veri Sağlayıcısı)
Müşterinin kullandığı raporlar (örn. "Mercer TRS 2026 — Üretim").
- `Id`, `Name`
- `SourceType` (enum: Mercer, WillisTowersWatson, KornFerry, KPMG, Aon, Manual, Other)

### BenchmarkData (Benchmark Verisi)
- `Id`, `BenchmarkProviderId`
- `BenchmarkPositionName` (örn. "Software Engineer III")
- `Sector`, `Region`, `CompanySize` (filtreleme için etiketler)
- `P25`, `P50`, `P75` (tutarlar)
- `Currency`
- `EffectiveDate` (verinin geçerlilik tarihi)
- `ImportedAt`, `ImportedBy`

### PositionBenchmarkMapping
Şirketin pozisyonu ↔ benchmark pozisyonu eşleştirmesi.
- `PositionId`, `BenchmarkPositionName`
- `MatchType` (enum: ExactMatch, Approximate, Manual)

---

## Senaryo ve Zam

### Scenario (Senaryo)
- `Id`, `Name`, `Description`
- `BaseDate` (snapshot tarihi)
- `Type` (enum: GeneralRaise, DepartmentRaise, PositionRaise, MeritMatrix, CustomTarget, MultiYear)
- `Parameters` (JSON — senaryo özel parametreleri)
- `Status` (enum: Draft, Calculated, Submitted, Approved, Rejected, Applied)
- `CreatedBy`, `CreatedAt`
- `AppliedBy`, `AppliedAt` (uygulandıysa)

### ScenarioEmployee (Senaryodaki Çalışan)
Senaryo başına çalışan başına satır.
- `Id`, `ScenarioId`, `EmployeeId`
- `OldGross`, `NewGross`
- `OldNet`, `NewNet` (cache)
- `OldEmployerCost`, `NewEmployerCost` (cache)
- `RaisePercent`, `RaiseAmount`
- `IsLocked`
- `IsManuallyOverridden` (kullanıcı manuel değiştirdi mi)
- `Notes`

### MeritMatrix
- `Id`, `ScenarioId`
- `Cells` (JSON — performance × range_position → percent)
- Örn. `{"high_q1": 15.0, "high_q2": 12.0, "high_q3": 10.0, "high_q4": 8.0, ...}`

---

## Onay Akışı

### ApprovalTemplate (Onay Şablonu)
- `Id`, `Name`
- `Steps` (JSON liste — sıralı: rol/kullanıcı, threshold)
- `TriggerType` (enum: IndividualRaise, BulkRaise, NewHire, OffCycle, ThresholdAbove)
- `IsActive`

### Approval (Onay)
- `Id`, `TemplateId`, `EntityType` (Scenario, EmployeeSalary, NewHire), `EntityId`
- `CurrentStep`, `Status` (enum: Pending, Approved, Rejected, Cancelled)
- `RequestedBy`, `RequestedAt`
- `Notes`

### ApprovalAction (Adım)
- `Id`, `ApprovalId`, `StepIndex`
- `ActorUserId`, `Decision` (enum: Approve, Reject, Return)
- `Comment`, `ActionAt`

---

## Vergi/SGK Parametreleri

### TaxParameter (Yıl Bazlı Sabitler)
- `Id`, `Year` (örn. 2026)
- `SgkEmployeePercent` (örn. 14.0)
- `UnemploymentEmployeePercent` (örn. 1.0)
- `SgkEmployerPercent` (örn. 20.75)
- `UnemploymentEmployerPercent` (örn. 2.0)
- `SgkEmployerDiscountPercent` (örn. 5.0)
- `ApplySgkDiscount` (bool)
- `StampTaxPercent` (örn. 0.759)

### IncomeTaxBracket (Gelir Vergisi Dilimi)
- `Id`, `Year`, `Order` (sıra)
- `IncomeTypeFlag` (enum: Wage, NonWage)  — ücret gelirleri için ayrı tarife
- `UpperLimit` (örn. 190000.00; son dilim için null = sonsuz)
- `Rate` (örn. 0.15)
- `FixedAmount` (örn. 28500 — bu dilime kadar olan vergi)

### MonthlyTaxPeriod (Aylık Dönem)
Yıl içinde değişen değerler (asgari ücret değişikliği vb.) için.
- `Id`, `Year`, `Month` (1-12)
- `SgkBaseMin` (asgari ücret brüt)
- `SgkBaseMax` (genelde min × 7.5)
- `MinWageNet`
- `MinWageGross`
- `GvExemptionAmount` (asgari ücret istisnası — aylık vergi matrahı kadar)
- `GvExemptionRate` (istisna hesabında kullanılacak oran — bracket'a göre)
- `StampExemptionAmount`

---

## Audit & Sistem

### AuditLog
- `Id`, `Timestamp`
- `UserId`, `Username`
- `EntityType`, `EntityId`
- `Action` (enum: Create, Update, Delete, View, Export, Login, LoginFailed)
- `OldValues` (JSON)
- `NewValues` (JSON)
- `IpAddress`, `UserAgent`
- **Append-only** — bu tabloya UPDATE/DELETE yasak

### User (Kullanıcı)
ASP.NET Identity AspNetUsers tablosunu genişletir.
- `Id` (Guid), `UserName`, `Email`, `PasswordHash`
- `EmployeeId` (FK, eğer kendisi de çalışansa)
- `IsActive`
- `LastLoginAt`, `FailedLoginCount`
- `LdapDn` (AD entegrasyonu)

### Role (Rol)
ASP.NET Identity AspNetRoles + custom kapsam.
- `Id`, `Name`, `Description`
- `IsSystemRole` (silinemez varsayılan roller)

### UserRole (Çoğa-Çok)
- `UserId`, `RoleId`
- `ScopeType` (enum: Global, Company, Department)
- `ScopeId` (departman ID'si vs.)

### Permission (İzin)
- `Id`, `Code` (örn. "Employee.Edit", "Salary.View")
- `Description`

### RolePermission
- `RoleId`, `PermissionId`

### License (Lisans)
- `Id`, `LicenseKey` (signed)
- `MaxEmployeeCount`, `MaxUserCount`
- `ExpiresAt`
- `IssuedTo` (müşteri firma adı)
- `MachineFingerprint` (kuruldu makineye bağlı)
- `IsActive`

### CompensationLetter (Ücret Bildirim Mektubu)
- `Id`, `EmployeeId`
- `IssuedAt`, `EffectiveDate`
- `OldGross`, `NewGross`, `RaisePercent`
- `Reason`, `Notes`
- `PdfBlob` (PDF binary, opsiyonel — ya da dosya yolunda saklı)
- `SignedByUserId`
- `SentToEmployee` (bool, e-posta gönderildi mi)

---

## Önemli İlişkiler (ER Özeti)

```
Company 1 ── N Department
Company 1 ── N Location
Department 1 ── N Position (departman pozisyonu yoktur, ortak havuz; ama bağlanabilir)
JobGrade 1 ── N Position
JobGrade 1 ── N SalaryBand (lokasyon başına ayrı band)
Position 1 ── 0..1 BenchmarkPositionMapping
Employee N ── 1 Position
Employee N ── 1 Department
Employee N ── 1 Location
Employee 1 ── N EmployeeSalary (history)
Employee 1 ── N EmployeePerformance
Employee 1 ── N EmployeeBenefit
Employee 1 ── 0..1 Manager (Employee)
Scenario 1 ── N ScenarioEmployee
Scenario 1 ── 0..1 MeritMatrix
Approval 1 ── N ApprovalAction
TaxParameter 1 ── N IncomeTaxBracket
TaxParameter 1 ── N MonthlyTaxPeriod
User N ── M Role
Role N ── M Permission
```

---

## İndeksleme Stratejisi (Performans)

- `Employee.EmployeeNumber` — UNIQUE
- `Employee.NationalId` — UNIQUE (encrypted ama unique kontrolü için deterministic encryption)
- `Employee.DepartmentId` — INDEX (departman müdürü filtresi için)
- `Employee.ManagerId` — INDEX (ekip listeleme)
- `EmployeeSalary.EmployeeId + EffectiveDate DESC` — INDEX (en son maaşı çekmek için)
- `AuditLog.Timestamp` — INDEX (rapor sorguları)
- `AuditLog.EntityType + EntityId` — INDEX (entity geçmişi)
- `ScenarioEmployee.ScenarioId` — INDEX

---

## Soft Delete

Aşağıdaki entity'ler **soft delete** (silme yerine `IsDeleted=true`):
- Employee (KVKK silme talebi farklı akışta — anonimleştirme)
- Scenario
- BenchmarkData
- ApprovalTemplate

Diğerleri hard delete.

---

## Multi-Tenancy (Holding Senaryosu)

İlk versiyon **single-tenant** — bir kurulum bir müşteri/holding.

Holding senaryosunda:
- `Company` entity'si birden fazla satır
- Kullanıcı bir veya birden fazla şirkete erişim
- Tüm sorgular `CompanyId` filtresi ekler (EF global query filter)

Multi-tenant SaaS değil — her müşterinin kendi DB'si var.
