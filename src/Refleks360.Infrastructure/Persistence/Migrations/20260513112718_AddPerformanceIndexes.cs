using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Refleks360.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaries_EmployeeId_EndDate",
                table: "EmployeeSalaries",
                columns: new[] { "EmployeeId", "EndDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeSalaries_EmployeeId_EndDate",
                table: "EmployeeSalaries");
        }
    }
}
