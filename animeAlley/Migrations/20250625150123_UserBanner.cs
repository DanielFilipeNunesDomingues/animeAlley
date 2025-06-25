using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class UserBanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Banner",
                table: "Utilizadores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "51ea0400-b54c-47d6-9bcc-220d3c0076d3", "AQAAAAIAAYagAAAAEL00rxsqS6REz+ft0zw56Y/8tADEewA2lbysBpLd8tJo5/SetAcafJ92NcF50phuPg==", "6001fd07-9d20-4440-a065-296b208297c4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Banner",
                table: "Utilizadores");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "61284c78-22bd-413e-bfa8-f71729a2b370", "AQAAAAIAAYagAAAAEPi68OQFUTMQC4Tz39BYOeHRdPloloMbBr8YQVnrmqLMYNgtKIS08SuLCs1XkirDng==", "f0636e63-8c56-4a5d-97a2-9b5adc9e8983" });
        }
    }
}
