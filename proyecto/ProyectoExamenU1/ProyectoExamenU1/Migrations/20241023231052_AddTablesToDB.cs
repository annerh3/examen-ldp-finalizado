using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolicitudPermiso.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesToDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "permission_types",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    max_days = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permission_requests",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    employee_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    permission_type_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    motive = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    permission_status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_permission_requests_permission_types_permission_type_id",
                        column: x => x.permission_type_id,
                        principalSchema: "dbo",
                        principalTable: "permission_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_permission_requests_users_employee_id",
                        column: x => x.employee_id,
                        principalSchema: "security",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_permission_requests_employee_id",
                schema: "dbo",
                table: "permission_requests",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_permission_requests_permission_type_id",
                schema: "dbo",
                table: "permission_requests",
                column: "permission_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "permission_requests",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "permission_types",
                schema: "dbo");
        }
    }
}
