namespace Refleks360.Domain.Security;

/// <summary>
/// Sistem rolleri sabitleri. ASP.NET Identity AspNetRoles tablosuna seed edilir.
/// </summary>
public static class Roles
{
    public const string SystemAdmin = "SystemAdmin";
    public const string HRDirector = "HRDirector";
    public const string HRManager = "HRManager";
    public const string HRSpecialist = "HRSpecialist";
    public const string DepartmentManager = "DepartmentManager";
    public const string CFO = "CFO";
    public const string CEO = "CEO";
    public const string Auditor = "Auditor";

    public static readonly IReadOnlyList<string> All = new[]
    {
        SystemAdmin, HRDirector, HRManager, HRSpecialist,
        DepartmentManager, CFO, CEO, Auditor
    };
}
