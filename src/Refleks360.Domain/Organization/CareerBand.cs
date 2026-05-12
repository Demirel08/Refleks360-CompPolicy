namespace Refleks360.Domain.Organization;

/// <summary>Kademe (JobGrade) kariyer bandı.</summary>
public enum CareerBand
{
    /// <summary>Giriş seviyesi.</summary>
    Entry = 0,
    /// <summary>Profesyonel (uzman, ekiplerde yardımcı/üye).</summary>
    Professional = 1,
    /// <summary>Yönetici / lead.</summary>
    Manager = 2,
    /// <summary>Direktör.</summary>
    Director = 3,
    /// <summary>Üst düzey yönetici (C-level, VP).</summary>
    Executive = 4,
}
