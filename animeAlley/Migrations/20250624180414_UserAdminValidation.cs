using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class UserAdminValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Foto",
                table: "Utilizadores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "61284c78-22bd-413e-bfa8-f71729a2b370", "AQAAAAIAAYagAAAAEPi68OQFUTMQC4Tz39BYOeHRdPloloMbBr8YQVnrmqLMYNgtKIS08SuLCs1XkirDng==", "f0636e63-8c56-4a5d-97a2-9b5adc9e8983" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Foto",
                table: "Utilizadores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a8100233-e3c1-44c5-90a7-b983d7811053", "AQAAAAIAAYagAAAAEM+sePmYT7w4+4G7kiypZCERXaLWWSznNTg0ly/yK2eOt/Waiem66u1g/7w3GwcZ+w==", "8144c093-b212-4656-beff-bd296802a83f" });
        }
    }
}
