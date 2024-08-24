using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSManager.Migrations
{
    /// <inheritdoc />
    public partial class AddCardCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CollectionKey",
                table: "Cards",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CardCollections",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardCollections", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CollectionKey",
                table: "Cards",
                column: "CollectionKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_CardCollections_CollectionKey",
                table: "Cards",
                column: "CollectionKey",
                principalTable: "CardCollections",
                principalColumn: "Key",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_CardCollections_CollectionKey",
                table: "Cards");

            migrationBuilder.DropTable(
                name: "CardCollections");

            migrationBuilder.DropIndex(
                name: "IX_Cards_CollectionKey",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CollectionKey",
                table: "Cards");
        }
    }
}
