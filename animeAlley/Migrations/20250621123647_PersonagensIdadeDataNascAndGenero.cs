using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class PersonagensIdadeDataNascAndGenero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataNasc",
                table: "Personagens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Idade",
                table: "Personagens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonagemSexualidade",
                table: "Personagens",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0a0681c8-2efb-4fad-b290-b16ae5260db9", "AQAAAAIAAYagAAAAEE//amtu92nAQPbm5eOQyqGpWTJmFaLHBsynTptRSYQUPUTViIIdPklv8VLDZ24a4w==", "3592836f-987d-4465-b312-2548c79ba622" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataNasc",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "Idade",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "PersonagemSexualidade",
                table: "Personagens");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "24e67d77-3f65-47cf-8e5c-ff3e5ff23e36", "AQAAAAIAAYagAAAAEGfNdkgg8F36+X2cejIwws25lJJhdWQax7JCKSSIzHWA9uQbg/o7vknn0VewDcFrRA==", "e9b3c518-d170-42b9-a53c-2e50b9029108" });
        }
    }
}
