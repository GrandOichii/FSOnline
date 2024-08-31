using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FSManager.Migrations
{
    /// <inheritdoc />
    public partial class MoveToPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardCollections",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardCollections", x => x.Key);
                });

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
                name: "Cards",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false),
                    CollectionKey = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Health = table.Column<int>(type: "integer", nullable: false),
                    Attack = table.Column<int>(type: "integer", nullable: false),
                    Evasion = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Script = table.Column<string>(type: "text", nullable: false),
                    SoulValue = table.Column<int>(type: "integer", nullable: false),
                    RewardsText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Cards_CardCollections_CollectionKey",
                        column: x => x.CollectionKey,
                        principalTable: "CardCollections",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardImages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Source = table.Column<string>(type: "text", nullable: false),
                    CardKey = table.Column<string>(type: "text", nullable: false),
                    CollectionKey = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CollectionKey",
                table: "Cards",
                column: "CollectionKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardImages");

            migrationBuilder.DropTable(
                name: "CardImageCollections");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "CardCollections");
        }
    }
}
