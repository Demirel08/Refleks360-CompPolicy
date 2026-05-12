using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Refleks360.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaxYears",
                columns: table => new
                {
                    Year = table.Column<int>(type: "int", nullable: false),
                    SgkEmployeeRate = table.Column<decimal>(type: "decimal(8,6)", nullable: false),
                    UnemploymentEmployeeRate = table.Column<decimal>(type: "decimal(8,6)", nullable: false),
                    SgkEmployerRate = table.Column<decimal>(type: "decimal(8,6)", nullable: false),
                    UnemploymentEmployerRate = table.Column<decimal>(type: "decimal(8,6)", nullable: false),
                    SgkEmployerDiscountRate = table.Column<decimal>(type: "decimal(8,6)", nullable: false),
                    ApplySgkEmployerDiscount = table.Column<bool>(type: "bit", nullable: false),
                    StampTaxRate = table.Column<decimal>(type: "decimal(8,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxYears", x => x.Year);
                });

            migrationBuilder.CreateTable(
                name: "IncomeTaxBrackets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxYearId = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    UpperLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(8,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeTaxBrackets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeTaxBrackets_TaxYears_TaxYearId",
                        column: x => x.TaxYearId,
                        principalTable: "TaxYears",
                        principalColumn: "Year",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyTaxPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxYearId = table.Column<int>(type: "int", nullable: false),
                    StartMonth = table.Column<int>(type: "int", nullable: false),
                    EndMonth = table.Column<int>(type: "int", nullable: false),
                    SgkBaseMin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SgkBaseMax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GvExemptionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GvExemptionRate = table.Column<decimal>(type: "decimal(8,6)", nullable: false),
                    StampExemptionAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyTaxPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyTaxPeriods_TaxYears_TaxYearId",
                        column: x => x.TaxYearId,
                        principalTable: "TaxYears",
                        principalColumn: "Year",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TaxYears",
                columns: new[] { "Year", "ApplySgkEmployerDiscount", "SgkEmployeeRate", "SgkEmployerDiscountRate", "SgkEmployerRate", "StampTaxRate", "UnemploymentEmployeeRate", "UnemploymentEmployerRate" },
                values: new object[] { 2026, false, 0.14m, 0.05m, 0.2075m, 0.00759m, 0.01m, 0.02m });

            migrationBuilder.InsertData(
                table: "IncomeTaxBrackets",
                columns: new[] { "Id", "OrderIndex", "Rate", "TaxYearId", "UpperLimit" },
                values: new object[,]
                {
                    { 1, 0, 0.15m, 2026, 190000m },
                    { 2, 1, 0.20m, 2026, 400000m },
                    { 3, 2, 0.27m, 2026, 1500000m },
                    { 4, 3, 0.35m, 2026, 5300000m },
                    { 5, 4, 0.40m, 2026, 9999999999999.99m }
                });

            migrationBuilder.InsertData(
                table: "MonthlyTaxPeriods",
                columns: new[] { "Id", "EndMonth", "GvExemptionAmount", "GvExemptionRate", "SgkBaseMax", "SgkBaseMin", "StampExemptionAmount", "StartMonth", "TaxYearId" },
                values: new object[,]
                {
                    { 1, 1, 28075.50m, 0.15m, 247725.000m, 33030.00m, 250.6977m, 1, 2026 },
                    { 2, 2, 28075.50m, 0.15m, 247725.000m, 33030.00m, 250.6977m, 2, 2026 },
                    { 3, 3, 28075.50m, 0.15m, 247725.000m, 33030.00m, 250.6977m, 3, 2026 },
                    { 4, 4, 28075.50m, 0.15m, 247725.000m, 33030.00m, 250.6977m, 4, 2026 },
                    { 5, 5, 28075.50m, 0.15m, 247725.000m, 33030.00m, 250.6977m, 5, 2026 },
                    { 6, 6, 28075.50m, 0.15m, 247725.000m, 33030.00m, 250.6977m, 6, 2026 },
                    { 7, 7, 28075.50m, 0.15m, 247725.000m, 33030.00m, 250.6977m, 7, 2026 },
                    { 8, 8, 28075.50m, 0.20m, 247725.000m, 33030.00m, 250.6977m, 8, 2026 },
                    { 9, 9, 28075.50m, 0.20m, 247725.000m, 33030.00m, 250.6977m, 9, 2026 },
                    { 10, 10, 28075.50m, 0.20m, 247725.000m, 33030.00m, 250.6977m, 10, 2026 },
                    { 11, 11, 28075.50m, 0.20m, 247725.000m, 33030.00m, 250.6977m, 11, 2026 },
                    { 12, 12, 28075.50m, 0.20m, 247725.000m, 33030.00m, 250.6977m, 12, 2026 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTaxBrackets_TaxYearId_OrderIndex",
                table: "IncomeTaxBrackets",
                columns: new[] { "TaxYearId", "OrderIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyTaxPeriods_TaxYearId_StartMonth",
                table: "MonthlyTaxPeriods",
                columns: new[] { "TaxYearId", "StartMonth" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomeTaxBrackets");

            migrationBuilder.DropTable(
                name: "MonthlyTaxPeriods");

            migrationBuilder.DropTable(
                name: "TaxYears");
        }
    }
}
