using MediatR;

namespace Refleks360.Application.Employees.Commands;

/// <summary>Çalışanı soft-delete eder (IsDeleted=true).</summary>
public sealed record DeleteEmployeeCommand(int Id) : IRequest<Unit>;
