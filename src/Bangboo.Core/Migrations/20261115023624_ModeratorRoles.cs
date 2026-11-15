using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bangboo.Core.Migrations
{
    /// <inheritdoc />
    public partial class ModeratorRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "moderator_roles",
                columns: table => new
                {
                    fk_guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    role_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moderator_roles", x => new { x.fk_guild_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_moderator_roles_guilds_fk_guild_id",
                        column: x => x.fk_guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "moderator_roles");
        }
    }
}
