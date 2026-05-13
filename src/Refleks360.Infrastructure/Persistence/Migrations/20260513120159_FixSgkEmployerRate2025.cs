using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Refleks360.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixSgkEmployerRate2025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TaxYears",
                keyColumn: "Year",
                keyValue: 2026,
                column: "SgkEmployerRate",
                value: 0.2175m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TaxYears",
                keyColumn: "Year",
                keyValue: 2026,
                column: "SgkEmployerRate",
                value: 0.2075m);
        }
    }
}
