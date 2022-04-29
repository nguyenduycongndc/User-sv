using Microsoft.EntityFrameworkCore.Migrations;

namespace KitanoUserService.API.Migrations
{
    public partial class catauditrequest7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "typename",
                table: "CAT_AUDIT_REQUEST",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "typecode",
                table: "CAT_AUDIT_REQUEST",
                newName: "code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "CAT_AUDIT_REQUEST",
                newName: "typename");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "CAT_AUDIT_REQUEST",
                newName: "typecode");
        }
    }
}
