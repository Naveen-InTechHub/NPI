using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class FGGSSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FGGSSetup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NpiRecordId = table.Column<int>(type: "INTEGER", nullable: false),
                    DosageForm = table.Column<string>(type: "TEXT", nullable: false),
                    ShapeAndColor = table.Column<string>(type: "TEXT", nullable: false),
                    TabletWeight = table.Column<int>(type: "INTEGER", nullable: false),
                    Dimension = table.Column<string>(type: "TEXT", nullable: false),
                    CoatingType = table.Column<string>(type: "TEXT", nullable: false),
                    Flavor = table.Column<string>(type: "TEXT", nullable: false),
                    PackageType = table.Column<string>(type: "TEXT", nullable: false),
                    CountPerUnit = table.Column<int>(type: "INTEGER", nullable: false),
                    ContainerMaterial = table.Column<string>(type: "TEXT", nullable: false),
                    ClouserType = table.Column<string>(type: "TEXT", nullable: false),
                    SecondaryPackaging = table.Column<string>(type: "TEXT", nullable: false),
                    LabelVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    RegulatoryMarket = table.Column<string>(type: "TEXT", nullable: false),
                    TargetShelfLife = table.Column<string>(type: "TEXT", nullable: false),
                    StorageConditions = table.Column<string>(type: "TEXT", nullable: false),
                    SpecialHandling = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FGGSSetup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FGGSSetup_NpiRecords_NpiRecordId",
                        column: x => x.NpiRecordId,
                        principalTable: "NpiRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualityPackageQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NpiRecordId = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionKey = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    QuestionText = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Value = table.Column<bool>(type: "INTEGER", nullable: true),
                    AnsweredBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AnsweredDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityPackageQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityPackageQuestion_NpiRecords_NpiRecordId",
                        column: x => x.NpiRecordId,
                        principalTable: "NpiRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FGGSSetup_NpiRecordId",
                table: "FGGSSetup",
                column: "NpiRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityPackageQuestion_NpiRecordId",
                table: "QualityPackageQuestion",
                column: "NpiRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FGGSSetup");

            migrationBuilder.DropTable(
                name: "QualityPackageQuestion");
        }
    }
}
