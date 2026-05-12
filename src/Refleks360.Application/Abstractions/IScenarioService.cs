using Refleks360.Application.Scenarios;

namespace Refleks360.Application.Abstractions;

public interface IScenarioService
{
    Task<IReadOnlyList<ScenarioListItem>> GetAllAsync(CancellationToken ct = default);
    Task<ScenarioDetail?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(CreateScenarioInput input, string createdBy, CancellationToken ct = default);
    Task ApplyAsync(int scenarioId, string appliedBy, CancellationToken ct = default);
    Task CancelAsync(int scenarioId, CancellationToken ct = default);
    Task DeleteAsync(int scenarioId, CancellationToken ct = default);
}
