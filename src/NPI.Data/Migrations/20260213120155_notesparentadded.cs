using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class notesparentadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Parent",
                table: "Notes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Parent",
                table: "Notes");
        }
    }
}
