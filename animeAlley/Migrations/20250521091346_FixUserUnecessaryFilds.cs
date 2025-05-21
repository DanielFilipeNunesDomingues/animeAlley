using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class FixUserUnecessaryFilds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Utilizadores");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "685f54de-1ccd-480e-9ac2-b263cfd30548", "AQAAAAIAAYagAAAAELtWyBXsC5vCTKHsODv3vSzq3HXItJiEKlrWIYfZQidZvsJVvVDgpvloVhH5B1m2uQ==", "3cf9f25f-c7b2-4037-9972-28435683c18d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Utilizadores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Utilizadores",
                type: "nvarchar(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d6096232-1271-4655-881c-af9d3d374286", "AQAAAAIAAYagAAAAENzE03EaVy6bu48PrQk3nI6+6EPC60WRVcUX0qek6a366a1ZMnKEnQpg3OkFkYsiqA==", "27b2e6c6-1107-4aa3-b0b9-aa73743779cc" });
        }
    }
}
