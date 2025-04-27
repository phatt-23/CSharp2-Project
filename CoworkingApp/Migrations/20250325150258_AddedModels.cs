using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoworkingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "coworking_center",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("coworking_center_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_role_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workspace_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("workspace_status_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "character varying", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_pkey", x => x.id);
                    table.ForeignKey(
                        name: "user_role_id_fkey",
                        column: x => x.role_id,
                        principalTable: "user_role",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "workspace",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    coworking_center_id = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("workspace_pkey", x => x.id);
                    table.ForeignKey(
                        name: "workspace_coworking_center_id_fkey",
                        column: x => x.coworking_center_id,
                        principalTable: "coworking_center");
                });

            migrationBuilder.CreateTable(
                name: "workspace_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    workspace_id = table.Column<int>(type: "integer", nullable: false),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("workspace_history_pkey", x => x.id);
                    table.ForeignKey(
                        name: "workspace_history_status_id_fkey",
                        column: x => x.status_id,
                        principalTable: "workspace_status",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "workspace_history_workspace_id_fkey",
                        column: x => x.workspace_id,
                        principalTable: "workspace",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "workspace_pricing",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    workspace_id = table.Column<int>(type: "integer", nullable: false),
                    price_per_hour = table.Column<decimal>(type: "money", nullable: false),
                    valid_from = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    valid_to = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("workspace_pricing_pkey", x => x.id);
                    table.ForeignKey(
                        name: "workspace_pricing_workspace_id_fkey",
                        column: x => x.workspace_id,
                        principalTable: "workspace",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "reservation",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    workspace_id = table.Column<int>(type: "integer", nullable: false),
                    customer_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    price = table.Column<decimal>(type: "money", nullable: false),
                    pricing_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reservation_pkey", x => x.id);
                    table.ForeignKey(
                        name: "reservation_pricing_id_fkey",
                        column: x => x.pricing_id,
                        principalTable: "workspace_pricing",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "reservation_workspace_id_fkey",
                        column: x => x.workspace_id,
                        principalTable: "workspace",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_reservation_pricing_id",
                table: "reservation",
                column: "pricing_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_workspace_id",
                table: "reservation",
                column: "workspace_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_id",
                table: "user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "user_email_key",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "user_role_name_key",
                table: "user_role",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_workspace_coworking_center_id",
                table: "workspace",
                column: "coworking_center_id");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_status_id",
                table: "workspace",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_history_status_id",
                table: "workspace_history",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_history_workspace_id",
                table: "workspace_history",
                column: "workspace_id");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_pricing_workspace_id",
                table: "workspace_pricing",
                column: "workspace_id");

            migrationBuilder.CreateIndex(
                name: "workspace_status_name_key",
                table: "workspace_status",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservation");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "workspace_history");

            migrationBuilder.DropTable(
                name: "workspace_pricing");

            migrationBuilder.DropTable(
                name: "user_role");

            migrationBuilder.DropTable(
                name: "workspace");

            migrationBuilder.DropTable(
                name: "coworking_center");

            migrationBuilder.DropTable(
                name: "workspace_status");
        }
    }
}
