using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KitanoUserService.API.Migrations
{
    public partial class SystemParameter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SYSTEM_PARAMETER",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sub_system = table.Column<string>(type: "text", nullable: true),
                    parameter_name = table.Column<string>(type: "text", nullable: true),
                    value = table.Column<string>(type: "text", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    reset_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    default_value = table.Column<string>(type: "text", nullable: true),
                    default_note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_PARAMETER", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SYSTEM_PARAMETER");

            migrationBuilder.AddColumn<int>(
                name: "UsersGroupId",
                table: "USERS_ROLES",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_USERS_ROLES_UsersGroupId",
                table: "USERS_ROLES",
                column: "UsersGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_USERS_ROLES_USERS_GROUP_UsersGroupId",
                table: "USERS_ROLES",
                column: "UsersGroupId",
                principalTable: "USERS_GROUP",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
