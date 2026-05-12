using Refleks360.Domain.Organization;

namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>Kademe (P1, P2 ... M1, M2 ...). Bant ve hedef maaş bunlara bağlanır.</summary>
public sealed class JobGradeEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CareerBand CareerBand { get; set; }
    public int OrderIndex { get; set; }
    public int? EvaluationScoreMin { get; set; }
    public int? EvaluationScoreMax { get; set; }
}
