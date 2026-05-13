using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Refleks360.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHoldingSecondCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Currency", "FiscalYearStartMonth", "IsActive", "LegalName", "Name", "TaxNo" },
                values: new object[] { 2, "TL", 1, true, "Refleks Demo Yatırım Holding Anonim Şirketi", "Refleks Demo Yatırım A.Ş.", "0000000001" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
