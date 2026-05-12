using MediatR;
using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Employees;
using Refleks360.Application.Employees.Queries;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Handlers.Employees;

internal sealed class GetEmployeeByIdHandler(CompDbContext db)
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetail?>
{
    public async Task<EmployeeDetail?> Handle(GetEmployeeByIdQuery request, CancellationToken ct)
    {
        return await db.Employees
            .AsNoTracking()
            .Where(e => e.Id == request.Id)
            .Select(e => new EmployeeDetail(
                e.Id,
                e.EmployeeNumber,
                e.FirstName,
                e.LastName,
                e.Email,
                e.Phone,
                e.Gender,
                e.EmploymentType,
                e.WorkSchedule,
                e.Status,
                e.HireDate,
                e.TerminationDate,
                e.PositionId,
                e.Position.Title,
                e.Position.JobGrade.Code,
                e.DepartmentId,
                e.Department.Name,
                e.LocationId,
                e.Location.Name,
                e.ManagerId,
                e.Manager == null ? null : (e.Manager.FirstName + " " + e.Manager.LastName),
                e.IsLocked,
                e.Notes))
            .FirstOrDefaultAsync(ct);
    }
}
