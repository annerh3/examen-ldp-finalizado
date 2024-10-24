using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolicitudPermiso.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "permission_status",
                schema: "dbo",
                table: "permission_requests",
                newName: "resolution_detail");

            migrationBuilder.AddColumn<Guid>(
                name: "permission_status_id",
                schema: "dbo",
                table: "permission_requests",
                type: "uniqueidentifier",
                maxLength: 50,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "permission_request_status",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    state_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    state_description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_request_status", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_permission_requests_permission_status_id",
                schema: "dbo",
                table: "permission_requests",
                column: "permission_status_id");

            migrationBuilder.AddForeignKey(
                name: "FK_permission_requests_permission_request_status_permission_status_id",
                schema: "dbo",
                table: "permission_requests",
                column: "permission_status_id",
                principalSchema: "dbo",
                principalTable: "permission_request_status",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_permission_requests_permission_request_status_permission_status_id",
                schema: "dbo",
                table: "permission_requests");

            migrationBuilder.DropTable(
                name: "permission_request_status",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_permission_requests_permission_status_id",
                schema: "dbo",
                table: "permission_requests");

            migrationBuilder.DropColumn(
                name: "permission_status_id",
                schema: "dbo",
                table: "permission_requests");

            migrationBuilder.RenameColumn(
                name: "resolution_detail",
                schema: "dbo",
                table: "permission_requests",
                newName: "permission_status");
        }
    }
}
