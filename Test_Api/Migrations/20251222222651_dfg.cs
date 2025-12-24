using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test_Api.Migrations
{
    /// <inheritdoc />
    public partial class dfg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationuserId",
                table: "Mycourse",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BuyAt",
                table: "Mycourse",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mycourse_ApplicationuserId",
                table: "Mycourse",
                column: "ApplicationuserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mycourse_AspNetUsers_ApplicationuserId",
                table: "Mycourse",
                column: "ApplicationuserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mycourse_AspNetUsers_ApplicationuserId",
                table: "Mycourse");

            migrationBuilder.DropIndex(
                name: "IX_Mycourse_ApplicationuserId",
                table: "Mycourse");

            migrationBuilder.DropColumn(
                name: "ApplicationuserId",
                table: "Mycourse");

            migrationBuilder.DropColumn(
                name: "BuyAt",
                table: "Mycourse");
        }
    }
}
