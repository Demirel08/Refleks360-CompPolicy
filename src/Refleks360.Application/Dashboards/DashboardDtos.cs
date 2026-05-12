namespace Refleks360.Application.Dashboards;

public sealed record DepartmentCostSlice(string Department, decimal MonthlyEmployerCost, int Headcount);

public sealed record GenderPayGap(decimal AvgMaleGross, decimal AvgFemaleGross, decimal RawGapPercent);

public sealed record DashboardSummary(
    int TotalEmployees,
    int ActiveEmployees,
    decimal MonthlyEmployerCostTotal,
    decimal AvgCompaRatio,
    int PendingScenarios,
    GenderPayGap PayGap,
    IReadOnlyList<DepartmentCostSlice> CostByDepartment);
