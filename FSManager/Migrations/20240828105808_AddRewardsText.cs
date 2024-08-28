using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSManager.Migrations
{
    /// <inheritdoc />
    public partial class AddRewardsText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RewardsText",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RewardsText",
                table: "Cards");
        }
    }
}
