namespace Refleks360.Domain.Organization;

/// <summary>EmployeeSalary geçmişindeki bir değişikliğin sebebi.</summary>
public enum SalaryChangeReason
{
    NewHire = 0,
    AnnualMerit = 1,
    Promotion = 2,
    MarketAdjustment = 3,
    Retention = 4,
    Other = 99,
}
