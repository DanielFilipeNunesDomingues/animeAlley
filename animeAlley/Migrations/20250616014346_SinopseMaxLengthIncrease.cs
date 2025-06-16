using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class SinopseMaxLengthIncrease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Sinopse",
                table: "Shows",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5fcae202-32cc-42be-af80-feeb18cbadf7", "AQAAAAIAAYagAAAAEK4ErIBl2crGDE+UHmoYC9TyPmw0HORuJrTHSKp58n5duhZbFH3TMQofbp24wlV/0w==", "d6238a76-87f1-438b-8be3-a88bc4930c5a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Sinopse",
                table: "Shows",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 10000);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1f555ef1-86a0-49d7-ba76-b505dabb5a35", "AQAAAAIAAYagAAAAEI9ENc4Qvkrag0RJILBFTiZcdDRnlQ+Us9msNQ61KxyeWBmcedG+zvQk73CYD57/Rw==", "fe021883-5931-4522-bd5e-c56277c5eb9b" });
        }
    }
}
