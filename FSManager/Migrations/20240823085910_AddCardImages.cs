using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSManager.Migrations
{
    /// <inheritdoc />
    public partial class AddCardImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardImageCollections",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardImageCollections", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CardImages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CollectionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardImages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CardImages_CardImageCollections_CollectionID",
                        column: x => x.CollectionID,
                        principalTable: "CardImageCollections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardImages_Cards_CardKey",
                        column: x => x.CardKey,
                        principalTable: "Cards",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardImages_CardKey",
                table: "CardImages",
                column: "CardKey");

            migrationBuilder.CreateIndex(
                name: "IX_CardImages_CollectionID",
                table: "CardImages",
                column: "CollectionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardImages");

            migrationBuilder.DropTable(
                name: "CardImageCollections");
        }
    }
}
