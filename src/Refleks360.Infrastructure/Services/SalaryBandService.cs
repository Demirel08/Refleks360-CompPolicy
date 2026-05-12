using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Organization;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Persistence.Entities;

namespace Refleks360.Infrastructure.Services;

public sealed class SalaryBandService(CompDbContext db) : ISalaryBandService
{
    public async Task<IReadOnlyList<SalaryBandDto>> GetAllAsync(CancellationToken ct = default) =>
        await db.SalaryBands.AsNoTracking()
            .OrderBy(b => b.JobGrade.OrderIndex).ThenBy(b => b.LocationId)
            .Select(b => new SalaryBandDto(
                b.Id, b.JobGradeId, b.JobGrade.Code,
                b.LocationId, b.Location != null ? b.Location.Name : null,
                b.Min, b.Mid, b.Max, b.EffectiveDate, b.EndDate))
            .ToListAsync(ct);

    public async Task<int> UpsertAsync(int? id, int jobGradeId, int? locationId, decimal min, decimal mid, decimal max, DateOnly effectiveDate, DateOnly? endDate, CancellationToken ct = default)
    {
        if (!(min <= mid && mid <= max && min > 0))
            throw new InvalidOperationException("Bant değerleri 0 < Min ≤ Mid ≤ Max olmalı.");

        SalaryBandEntity entity;
        if (id is null) { entity = new SalaryBandEntity(); db.SalaryBands.Add(entity); }
        else entity = await db.SalaryBands.FirstAsync(b => b.Id == id, ct);

        entity.JobGradeId = jobGradeId;
        entity.LocationId = locationId == 0 ? null : locationId;
        entity.Min = min;
        entity.Mid = mid;
        entity.Max = max;
        entity.EffectiveDate = effectiveDate;
        entity.EndDate = endDate;
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await db.SalaryBands.FirstAsync(b => b.Id == id, ct);
        db.SalaryBands.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task<int> AutoGenerateAsync(int firstGradeId, decimal firstMin, decimal firstMid, decimal firstMax, decimal midProgressionPercent, DateOnly effectiveDate, CancellationToken ct = default)
    {
        var grades = await db.JobGrades.AsNoTracking().OrderBy(g => g.OrderIndex).ToListAsync(ct);
        var firstIdx = grades.FindIndex(g => g.Id == firstGradeId);
        if (firstIdx < 0) throw new InvalidOperationException("İlk kademe bulunamadı.");

        var rangeSpread = (firstMax - firstMin) / firstMin; // örn 0.50 = %50
        var midFactor = 1m + midProgressionPercent / 100m;  // örn 1.15

        // İlk + sonraki tüm kademeleri üret
        int generated = 0;
        decimal curMid = firstMid;

        for (int i = firstIdx; i < grades.Count; i++)
        {
            var g = grades[i];
            decimal mid = (i == firstIdx) ? firstMid : Math.Round(curMid * midFactor, 0);
            curMid = mid;
            decimal min = Math.Round(mid / (1m + rangeSpread / 2m), 0);
            decimal max = Math.Round(min * (1m + rangeSpread), 0);

            // Eski lokasyon-bağımsız geçerli bandı bitir
            var existing = await db.SalaryBands.Where(b => b.JobGradeId == g.Id && b.LocationId == null && b.EndDate == null).FirstOrDefaultAsync(ct);
            if (existing is not null)
            {
                existing.EndDate = effectiveDate.AddDays(-1);
            }

            db.SalaryBands.Add(new SalaryBandEntity
            {
                JobGradeId = g.Id,
                LocationId = null,
                Min = min, Mid = mid, Max = max,
                EffectiveDate = effectiveDate,
            });
            generated++;
        }

        await db.SaveChangesAsync(ct);
        return generated;
    }
}
