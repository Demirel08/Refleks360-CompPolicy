using FluentValidation;
using MediatR;
using Refleks360.Domain.Organization;

namespace Refleks360.Application.Employees.Commands;

/// <summary>Yeni çalışan oluşturur ve yeni Id'yi döner.</summary>
public sealed record CreateEmployeeCommand(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    Gender Gender,
    EmploymentType EmploymentType,
    WorkSchedule WorkSchedule,
    DateOnly HireDate,
    int PositionId,
    int DepartmentId,
    int LocationId,
    int? ManagerId,
    string? Notes) : IRequest<int>;

public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.EmployeeNumber).NotEmpty().MaximumLength(32);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(128);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(32);
        RuleFor(x => x.HireDate).NotEmpty();
        RuleFor(x => x.PositionId).GreaterThan(0);
        RuleFor(x => x.DepartmentId).GreaterThan(0);
        RuleFor(x => x.LocationId).GreaterThan(0);
    }
}
