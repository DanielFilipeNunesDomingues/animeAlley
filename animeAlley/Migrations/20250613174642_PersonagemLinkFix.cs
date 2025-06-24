using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class PersonagemLinkFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonagemShow");

            migrationBuilder.AddColumn<int>(
                name: "ShowFK",
                table: "Personagens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "24e67d77-3f65-47cf-8e5c-ff3e5ff23e36", "AQAAAAIAAYagAAAAEGfNdkgg8F36+X2cejIwws25lJJhdWQax7JCKSSIzHWA9uQbg/o7vknn0VewDcFrRA==", "e9b3c518-d170-42b9-a53c-2e50b9029108" });

            migrationBuilder.CreateIndex(
                name: "IX_Personagens_ShowFK",
                table: "Personagens",
                column: "ShowFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Personagens_Shows_ShowFK",
                table: "Personagens",
                column: "ShowFK",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personagens_Shows_ShowFK",
                table: "Personagens");

            migrationBuilder.DropIndex(
                name: "IX_Personagens_ShowFK",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "ShowFK",
                table: "Personagens");

            migrationBuilder.CreateTable(
                name: "PersonagemShow",
                columns: table => new
                {
                    PersonagensShowsId = table.Column<int>(type: "int", nullable: false),
                    ShowsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonagemShow", x => new { x.PersonagensShowsId, x.ShowsId });
                    table.ForeignKey(
                        name: "FK_PersonagemShow_Personagens_PersonagensShowsId",
                        column: x => x.PersonagensShowsId,
                        principalTable: "Personagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonagemShow_Shows_ShowsId",
                        column: x => x.ShowsId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "73a333fc-0288-48f3-ad77-6e3505af0c17", "AQAAAAIAAYagAAAAEIxOUue5fX3WqAFcewt/LHmiYwKzkwLsbKQIZVIo9KuqYipd4Su5kQ/tT+ewr3sWTg==", "aa45054b-cc1b-49a6-a274-e6697bfb5c5c" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonagemShow_ShowsId",
                table: "PersonagemShow",
                column: "ShowsId");
        }
    }
}
