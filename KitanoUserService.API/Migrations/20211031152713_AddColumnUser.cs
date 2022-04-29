using Microsoft.EntityFrameworkCore.Migrations;

namespace KitanoUserService.API.Migrations
{
    public partial class AddColumnUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "USERS",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "modified_by",
                table: "USERS",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_by",
                table: "USERS");

            migrationBuilder.DropColumn(
                name: "modified_by",
                table: "USERS");
        }
    }
}
