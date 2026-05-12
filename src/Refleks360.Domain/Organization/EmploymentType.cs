namespace Refleks360.Domain.Organization;

/// <summary>İş sözleşmesi tipi.</summary>
public enum EmploymentType
{
    /// <summary>Süresiz (kadrolu).</summary>
    Permanent = 0,
    /// <summary>Belirli süreli.</summary>
    FixedTerm = 1,
    /// <summary>Stajyer.</summary>
    Intern = 2,
}
