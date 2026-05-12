namespace Refleks360.Domain.Security;

/// <summary>
/// İzin kodları + rol → izin haritası. Permission claim'leri olarak kullanıcıya atanır.
/// Claim type: <see cref="ClaimType"/> ("permission"), value: izin kodu.
/// </summary>
public static class Permissions
{
    public const string ClaimType = "permission";

    // Çalışan yönetimi
    public const string EmployeeView = "Employee.View";
    public const string EmployeeViewSalary = "Employee.ViewSalary";
    public const string EmployeeEdit = "Employee.Edit";
    public const string EmployeeDelete = "Employee.Delete";
    public const string EmployeeImport = "Employee.Import";

    // Organizasyon (departman/pozisyon/kademe/lokasyon)
    public const string OrgView = "Org.View";
    public const string OrgEdit = "Org.Edit";

    // Maaş bantları
    public const string BandView = "Band.View";
    public const string BandEdit = "Band.Edit";

    // Senaryolar
    public const string ScenarioView = "Scenario.View";
    public const string ScenarioCreate = "Scenario.Create";
    public const string ScenarioApply = "Scenario.Apply";

    // Raporlar
    public const string ReportView = "Report.View";
    public const string ReportExport = "Report.Export";

    // Vergi parametreleri
    public const string TaxParameterView = "TaxParameter.View";
    public const string TaxParameterEdit = "TaxParameter.Edit";

    // Sistem
    public const string UserManage = "User.Manage";
    public const string AuditView = "Audit.View";
    public const string Settings = "Settings.Manage";

    public static readonly IReadOnlyList<string> All = new[]
    {
        EmployeeView, EmployeeViewSalary, EmployeeEdit, EmployeeDelete, EmployeeImport,
        OrgView, OrgEdit,
        BandView, BandEdit,
        ScenarioView, ScenarioCreate, ScenarioApply,
        ReportView, ReportExport,
        TaxParameterView, TaxParameterEdit,
        UserManage, AuditView, Settings,
    };

    /// <summary>
    /// Rol → varsayılan izinler. SystemAdmin tüm izinlere sahiptir (özel kontrol).
    /// </summary>
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> DefaultRolePermissions = new Dictionary<string, IReadOnlyList<string>>
    {
        [Roles.SystemAdmin] = All,
        [Roles.HRDirector] = new[]
        {
            EmployeeView, EmployeeViewSalary, EmployeeEdit, EmployeeDelete, EmployeeImport,
            OrgView, OrgEdit,
            BandView, BandEdit,
            ScenarioView, ScenarioCreate, ScenarioApply,
            ReportView, ReportExport,
            TaxParameterView,
            AuditView,
        },
        [Roles.HRManager] = new[]
        {
            EmployeeView, EmployeeViewSalary, EmployeeEdit, EmployeeImport,
            OrgView, OrgEdit,
            BandView,
            ScenarioView, ScenarioCreate,
            ReportView, ReportExport,
            TaxParameterView,
        },
        [Roles.HRSpecialist] = new[]
        {
            EmployeeView, EmployeeEdit, EmployeeImport,
            OrgView,
            ScenarioView,
            ReportView,
        },
        [Roles.DepartmentManager] = new[]
        {
            EmployeeView, EmployeeViewSalary,
            OrgView,
            ScenarioView,
            ReportView,
        },
        [Roles.CFO] = new[]
        {
            EmployeeView, EmployeeViewSalary,
            OrgView,
            BandView,
            ScenarioView, ScenarioApply,
            ReportView, ReportExport,
            TaxParameterView,
            AuditView,
        },
        [Roles.CEO] = new[]
        {
            EmployeeView, EmployeeViewSalary,
            OrgView,
            BandView,
            ScenarioView, ScenarioApply,
            ReportView, ReportExport,
            AuditView,
        },
        [Roles.Auditor] = new[]
        {
            EmployeeView,
            OrgView,
            BandView,
            ScenarioView,
            ReportView, ReportExport,
            TaxParameterView,
            AuditView,
        },
    };
}
