using MediatR;

namespace Refleks360.Application.Employees.Queries;

/// <summary>Belirli bir çalışanın detayını döner; bulunamazsa <c>null</c>.</summary>
public sealed record GetEmployeeByIdQuery(int Id) : IRequest<EmployeeDetail?>;
