using Microsoft.EntityFrameworkCore.Migrations;

namespace KitanoUserService.API.Migrations
{
    public partial class users_roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USERS_ROLES",
                columns: table => new
                {
                    users_id = table.Column<int>(type: "integer", nullable: true),
                    roles_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_USERS_ROLES_ROLES_roles_id",
                        column: x => x.roles_id,
                        principalTable: "ROLES",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_USERS_ROLES_USERS_users_id",
                        column: x => x.users_id,
                        principalTable: "USERS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USERS_ROLES_roles_id",
                table: "USERS_ROLES",
                column: "roles_id");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_ROLES_users_id",
                table: "USERS_ROLES",
                column: "users_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "USERS_ROLES");
        }
    }
}
