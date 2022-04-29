using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KitanoUserService.API.Migrations
{
    public partial class ApprovalFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APPROVAL_FUNCTION_STATUS",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<int>(type: "integer", nullable: true),
                    function_name = table.Column<string>(type: "text", nullable: true),
                    function_code = table.Column<string>(type: "text", nullable: true),
                    approver = table.Column<int>(type: "integer", nullable: true),
                    status_code = table.Column<string>(type: "text", nullable: true),
                    status_name = table.Column<string>(type: "text", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    approval_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    path = table.Column<string>(type: "text", nullable: true),
                    file_type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPROVAL_FUNCTION_STATUS", x => x.id);
                    table.ForeignKey(
                        name: "FK_APPROVAL_FUNCTION_STATUS_USERS_approver",
                        column: x => x.approver,
                        principalTable: "USERS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APPROVAL_FUNCTION_STATUS_approver",
                table: "APPROVAL_FUNCTION_STATUS",
                column: "approver");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APPROVAL_FUNCTION_STATUS");
        }
    }
}
