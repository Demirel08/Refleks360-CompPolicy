using Refleks360.Application.TaxAdmin;

namespace Refleks360.Application.Abstractions;

public interface ITaxAdminService
{
    Task<IReadOnlyList<int>> GetYearsAsync(CancellationToken ct = default);
    Task<TaxYearDetail?> GetAsync(int year, CancellationToken ct = default);
    Task SaveYearAsync(TaxYearAdminDto y, CancellationToken ct = default);
    Task SaveBracketsAsync(int year, IReadOnlyList<TaxBracketAdminDto> brackets, CancellationToken ct = default);
    Task SavePeriodsAsync(int year, IReadOnlyList<MonthlyPeriodAdminDto> periods, CancellationToken ct = default);
    Task<int> CloneFromAsync(int sourceYear, int targetYear, CancellationToken ct = default);
}
