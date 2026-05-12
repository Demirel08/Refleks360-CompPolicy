using FluentValidation;
using MediatR;
using Refleks360.Domain.Organization;

namespace Refleks360.Application.Employees.Commands;

public sealed record UpdateEmployeeCommand(
    int Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    Gender Gender,
    EmploymentType EmploymentType,
    WorkSchedule WorkSchedule,
    EmployeeStatus Status,
    DateOnly HireDate,
    DateOnly? TerminationDate,
    int PositionId,
    int DepartmentId,
    int LocationId,
    int? ManagerId,
    bool IsLocked,
    string? Notes) : IRequest<Unit>;

public sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.EmployeeNumber).NotEmpty().MaximumLength(32);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(128);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.PositionId).GreaterThan(0);
        RuleFor(x => x.DepartmentId).GreaterThan(0);
        RuleFor(x => x.LocationId).GreaterThan(0);
        RuleFor(x => x.TerminationDate)
            .GreaterThanOrEqualTo(x => x.HireDate)
            .When(x => x.TerminationDate.HasValue);
        RuleFor(x => x.ManagerId).NotEqual(x => x.Id).WithMessage("Kendisi yöneticisi olamaz.");
    }
}
