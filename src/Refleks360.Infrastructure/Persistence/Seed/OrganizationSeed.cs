using Microsoft.EntityFrameworkCore;
using Refleks360.Domain.Organization;
using Refleks360.Infrastructure.Persistence.Entities;

namespace Refleks360.Infrastructure.Persistence.Seed;

/// <summary>
/// Demo organizasyon verisi: 1 şirket + 3 lokasyon + 5 departman + 4 iş ailesi +
/// 6 kademe + 10 pozisyon + 30 çalışan. Hafta 5 çıktısının "100 örnek" hedefinin
/// indirimli versiyonu (ilk listede yeterli görsel çeşitlilik).
/// </summary>
internal static class OrganizationSeed
{
    private const int CompanyId = 1;

    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompanyEntity>().HasData(new CompanyEntity
        {
            Id = CompanyId,
            Name = "Refleks Demo A.Ş.",
            LegalName = "Refleks Demo Sanayi ve Ticaret Anonim Şirketi",
            TaxNo = "0000000000",
            FiscalYearStartMonth = 1,
            Currency = "TL",
            IsActive = true,
        });

        modelBuilder.Entity<LocationEntity>().HasData(
            new LocationEntity { Id = 1, CompanyId = CompanyId, Name = "İstanbul Genel Müdürlük", City = "İstanbul", Country = "Türkiye", RegionalIndexPercent = 100m, IsActive = true },
            new LocationEntity { Id = 2, CompanyId = CompanyId, Name = "Ankara Bölge",           City = "Ankara",   Country = "Türkiye", RegionalIndexPercent = 95m,  IsActive = true },
            new LocationEntity { Id = 3, CompanyId = CompanyId, Name = "İzmir Fabrika",          City = "İzmir",    Country = "Türkiye", RegionalIndexPercent = 90m,  IsActive = true });

        modelBuilder.Entity<DepartmentEntity>().HasData(
            new DepartmentEntity { Id = 1, CompanyId = CompanyId, Name = "Bilgi Teknolojileri", CostCenterCode = "BT-100",  IsActive = true },
            new DepartmentEntity { Id = 2, CompanyId = CompanyId, Name = "İnsan Kaynakları",    CostCenterCode = "IK-200",  IsActive = true },
            new DepartmentEntity { Id = 3, CompanyId = CompanyId, Name = "Finans",              CostCenterCode = "FI-300",  IsActive = true },
            new DepartmentEntity { Id = 4, CompanyId = CompanyId, Name = "Satış ve Pazarlama",  CostCenterCode = "SAT-400", IsActive = true },
            new DepartmentEntity { Id = 5, CompanyId = CompanyId, Name = "Üretim",              CostCenterCode = "URT-500", IsActive = true });

        modelBuilder.Entity<JobFamilyEntity>().HasData(
            new JobFamilyEntity { Id = 1, Name = "Mühendislik",        Description = "Yazılım, donanım ve sistem mühendisliği" },
            new JobFamilyEntity { Id = 2, Name = "Operasyon",          Description = "Üretim, üretim yönetimi, kalite" },
            new JobFamilyEntity { Id = 3, Name = "Destek Fonksiyonlar", Description = "IK, finans, satın alma, hukuk" },
            new JobFamilyEntity { Id = 4, Name = "Ticari",             Description = "Satış, pazarlama, müşteri ilişkileri" });

        modelBuilder.Entity<JobGradeEntity>().HasData(
            new JobGradeEntity { Id = 1, Code = "P1", Name = "Uzman Yardımcısı", CareerBand = CareerBand.Entry,        OrderIndex = 1 },
            new JobGradeEntity { Id = 2, Code = "P2", Name = "Uzman",            CareerBand = CareerBand.Professional, OrderIndex = 2 },
            new JobGradeEntity { Id = 3, Code = "P3", Name = "Kıdemli Uzman",    CareerBand = CareerBand.Professional, OrderIndex = 3 },
            new JobGradeEntity { Id = 4, Code = "M1", Name = "Takım Lideri",     CareerBand = CareerBand.Manager,      OrderIndex = 4 },
            new JobGradeEntity { Id = 5, Code = "M2", Name = "Müdür",            CareerBand = CareerBand.Manager,      OrderIndex = 5 },
            new JobGradeEntity { Id = 6, Code = "D1", Name = "Direktör",         CareerBand = CareerBand.Director,     OrderIndex = 6 });

        modelBuilder.Entity<PositionEntity>().HasData(
            new PositionEntity { Id =  1, Title = "Yazılım Geliştirme Mühendisi (Junior)", JobGradeId = 1, JobFamilyId = 1, IsActive = true },
            new PositionEntity { Id =  2, Title = "Yazılım Geliştirme Mühendisi",          JobGradeId = 2, JobFamilyId = 1, IsActive = true },
            new PositionEntity { Id =  3, Title = "Kıdemli Yazılım Geliştirme Mühendisi",  JobGradeId = 3, JobFamilyId = 1, IsActive = true },
            new PositionEntity { Id =  4, Title = "Yazılım Takım Lideri",                  JobGradeId = 4, JobFamilyId = 1, IsActive = true },
            new PositionEntity { Id =  5, Title = "İK Uzmanı",                             JobGradeId = 2, JobFamilyId = 3, IsActive = true },
            new PositionEntity { Id =  6, Title = "İK Müdürü",                             JobGradeId = 5, JobFamilyId = 3, IsActive = true },
            new PositionEntity { Id =  7, Title = "Mali Müşavir",                          JobGradeId = 3, JobFamilyId = 3, IsActive = true },
            new PositionEntity { Id =  8, Title = "Satış Temsilcisi",                      JobGradeId = 2, JobFamilyId = 4, IsActive = true },
            new PositionEntity { Id =  9, Title = "Üretim Operatörü",                      JobGradeId = 1, JobFamilyId = 2, IsActive = true },
            new PositionEntity { Id = 10, Title = "Üretim Mühendisi",                      JobGradeId = 3, JobFamilyId = 2, IsActive = true });

        modelBuilder.Entity<EmployeeEntity>().HasData(BuildEmployees());
    }

    private static EmployeeEntity[] BuildEmployees()
    {
        // Sabit veri — hash'i değişmesin diye sıralı atama.
        var people = new (string First, string Last, Gender G)[]
        {
            ("Ahmet",   "Yılmaz",     Gender.Male),
            ("Mehmet",  "Demir",      Gender.Male),
            ("Mustafa", "Kara",       Gender.Male),
            ("Ali",     "Çelik",      Gender.Male),
            ("Hasan",   "Yıldız",     Gender.Male),
            ("Hüseyin", "Aydın",      Gender.Male),
            ("İbrahim", "Öztürk",     Gender.Male),
            ("Murat",   "Aslan",      Gender.Male),
            ("Emre",    "Doğan",      Gender.Male),
            ("Burak",   "Şahin",      Gender.Male),
            ("Can",     "Polat",      Gender.Male),
            ("Onur",    "Erdoğan",    Gender.Male),
            ("Serkan",  "Korkmaz",    Gender.Male),
            ("Selim",   "Kurt",       Gender.Male),
            ("Tolga",   "Acar",       Gender.Male),
            ("Ayşe",    "Kaya",       Gender.Female),
            ("Fatma",   "Şimşek",     Gender.Female),
            ("Zeynep",  "Arslan",     Gender.Female),
            ("Elif",    "Koç",        Gender.Female),
            ("Merve",   "Özdemir",    Gender.Female),
            ("Esra",    "Çetin",      Gender.Female),
            ("Gül",     "Yavuz",      Gender.Female),
            ("Sevgi",   "Yıldırım",   Gender.Female),
            ("Deniz",   "Bulut",      Gender.Female),
            ("Pınar",   "Güneş",      Gender.Female),
            ("Burcu",   "Erdem",      Gender.Female),
            ("Selin",   "Aktaş",      Gender.Female),
            ("Cansu",   "Sarı",       Gender.Female),
            ("Damla",   "Karaca",     Gender.Female),
            ("Tuğçe",   "Tekin",      Gender.Female),
        };

        // Her çalışana sabit (Id, Position, Department, Location, Manager) dağılımı.
        // Pattern: ilk 12 BT (dept 1), 4 IK (dept 2), 4 Finans (dept 3), 5 Satış (dept 4), 5 Üretim (dept 5)
        var dept = new int[]  { 1,1,1,1,1,1,1,1,1,1,1,1, 2,2,2,2, 3,3,3,3, 4,4,4,4,4, 5,5,5,5,5 };
        var pos  = new int[]  { 2,2,2,3,3,3,1,1,1,4,4,4, 5,5,5,6, 7,7,7,3, 8,8,8,8,8, 9,9,9,9,10 };
        var loc  = new int[]  { 1,1,1,1,1,1,2,2,2,1,1,1, 1,1,2,1, 1,1,1,1, 1,2,2,3,3, 3,3,3,3,3 };
        // Self-referencing FK seed'i tek transaction'da problemli — manager ilişkilerini
        // boş bırakıyoruz; UI üzerinden veya ikinci bir migration ile sonradan atanır.

        var baseHire = new DateOnly(2020, 1, 1);
        var employees = new EmployeeEntity[people.Length];

        for (int i = 0; i < people.Length; i++)
        {
            var (first, last, g) = people[i];
            employees[i] = new EmployeeEntity
            {
                Id = i + 1,
                EmployeeNumber = (1000 + i + 1).ToString(),
                FirstName = first,
                LastName = last,
                Gender = g,
                EmploymentType = EmploymentType.Permanent,
                WorkSchedule = WorkSchedule.FullTime,
                Status = EmployeeStatus.Active,
                HireDate = baseHire.AddDays(i * 47), // çeşitli işe alım tarihleri
                PositionId = pos[i],
                DepartmentId = dept[i],
                LocationId = loc[i],
                ManagerId = null,
                Email = $"{Slug(first)}.{Slug(last)}@refleks360.local",
                IsLocked = false,
                IsDeleted = false,
            };
        }

        return employees;
    }

    private static string Slug(string s) =>
        new string(s.ToLowerInvariant()
            .Replace("ı", "i").Replace("ş", "s").Replace("ç", "c")
            .Replace("ğ", "g").Replace("ü", "u").Replace("ö", "o")
            .Where(c => char.IsLetterOrDigit(c))
            .ToArray());
}
