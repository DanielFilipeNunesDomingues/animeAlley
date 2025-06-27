using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class StudioFundadoFechadoAndShowsDesenvolvidosAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Studios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Sobre",
                table: "Studios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fechado",
                table: "Studios",
                type: "datetime2",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fundado",
                table: "Studios",
                type: "datetime2",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dfe968f4-7132-42cf-bd65-e845a364694f", "AQAAAAIAAYagAAAAEB1AZvTTkSs5PK1ACPb1Tz4u6wG/kuC0I1UcC/ijBBPfGD6d2JqZF36UK/6nlP7sIg==", "d898baf9-8270-4f89-9a9c-8a85cbf3b1c4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fechado",
                table: "Studios");

            migrationBuilder.DropColumn(
                name: "Fundado",
                table: "Studios");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Studios",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Sobre",
                table: "Studios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cb6a8a12-ac5f-4c64-a223-ca2d1d2b0f41", "AQAAAAIAAYagAAAAEIC7wnWsQrEM60sh6+8Wy45n9mcWHe56rxHv8AQ/4fpMPmX83HhWyP/GKMoIbMHmSA==", "12d39c6a-cc39-4b6a-ba78-ded829a2a880" });
        }
    }
}
