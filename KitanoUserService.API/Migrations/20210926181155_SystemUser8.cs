using Microsoft.EntityFrameworkCore.Migrations;

namespace KitanoUserService.API.Migrations
{
    public partial class SystemUser8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_group_mapping_users_group_group_id1",
                table: "users_group_mapping");

            migrationBuilder.DropForeignKey(
                name: "FK_users_group_mapping_users_users_id1",
                table: "users_group_mapping");

            migrationBuilder.DropIndex(
                name: "IX_users_group_mapping_group_id1",
                table: "users_group_mapping");

            migrationBuilder.DropIndex(
                name: "IX_users_group_mapping_users_id1",
                table: "users_group_mapping");

            migrationBuilder.DropColumn(
                name: "group_id1",
                table: "users_group_mapping");

            migrationBuilder.DropColumn(
                name: "users_id1",
                table: "users_group_mapping");

            migrationBuilder.CreateIndex(
                name: "IX_users_group_mapping_group_id",
                table: "users_group_mapping",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_group_mapping_users_id",
                table: "users_group_mapping",
                column: "users_id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_group_mapping_users_group_group_id",
                table: "users_group_mapping",
                column: "group_id",
                principalTable: "users_group",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_group_mapping_users_users_id",
                table: "users_group_mapping",
                column: "users_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_group_mapping_users_group_group_id",
                table: "users_group_mapping");

            migrationBuilder.DropForeignKey(
                name: "FK_users_group_mapping_users_users_id",
                table: "users_group_mapping");

            migrationBuilder.DropIndex(
                name: "IX_users_group_mapping_group_id",
                table: "users_group_mapping");

            migrationBuilder.DropIndex(
                name: "IX_users_group_mapping_users_id",
                table: "users_group_mapping");

            migrationBuilder.AddColumn<int>(
                name: "group_id1",
                table: "users_group_mapping",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "users_id1",
                table: "users_group_mapping",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_group_mapping_group_id1",
                table: "users_group_mapping",
                column: "group_id1");

            migrationBuilder.CreateIndex(
                name: "IX_users_group_mapping_users_id1",
                table: "users_group_mapping",
                column: "users_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_users_group_mapping_users_group_group_id1",
                table: "users_group_mapping",
                column: "group_id1",
                principalTable: "users_group",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_group_mapping_users_users_id1",
                table: "users_group_mapping",
                column: "users_id1",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
