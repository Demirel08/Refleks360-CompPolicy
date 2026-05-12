using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Abstractions;
using Refleks360.Application.Employees;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Services;

public sealed class EmployeeQueryService(CompDbContext db) : IEmployeeQueryService
{
    public async Task<IReadOnlyList<EmployeeListItem>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.Employees
            .AsNoTracking()
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Select(e => new EmployeeListItem(
                e.Id,
                e.EmployeeNumber,
                e.FirstName + " " + e.LastName,
                e.Position.Title,
                e.Position.JobGrade.Code,
                e.Department.Name,
                e.Location.City,
                e.Status,
                e.Gender,
                e.HireDate))
            .ToListAsync(ct);
    }
}
