using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Organization;
using Refleks360.Domain.Organization;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Persistence.Entities;

namespace Refleks360.Infrastructure.Services;

public sealed class OrganizationAdminService(CompDbContext db) : IOrganizationAdminService
{
    private const int DefaultCompanyId = 1;

    // ---------- Departments ----------
    public async Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync(CancellationToken ct = default)
    {
        var counts = await db.Employees.GroupBy(e => e.DepartmentId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, ct);

        return await db.Departments.AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentDto(
                d.Id, d.Name,
                d.ParentDepartmentId, d.ParentDepartment != null ? d.ParentDepartment.Name : null,
                d.ManagerEmployeeId,
                d.ManagerEmployee != null ? d.ManagerEmployee.FirstName + " " + d.ManagerEmployee.LastName : null,
                d.CostCenterCode, d.IsActive,
                0))
            .ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<DepartmentDto>)t.Result
                .Select(d => d with { EmployeeCount = counts.TryGetValue(d.Id, out var c) ? c : 0 })
                .ToList(), ct);
    }

    public async Task<int> UpsertDepartmentAsync(int? id, string name, int? parentId, int? managerEmployeeId, string? costCenter, bool isActive, CancellationToken ct = default)
    {
        DepartmentEntity entity;
        if (id is null)
        {
            entity = new DepartmentEntity { CompanyId = DefaultCompanyId };
            db.Departments.Add(entity);
        }
        else
        {
            entity = await db.Departments.FirstAsync(d => d.Id == id, ct);
        }
        entity.Name = name;
        entity.ParentDepartmentId = parentId == 0 ? null : parentId;
        entity.ManagerEmployeeId = managerEmployeeId == 0 ? null : managerEmployeeId;
        entity.CostCenterCode = string.IsNullOrWhiteSpace(costCenter) ? null : costCenter;
        entity.IsActive = isActive;
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task DeleteDepartmentAsync(int id, CancellationToken ct = default)
    {
        var anyEmployee = await db.Employees.AnyAsync(e => e.DepartmentId == id, ct);
        if (anyEmployee)
            throw new InvalidOperationException("Bu departmana bağlı çalışanlar var; silinemez. Önce taşı.");
        var entity = await db.Departments.FirstAsync(d => d.Id == id, ct);
        db.Departments.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    // ---------- Positions ----------
    public async Task<IReadOnlyList<PositionDto>> GetPositionsAsync(CancellationToken ct = default)
    {
        var counts = await db.Employees.GroupBy(e => e.PositionId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, ct);

        var list = await db.Positions.AsNoTracking()
            .OrderBy(p => p.Title)
            .Select(p => new PositionDto(
                p.Id, p.Title,
                p.JobGradeId, p.JobGrade.Code,
                p.JobFamilyId, p.JobFamily.Name,
                p.BenchmarkMatchName, p.IsActive,
                0))
            .ToListAsync(ct);

        return list.Select(p => p with { EmployeeCount = counts.TryGetValue(p.Id, out var c) ? c : 0 }).ToList();
    }

    public async Task<int> UpsertPositionAsync(int? id, string title, int jobGradeId, int jobFamilyId, string? benchmark, bool isActive, CancellationToken ct = default)
    {
        PositionEntity entity;
        if (id is null) { entity = new PositionEntity(); db.Positions.Add(entity); }
        else entity = await db.Positions.FirstAsync(p => p.Id == id, ct);

        entity.Title = title;
        entity.JobGradeId = jobGradeId;
        entity.JobFamilyId = jobFamilyId;
        entity.BenchmarkMatchName = string.IsNullOrWhiteSpace(benchmark) ? null : benchmark;
        entity.IsActive = isActive;
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task DeletePositionAsync(int id, CancellationToken ct = default)
    {
        var anyEmployee = await db.Employees.AnyAsync(e => e.PositionId == id, ct);
        if (anyEmployee)
            throw new InvalidOperationException("Bu pozisyona bağlı çalışanlar var; silinemez.");
        var entity = await db.Positions.FirstAsync(p => p.Id == id, ct);
        db.Positions.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    // ---------- Locations ----------
    public async Task<IReadOnlyList<LocationDto>> GetLocationsAsync(CancellationToken ct = default)
    {
        var counts = await db.Employees.GroupBy(e => e.LocationId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, ct);

        var list = await db.Locations.AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => new LocationDto(l.Id, l.Name, l.City, l.Country, l.RegionalIndexPercent, l.IsActive, 0))
            .ToListAsync(ct);

        return list.Select(l => l with { EmployeeCount = counts.TryGetValue(l.Id, out var c) ? c : 0 }).ToList();
    }

    public async Task<int> UpsertLocationAsync(int? id, string name, string city, string country, decimal regionalIndex, bool isActive, CancellationToken ct = default)
    {
        LocationEntity entity;
        if (id is null) { entity = new LocationEntity { CompanyId = DefaultCompanyId }; db.Locations.Add(entity); }
        else entity = await db.Locations.FirstAsync(l => l.Id == id, ct);

        entity.Name = name;
        entity.City = city;
        entity.Country = string.IsNullOrWhiteSpace(country) ? "Türkiye" : country;
        entity.RegionalIndexPercent = regionalIndex;
        entity.IsActive = isActive;
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task DeleteLocationAsync(int id, CancellationToken ct = default)
    {
        if (await db.Employees.AnyAsync(e => e.LocationId == id, ct))
            throw new InvalidOperationException("Bu lokasyona bağlı çalışanlar var; silinemez.");
        var entity = await db.Locations.FirstAsync(l => l.Id == id, ct);
        db.Locations.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    // ---------- Job grades ----------
    public async Task<IReadOnlyList<JobGradeDto>> GetJobGradesAsync(CancellationToken ct = default) =>
        await db.JobGrades.AsNoTracking()
            .OrderBy(g => g.OrderIndex)
            .Select(g => new JobGradeDto(g.Id, g.Code, g.Name, g.CareerBand, g.OrderIndex, g.EvaluationScoreMin, g.EvaluationScoreMax))
            .ToListAsync(ct);

    public async Task<int> UpsertJobGradeAsync(int? id, string code, string name, CareerBand band, int order, int? scoreMin, int? scoreMax, CancellationToken ct = default)
    {
        JobGradeEntity entity;
        if (id is null) { entity = new JobGradeEntity(); db.JobGrades.Add(entity); }
        else entity = await db.JobGrades.FirstAsync(g => g.Id == id, ct);

        entity.Code = code;
        entity.Name = name;
        entity.CareerBand = band;
        entity.OrderIndex = order;
        entity.EvaluationScoreMin = scoreMin;
        entity.EvaluationScoreMax = scoreMax;
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task DeleteJobGradeAsync(int id, CancellationToken ct = default)
    {
        if (await db.Positions.AnyAsync(p => p.JobGradeId == id, ct))
            throw new InvalidOperationException("Bu kademe pozisyon(lar)da kullanılıyor; silinemez.");
        var entity = await db.JobGrades.FirstAsync(g => g.Id == id, ct);
        db.JobGrades.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    // ---------- Job families ----------
    public async Task<IReadOnlyList<JobFamilyDto>> GetJobFamiliesAsync(CancellationToken ct = default) =>
        await db.JobFamilies.AsNoTracking()
            .OrderBy(f => f.Name)
            .Select(f => new JobFamilyDto(f.Id, f.Name, f.Description))
            .ToListAsync(ct);

    public async Task<int> UpsertJobFamilyAsync(int? id, string name, string? description, CancellationToken ct = default)
    {
        JobFamilyEntity entity;
        if (id is null) { entity = new JobFamilyEntity(); db.JobFamilies.Add(entity); }
        else entity = await db.JobFamilies.FirstAsync(f => f.Id == id, ct);

        entity.Name = name;
        entity.Description = string.IsNullOrWhiteSpace(description) ? null : description;
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task DeleteJobFamilyAsync(int id, CancellationToken ct = default)
    {
        if (await db.Positions.AnyAsync(p => p.JobFamilyId == id, ct))
            throw new InvalidOperationException("Bu iş ailesi pozisyon(lar)da kullanılıyor; silinemez.");
        var entity = await db.JobFamilies.FirstAsync(f => f.Id == id, ct);
        db.JobFamilies.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}
