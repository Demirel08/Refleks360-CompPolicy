using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Refleks360.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeAndOrgEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TaxNo = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    FiscalYearStartMonth = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobFamilies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobFamilies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobGrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CareerBand = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    EvaluationScoreMin = table.Column<int>(type: "int", nullable: true),
                    EvaluationScoreMax = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobGrades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    City = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RegionalIndexPercent = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobGradeId = table.Column<int>(type: "int", nullable: false),
                    JobFamilyId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    BenchmarkMatchName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_JobFamilies_JobFamilyId",
                        column: x => x.JobFamilyId,
                        principalTable: "JobFamilies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Positions_JobGrades_JobGradeId",
                        column: x => x.JobGradeId,
                        principalTable: "JobGrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ParentDepartmentId = table.Column<int>(type: "int", nullable: true),
                    ManagerEmployeeId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CostCenterCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Departments_Departments_ParentDepartmentId",
                        column: x => x.ParentDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    EmploymentType = table.Column<int>(type: "int", nullable: false),
                    WorkSchedule = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TerminationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Currency", "FiscalYearStartMonth", "IsActive", "LegalName", "Name", "TaxNo" },
                values: new object[] { 1, "TL", 1, true, "Refleks Demo Sanayi ve Ticaret Anonim Şirketi", "Refleks Demo A.Ş.", "0000000000" });

            migrationBuilder.InsertData(
                table: "JobFamilies",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Yazılım, donanım ve sistem mühendisliği", "Mühendislik" },
                    { 2, "Üretim, üretim yönetimi, kalite", "Operasyon" },
                    { 3, "IK, finans, satın alma, hukuk", "Destek Fonksiyonlar" },
                    { 4, "Satış, pazarlama, müşteri ilişkileri", "Ticari" }
                });

            migrationBuilder.InsertData(
                table: "JobGrades",
                columns: new[] { "Id", "CareerBand", "Code", "EvaluationScoreMax", "EvaluationScoreMin", "Name", "OrderIndex" },
                values: new object[,]
                {
                    { 1, 0, "P1", null, null, "Uzman Yardımcısı", 1 },
                    { 2, 1, "P2", null, null, "Uzman", 2 },
                    { 3, 1, "P3", null, null, "Kıdemli Uzman", 3 },
                    { 4, 2, "M1", null, null, "Takım Lideri", 4 },
                    { 5, 2, "M2", null, null, "Müdür", 5 },
                    { 6, 3, "D1", null, null, "Direktör", 6 }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "CompanyId", "CostCenterCode", "IsActive", "ManagerEmployeeId", "Name", "ParentDepartmentId" },
                values: new object[,]
                {
                    { 1, 1, "BT-100", true, null, "Bilgi Teknolojileri", null },
                    { 2, 1, "IK-200", true, null, "İnsan Kaynakları", null },
                    { 3, 1, "FI-300", true, null, "Finans", null },
                    { 4, 1, "SAT-400", true, null, "Satış ve Pazarlama", null },
                    { 5, 1, "URT-500", true, null, "Üretim", null }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "City", "CompanyId", "Country", "IsActive", "Name", "RegionalIndexPercent" },
                values: new object[,]
                {
                    { 1, "İstanbul", 1, "Türkiye", true, "İstanbul Genel Müdürlük", 100m },
                    { 2, "Ankara", 1, "Türkiye", true, "Ankara Bölge", 95m },
                    { 3, "İzmir", 1, "Türkiye", true, "İzmir Fabrika", 90m }
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Id", "BenchmarkMatchName", "Description", "IsActive", "JobFamilyId", "JobGradeId", "Title" },
                values: new object[,]
                {
                    { 1, null, null, true, 1, 1, "Yazılım Geliştirme Mühendisi (Junior)" },
                    { 2, null, null, true, 1, 2, "Yazılım Geliştirme Mühendisi" },
                    { 3, null, null, true, 1, 3, "Kıdemli Yazılım Geliştirme Mühendisi" },
                    { 4, null, null, true, 1, 4, "Yazılım Takım Lideri" },
                    { 5, null, null, true, 3, 2, "İK Uzmanı" },
                    { 6, null, null, true, 3, 5, "İK Müdürü" },
                    { 7, null, null, true, 3, 3, "Mali Müşavir" },
                    { 8, null, null, true, 4, 2, "Satış Temsilcisi" },
                    { 9, null, null, true, 2, 1, "Üretim Operatörü" },
                    { 10, null, null, true, 2, 3, "Üretim Mühendisi" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DepartmentId", "Email", "EmployeeNumber", "EmploymentType", "FirstName", "Gender", "HireDate", "IsDeleted", "IsLocked", "LastName", "LocationId", "ManagerId", "Notes", "Phone", "PositionId", "Status", "TerminationDate", "WorkSchedule" },
                values: new object[,]
                {
                    { 1, 1, "ahmet.yilmaz@refleks360.local", "1001", 0, "Ahmet", 1, new DateOnly(2020, 1, 1), false, false, "Yılmaz", 1, null, null, null, 2, 0, null, 0 },
                    { 2, 1, "mehmet.demir@refleks360.local", "1002", 0, "Mehmet", 1, new DateOnly(2020, 2, 17), false, false, "Demir", 1, null, null, null, 2, 0, null, 0 },
                    { 3, 1, "mustafa.kara@refleks360.local", "1003", 0, "Mustafa", 1, new DateOnly(2020, 4, 4), false, false, "Kara", 1, null, null, null, 2, 0, null, 0 },
                    { 4, 1, "ali.celik@refleks360.local", "1004", 0, "Ali", 1, new DateOnly(2020, 5, 21), false, false, "Çelik", 1, null, null, null, 3, 0, null, 0 },
                    { 5, 1, "hasan.yildiz@refleks360.local", "1005", 0, "Hasan", 1, new DateOnly(2020, 7, 7), false, false, "Yıldız", 1, null, null, null, 3, 0, null, 0 },
                    { 6, 1, "huseyin.aydin@refleks360.local", "1006", 0, "Hüseyin", 1, new DateOnly(2020, 8, 23), false, false, "Aydın", 1, null, null, null, 3, 0, null, 0 },
                    { 7, 1, "İbrahim.ozturk@refleks360.local", "1007", 0, "İbrahim", 1, new DateOnly(2020, 10, 9), false, false, "Öztürk", 2, null, null, null, 1, 0, null, 0 },
                    { 8, 1, "murat.aslan@refleks360.local", "1008", 0, "Murat", 1, new DateOnly(2020, 11, 25), false, false, "Aslan", 2, null, null, null, 1, 0, null, 0 },
                    { 9, 1, "emre.dogan@refleks360.local", "1009", 0, "Emre", 1, new DateOnly(2021, 1, 11), false, false, "Doğan", 2, null, null, null, 1, 0, null, 0 },
                    { 10, 1, "burak.sahin@refleks360.local", "1010", 0, "Burak", 1, new DateOnly(2021, 2, 27), false, false, "Şahin", 1, null, null, null, 4, 0, null, 0 },
                    { 11, 1, "can.polat@refleks360.local", "1011", 0, "Can", 1, new DateOnly(2021, 4, 15), false, false, "Polat", 1, null, null, null, 4, 0, null, 0 },
                    { 12, 1, "onur.erdogan@refleks360.local", "1012", 0, "Onur", 1, new DateOnly(2021, 6, 1), false, false, "Erdoğan", 1, null, null, null, 4, 0, null, 0 },
                    { 13, 2, "serkan.korkmaz@refleks360.local", "1013", 0, "Serkan", 1, new DateOnly(2021, 7, 18), false, false, "Korkmaz", 1, null, null, null, 5, 0, null, 0 },
                    { 14, 2, "selim.kurt@refleks360.local", "1014", 0, "Selim", 1, new DateOnly(2021, 9, 3), false, false, "Kurt", 1, null, null, null, 5, 0, null, 0 },
                    { 15, 2, "tolga.acar@refleks360.local", "1015", 0, "Tolga", 1, new DateOnly(2021, 10, 20), false, false, "Acar", 2, null, null, null, 5, 0, null, 0 },
                    { 16, 2, "ayse.kaya@refleks360.local", "1016", 0, "Ayşe", 2, new DateOnly(2021, 12, 6), false, false, "Kaya", 1, null, null, null, 6, 0, null, 0 },
                    { 17, 3, "fatma.simsek@refleks360.local", "1017", 0, "Fatma", 2, new DateOnly(2022, 1, 22), false, false, "Şimşek", 1, null, null, null, 7, 0, null, 0 },
                    { 18, 3, "zeynep.arslan@refleks360.local", "1018", 0, "Zeynep", 2, new DateOnly(2022, 3, 10), false, false, "Arslan", 1, null, null, null, 7, 0, null, 0 },
                    { 19, 3, "elif.koc@refleks360.local", "1019", 0, "Elif", 2, new DateOnly(2022, 4, 26), false, false, "Koç", 1, null, null, null, 7, 0, null, 0 },
                    { 20, 3, "merve.ozdemir@refleks360.local", "1020", 0, "Merve", 2, new DateOnly(2022, 6, 12), false, false, "Özdemir", 1, null, null, null, 3, 0, null, 0 },
                    { 21, 4, "esra.cetin@refleks360.local", "1021", 0, "Esra", 2, new DateOnly(2022, 7, 29), false, false, "Çetin", 1, null, null, null, 8, 0, null, 0 },
                    { 22, 4, "gul.yavuz@refleks360.local", "1022", 0, "Gül", 2, new DateOnly(2022, 9, 14), false, false, "Yavuz", 2, null, null, null, 8, 0, null, 0 },
                    { 23, 4, "sevgi.yildirim@refleks360.local", "1023", 0, "Sevgi", 2, new DateOnly(2022, 10, 31), false, false, "Yıldırım", 2, null, null, null, 8, 0, null, 0 },
                    { 24, 4, "deniz.bulut@refleks360.local", "1024", 0, "Deniz", 2, new DateOnly(2022, 12, 17), false, false, "Bulut", 3, null, null, null, 8, 0, null, 0 },
                    { 25, 4, "pinar.gunes@refleks360.local", "1025", 0, "Pınar", 2, new DateOnly(2023, 2, 2), false, false, "Güneş", 3, null, null, null, 8, 0, null, 0 },
                    { 26, 5, "burcu.erdem@refleks360.local", "1026", 0, "Burcu", 2, new DateOnly(2023, 3, 21), false, false, "Erdem", 3, null, null, null, 9, 0, null, 0 },
                    { 27, 5, "selin.aktas@refleks360.local", "1027", 0, "Selin", 2, new DateOnly(2023, 5, 7), false, false, "Aktaş", 3, null, null, null, 9, 0, null, 0 },
                    { 28, 5, "cansu.sari@refleks360.local", "1028", 0, "Cansu", 2, new DateOnly(2023, 6, 23), false, false, "Sarı", 3, null, null, null, 9, 0, null, 0 },
                    { 29, 5, "damla.karaca@refleks360.local", "1029", 0, "Damla", 2, new DateOnly(2023, 8, 9), false, false, "Karaca", 3, null, null, null, 9, 0, null, 0 },
                    { 30, 5, "tugce.tekin@refleks360.local", "1030", 0, "Tuğçe", 2, new DateOnly(2023, 9, 25), false, false, "Tekin", 3, null, null, null, 10, 0, null, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_TaxNo",
                table: "Companies",
                column: "TaxNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyId_Name",
                table: "Departments",
                columns: new[] { "CompanyId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ManagerEmployeeId",
                table: "Departments",
                column: "ManagerEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeNumber",
                table: "Employees",
                column: "EmployeeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LocationId",
                table: "Employees",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerId",
                table: "Employees",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PositionId",
                table: "Employees",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Status",
                table: "Employees",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_JobFamilies_Name",
                table: "JobFamilies",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobGrades_Code",
                table: "JobGrades",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobGrades_OrderIndex",
                table: "JobGrades",
                column: "OrderIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CompanyId",
                table: "Locations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_JobFamilyId",
                table: "Positions",
                column: "JobFamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_JobGradeId",
                table: "Positions",
                column: "JobGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_Title",
                table: "Positions",
                column: "Title");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Employees_ManagerEmployeeId",
                table: "Departments",
                column: "ManagerEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Companies_CompanyId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Companies_CompanyId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Employees_ManagerEmployeeId",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "JobFamilies");

            migrationBuilder.DropTable(
                name: "JobGrades");
        }
    }
}
