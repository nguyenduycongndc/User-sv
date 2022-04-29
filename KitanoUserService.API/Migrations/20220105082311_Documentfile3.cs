using Microsoft.EntityFrameworkCore.Migrations;

namespace KitanoUserService.API.Migrations
{
    public partial class Documentfile3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "filetype",
                table: "DOCUMENT");

            migrationBuilder.DropColumn(
                name: "path",
                table: "DOCUMENT");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "filetype",
                table: "DOCUMENT",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "path",
                table: "DOCUMENT",
                type: "text",
                nullable: true);
        }
    }
}
