using MediatR;
using Refleks360.Application.Employees.Commands;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Infrastructure.Handlers.Employees;

internal sealed class UpdateEmployeeHandler(CompDbContext db)
    : IRequestHandler<UpdateEmployeeCommand, Unit>
{
    public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken ct)
    {
        var entity = await db.Employees.FindAsync(new object[] { request.Id }, ct)
            ?? throw new InvalidOperationException($"Çalışan bulunamadı: {request.Id}");

        entity.EmployeeNumber = request.EmployeeNumber;
        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Email = request.Email;
        entity.Phone = request.Phone;
        entity.Gender = request.Gender;
        entity.EmploymentType = request.EmploymentType;
        entity.WorkSchedule = request.WorkSchedule;
        entity.Status = request.Status;
        entity.HireDate = request.HireDate;
        entity.TerminationDate = request.TerminationDate;
        entity.PositionId = request.PositionId;
        entity.DepartmentId = request.DepartmentId;
        entity.LocationId = request.LocationId;
        entity.ManagerId = request.ManagerId;
        entity.IsLocked = request.IsLocked;
        entity.Notes = request.Notes;

        await db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
