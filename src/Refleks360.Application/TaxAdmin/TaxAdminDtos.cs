namespace Refleks360.Application.TaxAdmin;

public sealed record TaxYearAdminDto(
    int Year,
    decimal SgkEmployeeRate,
    decimal UnemploymentEmployeeRate,
    decimal SgkEmployerRate,
    decimal UnemploymentEmployerRate,
    decimal SgkEmployerDiscountRate,
    bool ApplySgkEmployerDiscount,
    decimal StampTaxRate);

public sealed record TaxBracketAdminDto(int Id, int OrderIndex, decimal UpperLimit, decimal Rate);

public sealed record MonthlyPeriodAdminDto(int Id, int StartMonth, int EndMonth, decimal SgkBaseMin, decimal SgkBaseMax, decimal GvExemptionAmount, decimal GvExemptionRate, decimal StampExemptionAmount);

public sealed record TaxYearDetail(
    TaxYearAdminDto Year,
    IReadOnlyList<TaxBracketAdminDto> Brackets,
    IReadOnlyList<MonthlyPeriodAdminDto> Periods);
