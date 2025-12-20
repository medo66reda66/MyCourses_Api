using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test_Api.Migrations
{
    /// <inheritdoc />
    public partial class PPP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Courses",
                newName: "QuantityLesons");

            migrationBuilder.AddColumn<string>(
                name: "language",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "totalstudents",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "language",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "totalstudents",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "QuantityLesons",
                table: "Courses",
                newName: "Quantity");
        }
    }
}
