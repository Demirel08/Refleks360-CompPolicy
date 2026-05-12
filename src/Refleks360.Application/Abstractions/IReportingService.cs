namespace Refleks360.Application.Abstractions;

public interface IReportingService
{
    Task<byte[]> ExportEmployeeListXlsxAsync(CancellationToken ct = default);
    Task<byte[]> ExportCompPolicyXlsxAsync(CancellationToken ct = default);
    Task<byte[]> ExportScenarioPdfAsync(int scenarioId, CancellationToken ct = default);
    Task<byte[]> ExportMonthlyPayrollPdfAsync(int year, int month, CancellationToken ct = default);
}
