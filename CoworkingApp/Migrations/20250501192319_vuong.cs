using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoworkingApp.Migrations
{
    /// <inheritdoc />
    public partial class vuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "workspace_status_id_fkey",
                table: "workspace");

            migrationBuilder.DropIndex(
                name: "IX_workspace_status_id",
                table: "workspace");

            migrationBuilder.DropColumn(
                name: "status_id",
                table: "workspace");

            migrationBuilder.DropColumn(
                name: "customer_email",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "price",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "latitude",
                table: "coworking_center");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "coworking_center");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "workspace_status",
                newName: "workspace_status_id");

            migrationBuilder.RenameColumn(
                name: "valid_to",
                table: "workspace_pricing",
                newName: "valid_until");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "workspace_pricing",
                newName: "workspace_pricing_id");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "workspace_history",
                newName: "change_at");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "workspace_history",
                newName: "workspace_history_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "workspace",
                newName: "workspace_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "user_role",
                newName: "user_role_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "user",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "reservation",
                newName: "reservation_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "coworking_center",
                newName: "coworking_center_id");

            migrationBuilder.AlterColumn<decimal>(
                name: "price_per_hour",
                table: "workspace_pricing",
                type: "money",
                nullable: false,
                comment: "dollar\n",
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "workspace_pricing",
                type: "integer",
                nullable: true,
                comment: "if null then it was created by the database admin who has direct access to the database");

            migrationBuilder.AddColumn<int>(
                name: "reservation_id",
                table: "workspace_history",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_removed",
                table: "workspace",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_updated",
                table: "workspace",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "updated_by",
                table: "workspace",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<bool>(
                name: "is_removed",
                table: "user",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "user",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "refresh_token_expiry",
                table: "user",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "pricing_id",
                table: "reservation",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "reservation",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "customer_id",
                table: "reservation",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_cancelled",
                table: "reservation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "total_price",
                table: "reservation",
                type: "money",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "address_id",
                table: "coworking_center",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "is_removed",
                table: "coworking_center",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_updated",
                table: "coworking_center",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "updated_by",
                table: "coworking_center",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "country",
                columns: table => new
                {
                    country_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("country_pkey", x => x.country_id);
                });

            migrationBuilder.CreateTable(
                name: "city",
                columns: table => new
                {
                    city_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    country_id = table.Column<int>(type: "integer", nullable: false),
                    last_updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("city_pkey", x => x.city_id);
                    table.ForeignKey(
                        name: "city_country_id_fkey",
                        column: x => x.country_id,
                        principalTable: "country",
                        principalColumn: "country_id");
                });

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    address_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    street_address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    district = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    city_id = table.Column<int>(type: "integer", nullable: false),
                    postal_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    last_updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("address_pkey", x => x.address_id);
                    table.ForeignKey(
                        name: "address_city_id_fkey",
                        column: x => x.city_id,
                        principalTable: "city",
                        principalColumn: "city_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_workspace_pricing_created_by",
                table: "workspace_pricing",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_history_reservation_id",
                table: "workspace_history",
                column: "reservation_id");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_updated_by",
                table: "workspace",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_customer_id",
                table: "reservation",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_coworking_center_address_id",
                table: "coworking_center",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "IX_coworking_center_updated_by",
                table: "coworking_center",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_address_city_id",
                table: "address",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_city_country_id",
                table: "city",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "unique_country_name",
                table: "country",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "coworking_center_address_id_fkey",
                table: "coworking_center",
                column: "address_id",
                principalTable: "address",
                principalColumn: "address_id");

            migrationBuilder.AddForeignKey(
                name: "coworking_center_updated_by_fkey",
                table: "coworking_center",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "reservation_customer_id_fkey",
                table: "reservation",
                column: "customer_id",
                principalTable: "user",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "workspace_updated_by_fkey",
                table: "workspace",
                column: "updated_by",
                principalTable: "user",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "workspace_history_reservation_id_fkey",
                table: "workspace_history",
                column: "reservation_id",
                principalTable: "reservation",
                principalColumn: "reservation_id");

            migrationBuilder.AddForeignKey(
                name: "workspace_pricing_created_by_fkey",
                table: "workspace_pricing",
                column: "created_by",
                principalTable: "user",
                principalColumn: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "coworking_center_address_id_fkey",
                table: "coworking_center");

            migrationBuilder.DropForeignKey(
                name: "coworking_center_updated_by_fkey",
                table: "coworking_center");

            migrationBuilder.DropForeignKey(
                name: "reservation_customer_id_fkey",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "workspace_updated_by_fkey",
                table: "workspace");

            migrationBuilder.DropForeignKey(
                name: "workspace_history_reservation_id_fkey",
                table: "workspace_history");

            migrationBuilder.DropForeignKey(
                name: "workspace_pricing_created_by_fkey",
                table: "workspace_pricing");

            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "city");

            migrationBuilder.DropTable(
                name: "country");

            migrationBuilder.DropIndex(
                name: "IX_workspace_pricing_created_by",
                table: "workspace_pricing");

            migrationBuilder.DropIndex(
                name: "IX_workspace_history_reservation_id",
                table: "workspace_history");

            migrationBuilder.DropIndex(
                name: "IX_workspace_updated_by",
                table: "workspace");

            migrationBuilder.DropIndex(
                name: "IX_reservation_customer_id",
                table: "reservation");

            migrationBuilder.DropIndex(
                name: "IX_coworking_center_address_id",
                table: "coworking_center");

            migrationBuilder.DropIndex(
                name: "IX_coworking_center_updated_by",
                table: "coworking_center");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "workspace_pricing");

            migrationBuilder.DropColumn(
                name: "reservation_id",
                table: "workspace_history");

            migrationBuilder.DropColumn(
                name: "last_updated",
                table: "workspace");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "workspace");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "user");

            migrationBuilder.DropColumn(
                name: "is_removed",
                table: "user");

            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "user");

            migrationBuilder.DropColumn(
                name: "refresh_token_expiry",
                table: "user");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "is_cancelled",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "total_price",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "address_id",
                table: "coworking_center");

            migrationBuilder.DropColumn(
                name: "is_removed",
                table: "coworking_center");

            migrationBuilder.DropColumn(
                name: "last_updated",
                table: "coworking_center");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "coworking_center");

            migrationBuilder.RenameColumn(
                name: "workspace_status_id",
                table: "workspace_status",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "valid_until",
                table: "workspace_pricing",
                newName: "valid_to");

            migrationBuilder.RenameColumn(
                name: "workspace_pricing_id",
                table: "workspace_pricing",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "change_at",
                table: "workspace_history",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "workspace_history_id",
                table: "workspace_history",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "workspace_id",
                table: "workspace",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "user_role_id",
                table: "user_role",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "user",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "reservation_id",
                table: "reservation",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "coworking_center_id",
                table: "coworking_center",
                newName: "id");

            migrationBuilder.AlterColumn<decimal>(
                name: "price_per_hour",
                table: "workspace_pricing",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldComment: "dollar\n");

            migrationBuilder.AlterColumn<bool>(
                name: "is_removed",
                table: "workspace",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "status_id",
                table: "workspace",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "pricing_id",
                table: "reservation",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "customer_email",
                table: "reservation",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "price",
                table: "reservation",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "latitude",
                table: "coworking_center",
                type: "numeric(9,6)",
                precision: 9,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "longitude",
                table: "coworking_center",
                type: "numeric(9,6)",
                precision: 9,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_workspace_status_id",
                table: "workspace",
                column: "status_id");

            migrationBuilder.AddForeignKey(
                name: "workspace_status_id_fkey",
                table: "workspace",
                column: "status_id",
                principalTable: "workspace_status",
                principalColumn: "id");
        }
    }
}
