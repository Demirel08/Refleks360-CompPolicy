namespace Refleks360.Application.Abstractions;

/// <summary>
/// O anki kullanıcının seçili şirketini sağlar. Multi-company (holding) modunda
/// query'ler bu Id'ye göre filtrelenebilir; tek şirketli kurulumda ise daima 1 döner.
/// </summary>
public interface ICurrentCompanyContext
{
    int CurrentCompanyId { get; }
    Task SetCurrentCompanyAsync(int companyId, CancellationToken ct = default);
    Task<IReadOnlyList<CompanyOption>> GetAccessibleCompaniesAsync(CancellationToken ct = default);
}

public sealed record CompanyOption(int Id, string Name);
