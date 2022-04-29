using Microsoft.EntityFrameworkCore.Migrations;

namespace KitanoUserService.API.Migrations
{
    public partial class updateapprovalfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "approver_last",
                table: "APPROVAL_FUNCTION_STATUS",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_APPROVAL_FUNCTION_STATUS_approver_last",
                table: "APPROVAL_FUNCTION_STATUS",
                column: "approver_last");

            migrationBuilder.AddForeignKey(
                name: "FK_APPROVAL_FUNCTION_STATUS_USERS_approver_last",
                table: "APPROVAL_FUNCTION_STATUS",
                column: "approver_last",
                principalTable: "USERS",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_APPROVAL_FUNCTION_STATUS_USERS_approver_last",
                table: "APPROVAL_FUNCTION_STATUS");

            migrationBuilder.DropIndex(
                name: "IX_APPROVAL_FUNCTION_STATUS_approver_last",
                table: "APPROVAL_FUNCTION_STATUS");

            migrationBuilder.DropColumn(
                name: "approver_last",
                table: "APPROVAL_FUNCTION_STATUS");
        }
    }
}
