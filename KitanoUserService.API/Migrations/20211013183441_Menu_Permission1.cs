using Microsoft.EntityFrameworkCore.Migrations;

namespace KitanoUserService.API.Migrations
{
    public partial class Menu_Permission1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuID",
                table: "ROLE_PERMISSION_MENU");

            migrationBuilder.DropColumn(
                name: "PermissionID",
                table: "ROLE_PERMISSION_MENU");

            migrationBuilder.DropColumn(
                name: "RoleID",
                table: "ROLE_PERMISSION_MENU");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuID",
                table: "ROLE_PERMISSION_MENU",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PermissionID",
                table: "ROLE_PERMISSION_MENU",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleID",
                table: "ROLE_PERMISSION_MENU",
                type: "integer",
                nullable: true);
        }
    }
}
