using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KitanoUserService.API.Migrations
{
    public partial class ApprovalConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APPROVAL_CONFIG",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<int>(type: "integer", nullable: true),
                    item_name = table.Column<string>(type: "text", nullable: true),
                    item_code = table.Column<string>(type: "text", nullable: true),
                    approval_level = table.Column<int>(type: "integer", nullable: true),
                    status_code = table.Column<string>(type: "text", nullable: true),
                    status_description = table.Column<string>(type: "text", nullable: true),
                    status_name = table.Column<string>(type: "text", nullable: true),
                    is_outside = table.Column<bool>(type: "boolean", nullable: true),
                    is_show = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPROVAL_CONFIG", x => x.id);
                    table.ForeignKey(
                        name: "FK_APPROVAL_CONFIG_MENU_item_id",
                        column: x => x.item_id,
                        principalTable: "MENU",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APPROVAL_CONFIG_item_id",
                table: "APPROVAL_CONFIG",
                column: "item_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APPROVAL_CONFIG");
        }
    }
}
