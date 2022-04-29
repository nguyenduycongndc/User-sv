using Microsoft.EntityFrameworkCore.Migrations;

namespace KitanoUserService.API.Migrations
{
    public partial class catauditrequest6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "modifiedBy",
                table: "CAT_AUDIT_REQUEST",
                newName: "modifiedby");

            migrationBuilder.RenameColumn(
                name: "modifiedAt",
                table: "CAT_AUDIT_REQUEST",
                newName: "modifiedat");

            migrationBuilder.RenameColumn(
                name: "deletedBy",
                table: "CAT_AUDIT_REQUEST",
                newName: "deletedby");

            migrationBuilder.RenameColumn(
                name: "deletedAt",
                table: "CAT_AUDIT_REQUEST",
                newName: "deletedat");

            migrationBuilder.RenameColumn(
                name: "createdBy",
                table: "CAT_AUDIT_REQUEST",
                newName: "createdby");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "CAT_AUDIT_REQUEST",
                newName: "createdat");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "modifiedby",
                table: "CAT_AUDIT_REQUEST",
                newName: "modifiedBy");

            migrationBuilder.RenameColumn(
                name: "modifiedat",
                table: "CAT_AUDIT_REQUEST",
                newName: "modifiedAt");

            migrationBuilder.RenameColumn(
                name: "deletedby",
                table: "CAT_AUDIT_REQUEST",
                newName: "deletedBy");

            migrationBuilder.RenameColumn(
                name: "deletedat",
                table: "CAT_AUDIT_REQUEST",
                newName: "deletedAt");

            migrationBuilder.RenameColumn(
                name: "createdby",
                table: "CAT_AUDIT_REQUEST",
                newName: "createdBy");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "CAT_AUDIT_REQUEST",
                newName: "createdAt");
        }
    }
}
