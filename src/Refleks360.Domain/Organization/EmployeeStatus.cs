namespace Refleks360.Domain.Organization;

/// <summary>Çalışan durumu.</summary>
public enum EmployeeStatus
{
    /// <summary>Aktif çalışan.</summary>
    Active = 0,
    /// <summary>İzinli (askerlik, doğum, sağlık vb.).</summary>
    OnLeave = 1,
    /// <summary>İşten ayrılmış.</summary>
    Terminated = 2,
}
