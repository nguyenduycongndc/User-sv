using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KitanoUserService.API.Migrations
{
    public partial class PermissionMenuCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PERMISSION_MENU",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    menuid = table.Column<int>(type: "integer", nullable: false),
                    permissionid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PERMISSION_MENU", x => x.id);
                    table.ForeignKey(
                        name: "FK_PERMISSION_MENU_MENU_menuid",
                        column: x => x.menuid,
                        principalTable: "MENU",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PERMISSION_MENU_PERMISSION_permissionid",
                        column: x => x.permissionid,
                        principalTable: "PERMISSION",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PERMISSION_MENU_menuid",
                table: "PERMISSION_MENU",
                column: "menuid");

            migrationBuilder.CreateIndex(
                name: "IX_PERMISSION_MENU_permissionid",
                table: "PERMISSION_MENU",
                column: "permissionid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PERMISSION_MENU");
        }
    }
}
