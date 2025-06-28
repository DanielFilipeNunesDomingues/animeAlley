using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class DataAtualizacaoAndDataCriacaoAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtualizacao",
                table: "Shows",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Shows",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a0cbd366-5ca1-47d9-a013-9cb895f7cc18", "AQAAAAIAAYagAAAAEOXAatLWqKdoSiLcPgc9AIfx6NnXip31v8fGfzy6O0rt0VBlPFwjVzZ8nSEZbl6TNw==", "a1ec4f24-19f3-4e0f-8938-f2cf41aabf8d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAtualizacao",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Shows");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "74b44e77-03e4-4c2e-855d-4bd927bbf422", "AQAAAAIAAYagAAAAEAeStEUI1boIE8T/4bpmqOaPRIYCbSFOmivzccieoGnShvX3UveaVNM92r+TCK6gZg==", "12e14d8d-f070-40f5-b4b5-007500b98eba" });
        }
    }
}
