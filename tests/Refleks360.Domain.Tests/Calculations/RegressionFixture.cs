using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Refleks360.Domain.Calculations;

namespace Refleks360.Domain.Tests.Calculations;

/// <summary>
/// <c>tests/Refleks360.Domain.Tests/Fixtures/regression-2026.json</c> dosyasını okuyan
/// sade modeller + yükleyici. JSON, <c>tools/regression/generate_fixture.py</c> tarafından
/// mevcut Python hesap motorundan üretilir. Bu dosyayı el ile düzenleme — script'i çalıştır.
/// </summary>
internal static class RegressionFixture
{
    private const string RelativePath = "Fixtures/regression-2026.json";

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
    };

    public static Fixture Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, RelativePath);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Regresyon fixture'ı bulunamadı: {path}. " +
                "tools/regression/generate_fixture.py'yi çalıştır.",
                path);
        }

        using var stream = File.OpenRead(path);
        var data = JsonSerializer.Deserialize<Fixture>(stream, Options)
            ?? throw new InvalidOperationException("Fixture deserialize edilemedi.");
        return data;
    }

    /// <summary>
    /// JSON'daki parametrelerden <see cref="TaxParameters"/> + ay → period eşlemesi üretir.
    /// </summary>
    public static (TaxParameters Parameters, Func<int, MonthlyTaxPeriod> PeriodForMonth) BuildEngine(Fixture fixture)
    {
        var p = fixture.Parameters;

        var brackets = p.IncomeTaxBrackets
            .Select(b => new TaxBracket(
                UpperLimit: ParseLimit(b.UpperLimit),
                Rate: (decimal)b.Rate))
            .ToList();

        var parameters = new TaxParameters
        {
            SgkEmployeeRate = (decimal)p.SgkEmployeeRate,
            UnemploymentEmployeeRate = (decimal)p.UnempEmployeeRate,
            SgkEmployerRate = (decimal)p.SgkEmployerRate,
            UnemploymentEmployerRate = (decimal)p.UnempEmployerRate,
            SgkEmployerDiscountRate = (decimal)p.SgkEmployerDiscountRate,
            ApplySgkEmployerDiscount = p.ApplySgkEmployerDiscount,
            StampTaxRate = (decimal)p.StampTaxRate,
            IncomeTaxBrackets = brackets,
        };

        var sgkMin = (decimal)p.SgkBaseMin;
        var sgkMax = (decimal)p.SgkBaseMax;
        var exemptionBase = (decimal)p.GvExemptionBase;
        var stampExemption = (decimal)p.StampExemptionPerMonth;
        var ratesByMonth = p.GvExemptionRateByMonth
            .ToDictionary(kv => int.Parse(kv.Key, CultureInfo.InvariantCulture), kv => (decimal)kv.Value);

        MonthlyTaxPeriod PeriodForMonth(int month) => new(
            StartMonth: month,
            EndMonth: month,
            SgkBaseMin: sgkMin,
            SgkBaseMax: sgkMax,
            GvExemptionAmount: exemptionBase,
            GvExemptionRate: ratesByMonth[month],
            StampExemptionAmount: stampExemption);

        return (parameters, PeriodForMonth);
    }

    private static decimal ParseLimit(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Number => element.GetDecimal(),
        JsonValueKind.String when element.GetString() == "Infinity" => decimal.MaxValue,
        _ => throw new InvalidOperationException($"Beklenmeyen upper_limit: {element}"),
    };

    // ------- DTO'lar (sadece JSON deserializasyonu için, dış dünyaya kapalı) -------

    public sealed class Fixture
    {
        public int Version { get; set; }
        public string? Generator { get; set; }
        public string? SourceEngine { get; set; }
        public int Year { get; set; }
        public FixtureParameters Parameters { get; set; } = default!;
        public List<FixtureCase> Cases { get; set; } = new();
    }

    public sealed class FixtureParameters
    {
        public double SgkEmployeeRate { get; set; }
        public double UnempEmployeeRate { get; set; }
        public double SgkEmployerRate { get; set; }
        public double UnempEmployerRate { get; set; }
        public double SgkEmployerDiscountRate { get; set; }
        public bool ApplySgkEmployerDiscount { get; set; }
        public double StampTaxRate { get; set; }
        public List<FixtureBracket> IncomeTaxBrackets { get; set; } = new();
        public double SgkBaseMin { get; set; }
        public double SgkBaseMax { get; set; }
        public double GvExemptionBase { get; set; }
        public double StampExemptionPerMonth { get; set; }
        public Dictionary<string, double> GvExemptionRateByMonth { get; set; } = new();
    }

    public sealed class FixtureBracket
    {
        public JsonElement UpperLimit { get; set; }
        public double Rate { get; set; }
    }

    public sealed class FixtureCase
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public double Gross { get; set; }
        public List<FixtureMonth> Months { get; set; } = new();
    }

    public sealed class FixtureMonth
    {
        public int Month { get; set; }
        public double CumulativeTaxableBefore { get; set; }
        public double Pek { get; set; }
        public double EmployeeContribution { get; set; }
        public double MonthlyTaxBase { get; set; }
        public double IncomeTaxRaw { get; set; }
        public double IncomeTaxExemption { get; set; }
        public double IncomeTax { get; set; }
        public double StampTax { get; set; }
        public double Net { get; set; }
        public double EmployerCost { get; set; }
    }
}
