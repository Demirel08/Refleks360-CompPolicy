namespace Refleks360.Application.MultiYear;

public sealed record YearProjection(int Year, decimal MonthlyEmployerCost, decimal AnnualEmployerCost, decimal AnnualPayroll);

public sealed record MultiYearPlan(
    decimal CurrentAnnualPayroll,
    IReadOnlyList<YearProjection> Years,
    decimal AssumedInflationPercent,
    decimal AssumedRaisePercent,
    int Headcount);

public sealed record DepartmentGradeHeatCell(string Department, string GradeCode, int Headcount, decimal? AvgCompaRatio);

public sealed record DepartmentGradeHeatMap(
    IReadOnlyList<string> Grades,
    IReadOnlyList<string> Departments,
    IReadOnlyList<DepartmentGradeHeatCell> Cells);
