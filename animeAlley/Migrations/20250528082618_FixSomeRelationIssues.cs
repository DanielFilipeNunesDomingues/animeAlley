using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class FixSomeRelationIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Generos_Shows_ShowId",
                table: "Generos");

            migrationBuilder.DropForeignKey(
                name: "FK_Obras_Autores_AutorId",
                table: "Obras");

            migrationBuilder.DropForeignKey(
                name: "FK_Obras_Personagens_PersonagemId",
                table: "Obras");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonagemShow_Shows_ShowsPersonagemId",
                table: "PersonagemShow");

            migrationBuilder.DropIndex(
                name: "IX_Obras_AutorId",
                table: "Obras");

            migrationBuilder.DropIndex(
                name: "IX_Obras_PersonagemId",
                table: "Obras");

            migrationBuilder.DropIndex(
                name: "IX_Generos_ShowId",
                table: "Generos");

            migrationBuilder.DropColumn(
                name: "AutorId",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "PersonagemId",
                table: "Obras");

            migrationBuilder.DropColumn(
                name: "ShowId",
                table: "Generos");

            migrationBuilder.RenameColumn(
                name: "ShowsPersonagemId",
                table: "PersonagemShow",
                newName: "ShowsId");

            migrationBuilder.RenameIndex(
                name: "IX_PersonagemShow_ShowsPersonagemId",
                table: "PersonagemShow",
                newName: "IX_PersonagemShow_ShowsId");

            migrationBuilder.CreateTable(
                name: "GeneroShow",
                columns: table => new
                {
                    GenerosShowsId = table.Column<int>(type: "int", nullable: false),
                    ShowsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneroShow", x => new { x.GenerosShowsId, x.ShowsId });
                    table.ForeignKey(
                        name: "FK_GeneroShow_Generos_GenerosShowsId",
                        column: x => x.GenerosShowsId,
                        principalTable: "Generos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneroShow_Shows_ShowsId",
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
                values: new object[] { "f4a901a9-bfa3-48c7-b95d-106af1381303", "AQAAAAIAAYagAAAAEGNhvklKz4H52MQadn6/mAtcEsEyhNwbAxSWynnu0tnfxsKTNObES+WnGViDPgA1AQ==", "3b23b22e-7ee4-494e-bd28-58b0caf2f5ba" });

            migrationBuilder.CreateIndex(
                name: "IX_GeneroShow_ShowsId",
                table: "GeneroShow",
                column: "ShowsId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonagemShow_Shows_ShowsId",
                table: "PersonagemShow",
                column: "ShowsId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonagemShow_Shows_ShowsId",
                table: "PersonagemShow");

            migrationBuilder.DropTable(
                name: "GeneroShow");

            migrationBuilder.RenameColumn(
                name: "ShowsId",
                table: "PersonagemShow",
                newName: "ShowsPersonagemId");

            migrationBuilder.RenameIndex(
                name: "IX_PersonagemShow_ShowsId",
                table: "PersonagemShow",
                newName: "IX_PersonagemShow_ShowsPersonagemId");

            migrationBuilder.AddColumn<int>(
                name: "AutorId",
                table: "Obras",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonagemId",
                table: "Obras",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShowId",
                table: "Generos",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b6e09b58-2a85-465e-9da7-2dac58b6631e", "AQAAAAIAAYagAAAAEDYgsAHoDNr9d9jOgIzjneDjne/7s9Pt0uSXJGfz9/pXPYidSlltB+ws7vWqvxP15w==", "835695f5-d754-450a-ae68-82365f58e2d6" });

            migrationBuilder.CreateIndex(
                name: "IX_Obras_AutorId",
                table: "Obras",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_PersonagemId",
                table: "Obras",
                column: "PersonagemId");

            migrationBuilder.CreateIndex(
                name: "IX_Generos_ShowId",
                table: "Generos",
                column: "ShowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Generos_Shows_ShowId",
                table: "Generos",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Obras_Autores_AutorId",
                table: "Obras",
                column: "AutorId",
                principalTable: "Autores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Obras_Personagens_PersonagemId",
                table: "Obras",
                column: "PersonagemId",
                principalTable: "Personagens",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonagemShow_Shows_ShowsPersonagemId",
                table: "PersonagemShow",
                column: "ShowsPersonagemId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
