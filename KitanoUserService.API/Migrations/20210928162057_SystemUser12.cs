using Microsoft.EntityFrameworkCore.Migrations;

namespace KitanoUserService.API.Migrations
{
    public partial class SystemUser12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "users_id",
                table: "UsersWorkHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersWorkHistory_users_id",
                table: "UsersWorkHistory",
                column: "users_id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersWorkHistory_users_users_id",
                table: "UsersWorkHistory",
                column: "users_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersWorkHistory_users_users_id",
                table: "UsersWorkHistory");

            migrationBuilder.DropIndex(
                name: "IX_UsersWorkHistory_users_id",
                table: "UsersWorkHistory");

            migrationBuilder.DropColumn(
                name: "users_id",
                table: "UsersWorkHistory");
        }
    }
}
