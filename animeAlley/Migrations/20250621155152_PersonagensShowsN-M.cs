using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class PersonagensShowsNM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    PersonagensId = table.Column<int>(type: "int", nullable: false),
                    ShowsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonagemShow", x => new { x.PersonagensId, x.ShowsId });
                    table.ForeignKey(
                        name: "FK_PersonagemShow_Personagens_PersonagensId",
                        column: x => x.PersonagensId,
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
                values: new object[] { "a8100233-e3c1-44c5-90a7-b983d7811053", "AQAAAAIAAYagAAAAEM+sePmYT7w4+4G7kiypZCERXaLWWSznNTg0ly/yK2eOt/Waiem66u1g/7w3GwcZ+w==", "8144c093-b212-4656-beff-bd296802a83f" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonagemShow_ShowsId",
                table: "PersonagemShow",
                column: "ShowsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                values: new object[] { "5fcae202-32cc-42be-af80-feeb18cbadf7", "AQAAAAIAAYagAAAAEK4ErIBl2crGDE+UHmoYC9TyPmw0HORuJrTHSKp58n5duhZbFH3TMQofbp24wlV/0w==", "d6238a76-87f1-438b-8be3-a88bc4930c5a" });

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
    }
}
