using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KitanoUserService.API.Migrations
{
    public partial class Group_Roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USERS_GROUP_ROLES",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    group_id = table.Column<int>(type: "integer", nullable: true),
                    roles_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS_GROUP_ROLES", x => x.id);
                    table.ForeignKey(
                        name: "FK_USERS_GROUP_ROLES_ROLES_roles_id",
                        column: x => x.roles_id,
                        principalTable: "ROLES",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_USERS_GROUP_ROLES_USERS_GROUP_group_id",
                        column: x => x.group_id,
                        principalTable: "USERS_GROUP",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USERS_GROUP_ROLES_group_id",
                table: "USERS_GROUP_ROLES",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_GROUP_ROLES_roles_id",
                table: "USERS_GROUP_ROLES",
                column: "roles_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "USERS_GROUP_ROLES");
        }
    }
}
