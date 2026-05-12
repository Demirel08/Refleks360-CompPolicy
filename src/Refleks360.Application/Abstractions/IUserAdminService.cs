using Refleks360.Application.Users;

namespace Refleks360.Application.Abstractions;

/// <summary>Kullanıcı yönetimi (sadece SystemAdmin/UserManage izni).</summary>
public interface IUserAdminService
{
    Task<IReadOnlyList<UserListItem>> GetAllAsync(CancellationToken ct = default);
    Task<bool> SetActiveAsync(string userId, bool isActive, CancellationToken ct = default);
    Task<bool> SetRolesAsync(string userId, IReadOnlyList<string> roleNames, CancellationToken ct = default);
    Task<(bool ok, string? error, string? createdUserId)> CreateAsync(string userName, string email, string fullName, string password, IReadOnlyList<string> roles, CancellationToken ct = default);
    Task<bool> ResetPasswordAsync(string userId, string newPassword, CancellationToken ct = default);
}
