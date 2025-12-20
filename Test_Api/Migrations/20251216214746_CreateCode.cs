using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test_Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "promotions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "promotions");
        }
    }
}
