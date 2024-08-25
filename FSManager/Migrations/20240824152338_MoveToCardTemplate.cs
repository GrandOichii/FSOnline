using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSManager.Migrations
{
    /// <inheritdoc />
    public partial class MoveToCardTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Attack",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Evasion",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Health",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Script",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SoulValue",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attack",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Evasion",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Health",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Script",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "SoulValue",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Cards");
        }
    }
}
