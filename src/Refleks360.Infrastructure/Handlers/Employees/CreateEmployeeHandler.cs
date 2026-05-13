using MediatR;
using Refleks360.Application.Employees.Commands;
using Refleks360.Domain.Organization;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Persistence.Entities;
using Refleks360.Infrastructure.Services;

namespace Refleks360.Infrastructure.Handlers.Employees;

internal sealed class CreateEmployeeHandler(CompDbContext db, CacheInvalidator cacheInvalidator)
    : IRequestHandler<CreateEmployeeCommand, int>
{
    public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken ct)
    {
        var entity = new EmployeeEntity
        {
            EmployeeNumber = request.EmployeeNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Gender = request.Gender,
            EmploymentType = request.EmploymentType,
            WorkSchedule = request.WorkSchedule,
            Status = EmployeeStatus.Active,
            HireDate = request.HireDate,
            PositionId = request.PositionId,
            DepartmentId = request.DepartmentId,
            LocationId = request.LocationId,
            ManagerId = request.ManagerId,
            Notes = request.Notes,
            IsLocked = false,
            IsDeleted = false,
        };

        db.Employees.Add(entity);
        await db.SaveChangesAsync(ct);
        cacheInvalidator.InvalidateEmployees();
        return entity.Id;
    }
}
