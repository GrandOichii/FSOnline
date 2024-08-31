using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FSManager.Migrations
{
    /// <inheritdoc />
    public partial class AddCardRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardRelations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RelationType = table.Column<int>(type: "integer", nullable: false),
                    RelatedToKey = table.Column<string>(type: "text", nullable: true),
                    RelatedCardKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardRelations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CardRelations_Cards_RelatedCardKey",
                        column: x => x.RelatedCardKey,
                        principalTable: "Cards",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardRelations_Cards_RelatedToKey",
                        column: x => x.RelatedToKey,
                        principalTable: "Cards",
                        principalColumn: "Key");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardRelations_RelatedCardKey",
                table: "CardRelations",
                column: "RelatedCardKey");

            migrationBuilder.CreateIndex(
                name: "IX_CardRelations_RelatedToKey",
                table: "CardRelations",
                column: "RelatedToKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardRelations");
        }
    }
}
