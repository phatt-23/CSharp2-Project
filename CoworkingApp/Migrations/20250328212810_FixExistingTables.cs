using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoworkingApp.Migrations
{
    /// <inheritdoc />
    public partial class FixExistingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "workspace_status",
                type: "character varying",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "workspace",
                type: "character varying",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "is_removed",
                table: "workspace",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "workspace",
                type: "character varying",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "coworking_center",
                type: "character varying",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "workspace_status");

            migrationBuilder.DropColumn(
                name: "description",
                table: "workspace");

            migrationBuilder.DropColumn(
                name: "is_removed",
                table: "workspace");

            migrationBuilder.DropColumn(
                name: "name",
                table: "workspace");

            migrationBuilder.DropColumn(
                name: "description",
                table: "coworking_center");
        }
    }
}
