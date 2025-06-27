using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class FixShowDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeneroShow_Generos_GenerosShowsId",
                table: "GeneroShow");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneroShow_Shows_ShowsId",
                table: "GeneroShow");

            migrationBuilder.DropForeignKey(
                name: "FK_ListaShows_Shows_ShowId",
                table: "ListaShows");

            migrationBuilder.DropForeignKey(
                name: "FK_ListaShows_Shows_ShowId1",
                table: "ListaShows");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonagemShow_Personagens_PersonagensId",
                table: "PersonagemShow");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonagemShow_Shows_ShowsId",
                table: "PersonagemShow");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonagemShow",
                table: "PersonagemShow");

            migrationBuilder.DropIndex(
                name: "IX_ListaShows_ShowId1",
                table: "ListaShows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GeneroShow",
                table: "GeneroShow");

            migrationBuilder.DropColumn(
                name: "ShowId1",
                table: "ListaShows");

            migrationBuilder.RenameTable(
                name: "GeneroShow",
                newName: "ShowGeneros");

            migrationBuilder.RenameColumn(
                name: "ShowsId",
                table: "PersonagemShow",
                newName: "ShowId1");

            migrationBuilder.RenameColumn(
                name: "PersonagensId",
                table: "PersonagemShow",
                newName: "PersonagemId1");

            migrationBuilder.RenameIndex(
                name: "IX_PersonagemShow_ShowsId",
                table: "PersonagemShow",
                newName: "IX_PersonagemShow_ShowId1");

            migrationBuilder.RenameIndex(
                name: "IX_GeneroShow_ShowsId",
                table: "ShowGeneros",
                newName: "IX_ShowGeneros_ShowsId");

            migrationBuilder.AddColumn<int>(
                name: "PersonagemId",
                table: "PersonagemShow",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<int>(
                name: "ShowId",
                table: "PersonagemShow",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<string>(
                name: "PapelNoShow",
                table: "PersonagemShow",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrimeiraAparicao",
                table: "PersonagemShow",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonagemShow",
                table: "PersonagemShow",
                columns: new[] { "PersonagemId", "ShowId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShowGeneros",
                table: "ShowGeneros",
                columns: new[] { "GenerosShowsId", "ShowsId" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "da44a269-7867-460c-84a9-da020308eae4", "AQAAAAIAAYagAAAAEP22nUQ+HTELpBU0vTxOod0Hs3dtPidMSgFWCvwAI9fU5k6/4EMYmpkS1KtF5QyuUw==", "f96bc047-4b0f-4952-b214-488181294471" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonagemShow_PersonagemId1",
                table: "PersonagemShow",
                column: "PersonagemId1");

            migrationBuilder.CreateIndex(
                name: "IX_PersonagemShow_ShowId",
                table: "PersonagemShow",
                column: "ShowId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListaShows_Shows_ShowId",
                table: "ListaShows",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonagemShow_Personagens_PersonagemId",
                table: "PersonagemShow",
                column: "PersonagemId",
                principalTable: "Personagens",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonagemShow_Personagens_PersonagemId1",
                table: "PersonagemShow",
                column: "PersonagemId1",
                principalTable: "Personagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonagemShow_Shows_ShowId",
                table: "PersonagemShow",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonagemShow_Shows_ShowId1",
                table: "PersonagemShow",
                column: "ShowId1",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowGeneros_Generos_GenerosShowsId",
                table: "ShowGeneros",
                column: "GenerosShowsId",
                principalTable: "Generos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowGeneros_Shows_ShowsId",
                table: "ShowGeneros",
                column: "ShowsId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListaShows_Shows_ShowId",
                table: "ListaShows");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonagemShow_Personagens_PersonagemId",
                table: "PersonagemShow");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonagemShow_Personagens_PersonagemId1",
                table: "PersonagemShow");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonagemShow_Shows_ShowId",
                table: "PersonagemShow");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonagemShow_Shows_ShowId1",
                table: "PersonagemShow");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowGeneros_Generos_GenerosShowsId",
                table: "ShowGeneros");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowGeneros_Shows_ShowsId",
                table: "ShowGeneros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonagemShow",
                table: "PersonagemShow");

            migrationBuilder.DropIndex(
                name: "IX_PersonagemShow_PersonagemId1",
                table: "PersonagemShow");

            migrationBuilder.DropIndex(
                name: "IX_PersonagemShow_ShowId",
                table: "PersonagemShow");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShowGeneros",
                table: "ShowGeneros");

            migrationBuilder.DropColumn(
                name: "PersonagemId",
                table: "PersonagemShow");

            migrationBuilder.DropColumn(
                name: "ShowId",
                table: "PersonagemShow");

            migrationBuilder.DropColumn(
                name: "PapelNoShow",
                table: "PersonagemShow");

            migrationBuilder.DropColumn(
                name: "PrimeiraAparicao",
                table: "PersonagemShow");

            migrationBuilder.RenameTable(
                name: "ShowGeneros",
                newName: "GeneroShow");

            migrationBuilder.RenameColumn(
                name: "ShowId1",
                table: "PersonagemShow",
                newName: "ShowsId");

            migrationBuilder.RenameColumn(
                name: "PersonagemId1",
                table: "PersonagemShow",
                newName: "PersonagensId");

            migrationBuilder.RenameIndex(
                name: "IX_PersonagemShow_ShowId1",
                table: "PersonagemShow",
                newName: "IX_PersonagemShow_ShowsId");

            migrationBuilder.RenameIndex(
                name: "IX_ShowGeneros_ShowsId",
                table: "GeneroShow",
                newName: "IX_GeneroShow_ShowsId");

            migrationBuilder.AddColumn<int>(
                name: "ShowId1",
                table: "ListaShows",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonagemShow",
                table: "PersonagemShow",
                columns: new[] { "PersonagensId", "ShowsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GeneroShow",
                table: "GeneroShow",
                columns: new[] { "GenerosShowsId", "ShowsId" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5467dec3-7cbb-4783-ae99-e2d2d2b038a2", "AQAAAAIAAYagAAAAEGQMOQvcvxpq89d3DOHBbocs/pTQsPWr1g0L5lC4ibklKfk7a3lUkJSIUptJQTIZ3g==", "50fb603e-d32c-4802-a49c-3a0d7daeef5b" });

            migrationBuilder.CreateIndex(
                name: "IX_ListaShows_ShowId1",
                table: "ListaShows",
                column: "ShowId1");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneroShow_Generos_GenerosShowsId",
                table: "GeneroShow",
                column: "GenerosShowsId",
                principalTable: "Generos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GeneroShow_Shows_ShowsId",
                table: "GeneroShow",
                column: "ShowsId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ListaShows_Shows_ShowId",
                table: "ListaShows",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListaShows_Shows_ShowId1",
                table: "ListaShows",
                column: "ShowId1",
                principalTable: "Shows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonagemShow_Personagens_PersonagensId",
                table: "PersonagemShow",
                column: "PersonagensId",
                principalTable: "Personagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonagemShow_Shows_ShowsId",
                table: "PersonagemShow",
                column: "ShowsId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
