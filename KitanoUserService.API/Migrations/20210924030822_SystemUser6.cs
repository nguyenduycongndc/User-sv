using Microsoft.EntityFrameworkCore.Migrations;

namespace KitanoUserService.API.Migrations
{
    public partial class SystemUser6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_unit",
                table: "department");

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "department",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "department_type_id",
                table: "department",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code",
                table: "department");

            migrationBuilder.DropColumn(
                name: "department_type_id",
                table: "department");

            migrationBuilder.AddColumn<bool>(
                name: "is_unit",
                table: "department",
                type: "boolean",
                nullable: true);
        }
    }
}
