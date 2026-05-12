using Refleks360.Application.MultiYear;

namespace Refleks360.Application.Abstractions;

public interface IMultiYearPlanService
{
    Task<MultiYearPlan> ProjectAsync(int years, decimal annualRaisePercent, decimal annualInflationPercent, CancellationToken ct = default);
    Task<DepartmentGradeHeatMap> ComputeDeptGradeHeatMapAsync(CancellationToken ct = default);
}
