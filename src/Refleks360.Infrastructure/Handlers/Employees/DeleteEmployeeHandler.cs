using MediatR;
using Refleks360.Application.Employees.Commands;
using Refleks360.Infrastructure.Persistence;
using Refleks360.Infrastructure.Services;

namespace Refleks360.Infrastructure.Handlers.Employees;

internal sealed class DeleteEmployeeHandler(CompDbContext db, CacheInvalidator cacheInvalidator)
    : IRequestHandler<DeleteEmployeeCommand, Unit>
{
    public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        var entity = await db.Employees.FindAsync(new object[] { request.Id }, ct)
            ?? throw new InvalidOperationException($"Çalışan bulunamadı: {request.Id}");
        entity.IsDeleted = true;
        await db.SaveChangesAsync(ct);
        cacheInvalidator.InvalidateEmployees();
        return Unit.Value;
    }
}
