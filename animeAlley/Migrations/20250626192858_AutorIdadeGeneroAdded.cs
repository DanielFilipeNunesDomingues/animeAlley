using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class AutorIdadeGeneroAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateNasc",
                table: "Autores",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "Idade",
                table: "Autores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonagemSexualidade",
                table: "Autores",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "29083d09-5863-4a33-b8db-10ed0e0e1e21", "AQAAAAIAAYagAAAAEHX8coBozSAKZUVMMR7FXr+H2sWIYcyCSm0KgQ5UtG4uMA8LOiWPWCjvmvGaULpsig==", "9981cc6c-7779-45ca-8d88-635bd244b9df" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Idade",
                table: "Autores");

            migrationBuilder.DropColumn(
                name: "PersonagemSexualidade",
                table: "Autores");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateNasc",
                table: "Autores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b8df3925-851a-4414-881e-af34887154a9", "AQAAAAIAAYagAAAAEE/zc+rIkN7Nuh9NnoO10ekxd9T6IiHF/SIlBIDuKcErfIGlEOqbp/s7QMgWk7e36Q==", "3d6401cb-e514-4b4f-b416-9d3fa7afe112" });
        }
    }
}
