using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Refleks360.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScenarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scenarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ParametersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppliedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AppliedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scenarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScenarioId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    OldGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OldNetMonthly = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewNetMonthly = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OldEmployerCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewEmployerCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RaisePercent = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    RaiseAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    IsManuallyOverridden = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioEmployees_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScenarioEmployees_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioEmployees_EmployeeId",
                table: "ScenarioEmployees",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioEmployees_ScenarioId_EmployeeId",
                table: "ScenarioEmployees",
                columns: new[] { "ScenarioId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_CreatedAtUtc",
                table: "Scenarios",
                column: "CreatedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScenarioEmployees");

            migrationBuilder.DropTable(
                name: "Scenarios");
        }
    }
}
