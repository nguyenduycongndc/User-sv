using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KitanoUserService.API.Migrations
{
    public partial class catauditrequest1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatAuditRequest",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    typename = table.Column<string>(type: "text", nullable: true),
                    typecode = table.Column<string>(type: "text", nullable: true),
                    createdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modifiedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    createdBy = table.Column<int>(type: "integer", nullable: true),
                    modifiedBy = table.Column<int>(type: "integer", nullable: true),
                    deletedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatAuditRequest", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatAuditRequest");
        }
    }
}
