using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bangboo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    avatar = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "auths",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    access_token = table.Column<string>(type: "text", nullable: false),
                    refresh_token = table.Column<string>(type: "text", nullable: false),
                    token_type = table.Column<string>(type: "text", nullable: false),
                    expires_in = table.Column<DateOnly>(type: "date", nullable: false),
                    scope = table.Column<string>(type: "text", nullable: false),
                    fk_user = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auths", x => x.id);
                    table.ForeignKey(
                        name: "FK_auths_users_fk_user",
                        column: x => x.fk_user,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guilds",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    icon = table.Column<string>(type: "text", nullable: true),
                    banner = table.Column<string>(type: "text", nullable: true),
                    xp_per_message = table.Column<long>(type: "bigint", nullable: false),
                    logs_channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    fk_owner = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guilds", x => x.id);
                    table.ForeignKey(
                        name: "FK_guilds_users_fk_owner",
                        column: x => x.fk_owner,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    user_agent = table.Column<string>(type: "text", nullable: false),
                    language = table.Column<string>(type: "text", nullable: false),
                    fk_auth_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_sessions_auths_fk_auth_id",
                        column: x => x.fk_auth_id,
                        principalTable: "auths",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "member_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    on_join = table.Column<bool>(type: "boolean", nullable: false),
                    on_leave = table.Column<bool>(type: "boolean", nullable: false),
                    on_ban = table.Column<bool>(type: "boolean", nullable: false),
                    on_boost = table.Column<bool>(type: "boolean", nullable: false),
                    on_mute = table.Column<bool>(type: "boolean", nullable: false),
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
                    xp = table.Column<long>(type: "bigint", nullable: false),
                    fk_guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GuildModelId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    fk_user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_members_guilds_GuildModelId",
                        column: x => x.GuildModelId,
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
                    should_ban = table.Column<bool>(type: "boolean", nullable: false),
                    message_dm = table.Column<string>(type: "text", nullable: true),
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
                name: "IX_auths_fk_user",
                table: "auths",
                column: "fk_user",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_guilds_fk_owner",
                table: "guilds",
                column: "fk_owner");

            migrationBuilder.CreateIndex(
                name: "IX_member_events_fk_guild_id",
                table: "member_events",
                column: "fk_guild_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_members_fk_user_id",
                table: "members",
                column: "fk_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_members_GuildModelId",
                table: "members",
                column: "GuildModelId");

            migrationBuilder.CreateIndex(
                name: "IX_mousetraps_fk_guild_id",
                table: "mousetraps",
                column: "fk_guild_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sessions_fk_auth_id",
                table: "sessions",
                column: "fk_auth_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "member_events");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "mousetraps");

            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "guilds");

            migrationBuilder.DropTable(
                name: "auths");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
