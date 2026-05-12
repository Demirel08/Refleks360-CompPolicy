namespace Refleks360.Application.Simulation;

public sealed class NewHireInput
{
    public string Label { get; set; } = "Yeni İşe Alım";
    public int PositionId { get; set; }
    public int DepartmentId { get; set; }
    public int LocationId { get; set; }
    public decimal MonthlyGross { get; set; }
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}

public sealed class DepartureInput
{
    public int EmployeeId { get; set; }
    public DateOnly TerminationDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}

public sealed record SimulationLine(
    string Label,
    string Kind, // "hire" | "leave"
    decimal MonthlyDelta,
    decimal AnnualDelta,
    decimal SeveranceLiability);

public sealed record SimulationResult(
    decimal TotalMonthlyDelta,
    decimal TotalAnnualDelta,
    decimal TotalSeveranceLiability,
    IReadOnlyList<SimulationLine> Lines);
