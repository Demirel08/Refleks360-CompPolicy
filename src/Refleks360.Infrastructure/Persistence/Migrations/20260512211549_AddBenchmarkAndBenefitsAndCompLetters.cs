using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Refleks360.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBenchmarkAndBenefitsAndCompLetters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BenchmarkProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenchmarkProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompensationLetters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    IssuedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    OldGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RaisePercent = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignedByUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SentToEmployee = table.Column<bool>(type: "bit", nullable: false),
                    PdfBlob = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompensationLetters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompensationLetters_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeBenefits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BenefitType = table.Column<int>(type: "int", nullable: false),
                    MonthlyValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeBenefits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeBenefits_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BenchmarkData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BenchmarkProviderId = table.Column<int>(type: "int", nullable: false),
                    BenchmarkPositionName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Sector = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CompanySize = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    P25 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P50 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    P75 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ImportedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImportedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenchmarkData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BenchmarkData_BenchmarkProviders_BenchmarkProviderId",
                        column: x => x.BenchmarkProviderId,
                        principalTable: "BenchmarkProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PositionBenchmarkMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    BenchmarkDataId = table.Column<int>(type: "int", nullable: false),
                    MatchType = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionBenchmarkMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionBenchmarkMappings_BenchmarkData_BenchmarkDataId",
                        column: x => x.BenchmarkDataId,
                        principalTable: "BenchmarkData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PositionBenchmarkMappings_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BenchmarkData_BenchmarkProviderId_BenchmarkPositionName",
                table: "BenchmarkData",
                columns: new[] { "BenchmarkProviderId", "BenchmarkPositionName" });

            migrationBuilder.CreateIndex(
                name: "IX_CompensationLetters_EmployeeId",
                table: "CompensationLetters",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBenefits_EmployeeId_BenefitType",
                table: "EmployeeBenefits",
                columns: new[] { "EmployeeId", "BenefitType" });

            migrationBuilder.CreateIndex(
                name: "IX_PositionBenchmarkMappings_BenchmarkDataId",
                table: "PositionBenchmarkMappings",
                column: "BenchmarkDataId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionBenchmarkMappings_PositionId_BenchmarkDataId",
                table: "PositionBenchmarkMappings",
                columns: new[] { "PositionId", "BenchmarkDataId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompensationLetters");

            migrationBuilder.DropTable(
                name: "EmployeeBenefits");

            migrationBuilder.DropTable(
                name: "PositionBenchmarkMappings");

            migrationBuilder.DropTable(
                name: "BenchmarkData");

            migrationBuilder.DropTable(
                name: "BenchmarkProviders");
        }
    }
}
