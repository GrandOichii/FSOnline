using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FSManager.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCardImageCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardImages");

            migrationBuilder.DropTable(
                name: "CardImageCollections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardImageCollections",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardImageCollections", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "CardImages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CardKey = table.Column<string>(type: "text", nullable: false),
                    CollectionKey = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardImages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CardImages_CardImageCollections_CollectionKey",
                        column: x => x.CollectionKey,
                        principalTable: "CardImageCollections",
                        principalColumn: "Key",
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
                name: "IX_CardImages_CollectionKey",
                table: "CardImages",
                column: "CollectionKey");
        }
    }
}
