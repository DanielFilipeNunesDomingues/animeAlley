using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class FixPersonagemNotLinkingToShows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShowGeneros",
                table: "ShowGeneros");

            migrationBuilder.DropIndex(
                name: "IX_ShowGeneros_ShowsId",
                table: "ShowGeneros");

            migrationBuilder.DropIndex(
                name: "IX_PersonagemShow_PersonagemId1",
                table: "PersonagemShow");

            migrationBuilder.DropIndex(
                name: "IX_PersonagemShow_ShowId1",
                table: "PersonagemShow");

            migrationBuilder.DropColumn(
                name: "PapelNoShow",
                table: "PersonagemShow");

            migrationBuilder.DropColumn(
                name: "PersonagemId1",
                table: "PersonagemShow");

            migrationBuilder.DropColumn(
                name: "PrimeiraAparicao",
                table: "PersonagemShow");

            migrationBuilder.DropColumn(
                name: "ShowId1",
                table: "PersonagemShow");

            migrationBuilder.RenameColumn(
                name: "GenerosShowsId",
                table: "ShowGeneros",
                newName: "GenerosId");

            migrationBuilder.AlterColumn<int>(
                name: "ShowId",
                table: "PersonagemShow",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "PersonagemId",
                table: "PersonagemShow",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShowGeneros",
                table: "ShowGeneros",
                columns: new[] { "ShowsId", "GenerosId" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "74b44e77-03e4-4c2e-855d-4bd927bbf422", "AQAAAAIAAYagAAAAEAeStEUI1boIE8T/4bpmqOaPRIYCbSFOmivzccieoGnShvX3UveaVNM92r+TCK6gZg==", "12e14d8d-f070-40f5-b4b5-007500b98eba" });

            migrationBuilder.CreateIndex(
                name: "IX_ShowGeneros_GenerosId",
                table: "ShowGeneros",
                column: "GenerosId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonagemShow_Personagens_PersonagemId",
                table: "PersonagemShow",
                column: "PersonagemId",
                principalTable: "Personagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonagemShow_Shows_ShowId",
                table: "PersonagemShow",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShowGeneros_Generos_GenerosId",
                table: "ShowGeneros",
                column: "GenerosId",
                principalTable: "Generos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonagemShow_Personagens_PersonagemId",
                table: "PersonagemShow");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonagemShow_Shows_ShowId",
                table: "PersonagemShow");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowGeneros_Generos_GenerosId",
                table: "ShowGeneros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShowGeneros",
                table: "ShowGeneros");

            migrationBuilder.DropIndex(
                name: "IX_ShowGeneros_GenerosId",
                table: "ShowGeneros");

            migrationBuilder.RenameColumn(
                name: "GenerosId",
                table: "ShowGeneros",
                newName: "GenerosShowsId");

            migrationBuilder.AlterColumn<int>(
                name: "ShowId",
                table: "PersonagemShow",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "PersonagemId",
                table: "PersonagemShow",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<string>(
                name: "PapelNoShow",
                table: "PersonagemShow",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonagemId1",
                table: "PersonagemShow",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrimeiraAparicao",
                table: "PersonagemShow",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShowId1",
                table: "PersonagemShow",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShowGeneros",
                table: "ShowGeneros",
                columns: new[] { "GenerosShowsId", "ShowsId" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b1d08448-54ef-4cfd-8baa-fb4c5c268222", "AQAAAAIAAYagAAAAEIZch0/Eg/1Heiha/mZTbQXbJb3cjctsMVeRBOhngOGztUAflXaP0cwIQeTCmK4SEg==", "d351569f-c318-4b7d-b976-876c9eb5b3da" });

            migrationBuilder.CreateIndex(
                name: "IX_ShowGeneros_ShowsId",
                table: "ShowGeneros",
                column: "ShowsId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonagemShow_PersonagemId1",
                table: "PersonagemShow",
                column: "PersonagemId1");

            migrationBuilder.CreateIndex(
                name: "IX_PersonagemShow_ShowId1",
                table: "PersonagemShow",
                column: "ShowId1");

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
        }
    }
}
