using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bangboo.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bot_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    latency_ms = table.Column<int>(type: "integer", nullable: false),
                    guild_count = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bot_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "data_protection_keys",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    friendly_name = table.Column<string>(type: "text", nullable: true),
                    xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_data_protection_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    global_name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    avatar = table.Column<string>(type: "text", nullable: true),
                    last_login_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "guilds",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    icon = table.Column<string>(type: "text", nullable: true),
                    banner = table.Column<string>(type: "text", nullable: true),
                    xp_per_message = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    logs_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    fk_owner_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guilds", x => x.id);
                    table.ForeignKey(
                        name: "FK_guilds_users_fk_owner_id",
                        column: x => x.fk_owner_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "member_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    on_join = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    on_leave = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    on_ban = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    on_boost = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    on_mute = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    system_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    boost_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    mod_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    fk_guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_member_events_guilds_fk_guild_id",
                        column: x => x.fk_guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    xp = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    fk_guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    fk_user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_members_guilds_fk_guild_id",
                        column: x => x.fk_guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_members_users_fk_user_id",
                        column: x => x.fk_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mousetraps",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    should_ban = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    message_dm = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    fk_guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mousetraps", x => x.id);
                    table.ForeignKey(
                        name: "FK_mousetraps_guilds_fk_guild_id",
                        column: x => x.fk_guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_guilds_fk_owner_id",
                table: "guilds",
                column: "fk_owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_events_fk_guild_id",
                table: "member_events",
                column: "fk_guild_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_members_fk_guild_id_fk_user_id",
                table: "members",
                columns: new[] { "fk_guild_id", "fk_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_members_fk_user_id",
                table: "members",
                column: "fk_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mousetraps_fk_guild_id",
                table: "mousetraps",
                column: "fk_guild_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bot_status");

            migrationBuilder.DropTable(
                name: "data_protection_keys");

            migrationBuilder.DropTable(
                name: "member_events");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "mousetraps");

            migrationBuilder.DropTable(
                name: "guilds");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
