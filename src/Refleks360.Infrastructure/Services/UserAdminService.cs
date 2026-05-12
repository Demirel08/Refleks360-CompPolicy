using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Users;
using Refleks360.Infrastructure.Identity;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class UserAdminService(
    UserManager<ApplicationUser> userMgr,
    CompDbContext db) : IUserAdminService
{
    public async Task<IReadOnlyList<UserListItem>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await userMgr.Users.AsNoTracking()
            .OrderBy(u => u.UserName)
            .ToListAsync(ct);

        var roleMap = await db.UserRoles.AsNoTracking()
            .Join(db.Roles.AsNoTracking(),
                ur => ur.RoleId, r => r.Id,
                (ur, r) => new { ur.UserId, r.Name })
            .ToListAsync(ct);

        return users.Select(u => new UserListItem(
            u.Id,
            u.UserName ?? "(yok)",
            u.Email ?? "",
            u.FullName,
            u.IsActive,
            u.LastLoginAt,
            roleMap.Where(r => r.UserId == u.Id).Select(r => r.Name!).ToArray())).ToList();
    }

    public async Task<bool> SetActiveAsync(string userId, bool isActive, CancellationToken ct = default)
    {
        var user = await userMgr.FindByIdAsync(userId);
        if (user is null) return false;
        user.IsActive = isActive;
        var result = await userMgr.UpdateAsync(user);
        if (result.Succeeded && !isActive)
        {
            // Aktif değilse security stamp güncelle: revalidating provider düşürür.
            await userMgr.UpdateSecurityStampAsync(user);
        }
        return result.Succeeded;
    }

    public async Task<bool> SetRolesAsync(string userId, IReadOnlyList<string> roleNames, CancellationToken ct = default)
    {
        var user = await userMgr.FindByIdAsync(userId);
        if (user is null) return false;

        var currentRoles = await userMgr.GetRolesAsync(user);
        var toRemove = currentRoles.Except(roleNames).ToList();
        var toAdd = roleNames.Except(currentRoles).ToList();

        if (toRemove.Count > 0)
        {
            var rr = await userMgr.RemoveFromRolesAsync(user, toRemove);
            if (!rr.Succeeded) return false;
        }
        if (toAdd.Count > 0)
        {
            var rr = await userMgr.AddToRolesAsync(user, toAdd);
            if (!rr.Succeeded) return false;
        }

        // Cookie permission claim'leri rolden geliyor; security stamp güncelle ki yeniden imzalansın
        await userMgr.UpdateSecurityStampAsync(user);
        return true;
    }

    public async Task<(bool ok, string? error, string? createdUserId)> CreateAsync(
        string userName, string email, string fullName, string password,
        IReadOnlyList<string> roles, CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName,
            IsActive = true,
        };
        var create = await userMgr.CreateAsync(user, password);
        if (!create.Succeeded)
        {
            return (false, string.Join("; ", create.Errors.Select(e => e.Description)), null);
        }
        if (roles.Count > 0)
        {
            await userMgr.AddToRolesAsync(user, roles);
        }
        return (true, null, user.Id);
    }

    public async Task<bool> ResetPasswordAsync(string userId, string newPassword, CancellationToken ct = default)
    {
        var user = await userMgr.FindByIdAsync(userId);
        if (user is null) return false;
        var token = await userMgr.GeneratePasswordResetTokenAsync(user);
        var result = await userMgr.ResetPasswordAsync(user, token, newPassword);
        if (result.Succeeded)
        {
            await userMgr.UpdateSecurityStampAsync(user);
        }
        return result.Succeeded;
    }
}
