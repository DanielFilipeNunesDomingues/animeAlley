using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class FixSomeFieldIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personagens_Shows_ShowId",
                table: "Personagens");

            migrationBuilder.DropForeignKey(
                name: "FK_Shows_Stuidos_StudioFK",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Personagens_ShowId",
                table: "Personagens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stuidos",
                table: "Stuidos");

            migrationBuilder.DropColumn(
                name: "ShowId",
                table: "Personagens");

            migrationBuilder.RenameTable(
                name: "Stuidos",
                newName: "Studios");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Studios",
                table: "Studios",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PersonagemShow",
                columns: table => new
                {
                    PersonagensShowsId = table.Column<int>(type: "int", nullable: false),
                    ShowsPersonagemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonagemShow", x => new { x.PersonagensShowsId, x.ShowsPersonagemId });
                    table.ForeignKey(
                        name: "FK_PersonagemShow_Personagens_PersonagensShowsId",
                        column: x => x.PersonagensShowsId,
                        principalTable: "Personagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonagemShow_Shows_ShowsPersonagemId",
                        column: x => x.ShowsPersonagemId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b6e09b58-2a85-465e-9da7-2dac58b6631e", "AQAAAAIAAYagAAAAEDYgsAHoDNr9d9jOgIzjneDjne/7s9Pt0uSXJGfz9/pXPYidSlltB+ws7vWqvxP15w==", "835695f5-d754-450a-ae68-82365f58e2d6" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonagemShow_ShowsPersonagemId",
                table: "PersonagemShow",
                column: "ShowsPersonagemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_Studios_StudioFK",
                table: "Shows",
                column: "StudioFK",
                principalTable: "Studios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shows_Studios_StudioFK",
                table: "Shows");

            migrationBuilder.DropTable(
                name: "PersonagemShow");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Studios",
                table: "Studios");

            migrationBuilder.RenameTable(
                name: "Studios",
                newName: "Stuidos");

            migrationBuilder.AddColumn<int>(
                name: "ShowId",
                table: "Personagens",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stuidos",
                table: "Stuidos",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "685f54de-1ccd-480e-9ac2-b263cfd30548", "AQAAAAIAAYagAAAAELtWyBXsC5vCTKHsODv3vSzq3HXItJiEKlrWIYfZQidZvsJVvVDgpvloVhH5B1m2uQ==", "3cf9f25f-c7b2-4037-9972-28435683c18d" });

            migrationBuilder.CreateIndex(
                name: "IX_Personagens_ShowId",
                table: "Personagens",
                column: "ShowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personagens_Shows_ShowId",
                table: "Personagens",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_Stuidos_StudioFK",
                table: "Shows",
                column: "StudioFK",
                principalTable: "Stuidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
