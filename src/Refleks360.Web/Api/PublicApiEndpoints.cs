using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Refleks360.Application.Employees.Commands;
using Refleks360.Infrastructure.Persistence;

namespace Refleks360.Web.Api;

/// <summary>
/// Refleks 360 dış sistem entegrasyon API'leri. X-Api-Key header zorunlu.
/// </summary>
public static class PublicApiEndpoints
{
    public static IEndpointRouteBuilder MapPublicApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1")
            .RequireAuthorization(p => p
                .AddAuthenticationSchemes(ApiKeyAuthOptions.Scheme)
                .RequireRole("ApiClient"))
            .DisableAntiforgery();

        // GET /api/v1/employees
        group.MapGet("/employees", async (CompDbContext db, CancellationToken ct) =>
        {
            var list = await db.Employees.AsNoTracking()
                .Select(e => new
                {
                    e.Id, e.EmployeeNumber, e.FirstName, e.LastName, e.Email,
                    Department = e.Department.Name,
                    Position = e.Position.Title,
                    Grade = e.Position.JobGrade.Code,
                    e.HireDate, Status = e.Status.ToString(),
                    CurrentGross = db.EmployeeSalaries.Where(s => s.EmployeeId == e.Id && s.EndDate == null)
                        .Select(s => (decimal?)s.GrossMonthly).FirstOrDefault(),
                })
                .ToListAsync(ct);
            return Results.Ok(list);
        });

        // GET /api/v1/employees/{id}
        group.MapGet("/employees/{id:int}", async (int id, CompDbContext db, CancellationToken ct) =>
        {
            var e = await db.Employees.AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new { x.Id, x.EmployeeNumber, x.FirstName, x.LastName, x.Email, x.Phone, x.HireDate, x.TerminationDate })
                .FirstOrDefaultAsync(ct);
            return e is null ? Results.NotFound() : Results.Ok(e);
        });

        // GET /api/v1/employees/{id}/salaries
        group.MapGet("/employees/{id:int}/salaries", async (int id, CompDbContext db, CancellationToken ct) =>
        {
            var rows = await db.EmployeeSalaries.AsNoTracking()
                .Where(s => s.EmployeeId == id)
                .OrderByDescending(s => s.EffectiveDate)
                .Select(s => new { s.Id, s.GrossMonthly, s.EffectiveDate, s.EndDate, Reason = s.Reason.ToString() })
                .ToListAsync(ct);
            return Results.Ok(rows);
        });

        // POST /api/v1/employees (sicil ile upsert)
        group.MapPost("/employees", async (CreateEmployeePayload p, MediatR.IMediator mediator, CancellationToken ct) =>
        {
            var id = await mediator.Send(new CreateEmployeeCommand(
                p.EmployeeNumber, p.FirstName, p.LastName, p.Email, p.Phone,
                Refleks360.Domain.Organization.Gender.NotSpecified,
                Refleks360.Domain.Organization.EmploymentType.Permanent,
                Refleks360.Domain.Organization.WorkSchedule.FullTime,
                p.HireDate, p.PositionId, p.DepartmentId, p.LocationId, p.ManagerId, p.Notes), ct);
            return Results.Created($"/api/v1/employees/{id}", new { id });
        });

        // GET /api/v1/companies
        group.MapGet("/companies", async (CompDbContext db, CancellationToken ct) =>
            await db.Companies.AsNoTracking().Select(c => new { c.Id, c.Name, c.TaxNo }).ToListAsync(ct));

        // GET /api/v1/departments
        group.MapGet("/departments", async (CompDbContext db, CancellationToken ct) =>
            await db.Departments.AsNoTracking().Select(d => new { d.Id, d.Name, d.CompanyId }).ToListAsync(ct));

        // GET /api/v1/positions
        group.MapGet("/positions", async (CompDbContext db, CancellationToken ct) =>
            await db.Positions.AsNoTracking()
                .Select(p => new { p.Id, p.Title, Grade = p.JobGrade.Code, Family = p.JobFamily.Name }).ToListAsync(ct));

        return endpoints;
    }
}

public sealed record CreateEmployeePayload(
    string EmployeeNumber, string FirstName, string LastName,
    string? Email, string? Phone, DateOnly HireDate,
    int PositionId, int DepartmentId, int LocationId, int? ManagerId, string? Notes);
