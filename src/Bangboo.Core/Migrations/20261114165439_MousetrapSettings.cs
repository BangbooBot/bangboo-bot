using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bangboo.Core.Migrations
{
    /// <inheritdoc />
    public partial class MousetrapSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // should_ban (antigo) não tem equivalente: a punição agora é a coluna "action".
            migrationBuilder.DropColumn(
                name: "should_ban",
                table: "mousetraps");

            migrationBuilder.AddColumn<bool>(
                name: "send_dm",
                table: "mousetraps",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "message_dm",
                table: "mousetraps",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "channel_id",
                table: "mousetraps",
                type: "numeric(20,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AddColumn<string>(
                name: "action",
                table: "mousetraps",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "Ban");

            migrationBuilder.AddColumn<bool>(
                name: "enabled",
                table: "mousetraps",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "min_tenure_days",
                table: "mousetraps",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "action",
                table: "mousetraps");

            migrationBuilder.DropColumn(
                name: "enabled",
                table: "mousetraps");

            migrationBuilder.DropColumn(
                name: "min_tenure_days",
                table: "mousetraps");

            migrationBuilder.DropColumn(
                name: "send_dm",
                table: "mousetraps");

            migrationBuilder.AddColumn<bool>(
                name: "should_ban",
                table: "mousetraps",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "message_dm",
                table: "mousetraps",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "channel_id",
                table: "mousetraps",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)",
                oldNullable: true);
        }
    }
}
