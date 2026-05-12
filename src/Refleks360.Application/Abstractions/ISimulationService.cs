using Refleks360.Application.Simulation;

namespace Refleks360.Application.Abstractions;

public interface ISimulationService
{
    Task<SimulationResult> SimulateAsync(
        IReadOnlyList<NewHireInput> hires,
        IReadOnlyList<DepartureInput> departures,
        CancellationToken ct = default);
}
