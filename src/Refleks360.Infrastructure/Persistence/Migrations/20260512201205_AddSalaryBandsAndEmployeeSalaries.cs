using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Refleks360.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSalaryBandsAndEmployeeSalaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeSalaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    GrossMonthly = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetMonthlyCached = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EmployerCostCached = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    ChangePercent = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    ChangeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ScenarioId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSalaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaries_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalaryBands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobGradeId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    Min = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Mid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Max = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryBands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryBands_JobGrades_JobGradeId",
                        column: x => x.JobGradeId,
                        principalTable: "JobGrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalaryBands_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "EmployeeSalaries",
                columns: new[] { "Id", "ChangeAmount", "ChangePercent", "CreatedAtUtc", "CreatedBy", "EffectiveDate", "EmployeeId", "EmployerCostCached", "EndDate", "GrossMonthly", "NetMonthlyCached", "Reason", "ScenarioId" },
                values: new object[,]
                {
                    { 1, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 1, null, null, 59500m, null, 0, null },
                    { 2, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 2, null, null, 64400m, null, 0, null },
                    { 3, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 3, null, null, 70000m, null, 0, null },
                    { 4, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 4, null, null, 105000m, null, 0, null },
                    { 5, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 5, null, null, 98000m, null, 0, null },
                    { 6, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 6, null, null, 110000m, null, 0, null },
                    { 7, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 7, null, null, 44000m, null, 0, null },
                    { 8, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 8, null, null, 51500m, null, 0, null },
                    { 9, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 9, null, null, 47500m, null, 0, null },
                    { 10, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 10, null, null, 156800m, null, 0, null },
                    { 11, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 11, null, null, 126000m, null, 0, null },
                    { 12, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 12, null, null, 147000m, null, 0, null },
                    { 13, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 13, null, null, 67200m, null, 0, null },
                    { 14, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 14, null, null, 71400m, null, 0, null },
                    { 15, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 15, null, null, 69300m, null, 0, null },
                    { 16, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 16, null, null, 216000m, null, 0, null },
                    { 17, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 17, null, null, 93000m, null, 0, null },
                    { 18, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 18, null, null, 106000m, null, 0, null },
                    { 19, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 19, null, null, 100000m, null, 0, null },
                    { 20, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 20, null, null, 97000m, null, 0, null },
                    { 21, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 21, null, null, 63700m, null, 0, null },
                    { 22, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 22, null, null, 72800m, null, 0, null },
                    { 23, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 23, null, null, 70000m, null, 0, null },
                    { 24, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 24, null, null, 65800m, null, 0, null },
                    { 25, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 25, null, null, 74900m, null, 0, null },
                    { 26, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 26, null, null, 44500m, null, 0, null },
                    { 27, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 27, null, null, 50500m, null, 0, null },
                    { 28, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 28, null, null, 49000m, null, 0, null },
                    { 29, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 29, null, null, 52500m, null, 0, null },
                    { 30, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed", new DateOnly(2026, 1, 1), 30, null, null, 96000m, null, 0, null }
                });

            migrationBuilder.InsertData(
                table: "SalaryBands",
                columns: new[] { "Id", "EffectiveDate", "EndDate", "JobGradeId", "LocationId", "Max", "Mid", "Min" },
                values: new object[,]
                {
                    { 1, new DateOnly(2026, 1, 1), null, 1, null, 60000m, 50000m, 40000m },
                    { 2, new DateOnly(2026, 1, 1), null, 2, null, 85000m, 70000m, 55000m },
                    { 3, new DateOnly(2026, 1, 1), null, 3, null, 120000m, 100000m, 80000m },
                    { 4, new DateOnly(2026, 1, 1), null, 4, null, 170000m, 140000m, 110000m },
                    { 5, new DateOnly(2026, 1, 1), null, 5, null, 240000m, 200000m, 160000m },
                    { 6, new DateOnly(2026, 1, 1), null, 6, null, 360000m, 300000m, 240000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaries_EmployeeId_EffectiveDate",
                table: "EmployeeSalaries",
                columns: new[] { "EmployeeId", "EffectiveDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SalaryBands_JobGradeId_LocationId_EffectiveDate",
                table: "SalaryBands",
                columns: new[] { "JobGradeId", "LocationId", "EffectiveDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SalaryBands_LocationId",
                table: "SalaryBands",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeSalaries");

            migrationBuilder.DropTable(
                name: "SalaryBands");
        }
    }
}
