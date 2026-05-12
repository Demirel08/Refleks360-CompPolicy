namespace Refleks360.Infrastructure.Persistence.Entities;

/// <summary>İş ailesi (Mühendislik, Satış, Finans, vb.).</summary>
public sealed class JobFamilyEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
