namespace Refleks360.Application.Users;

public sealed record UserListItem(
    string Id,
    string UserName,
    string Email,
    string FullName,
    bool IsActive,
    DateTimeOffset? LastLoginAt,
    IReadOnlyList<string> Roles);
