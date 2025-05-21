using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class UserBridgeIdentityFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Utilizadores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "isAdmin",
                table: "Utilizadores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d6096232-1271-4655-881c-af9d3d374286", "AQAAAAIAAYagAAAAENzE03EaVy6bu48PrQk3nI6+6EPC60WRVcUX0qek6a366a1ZMnKEnQpg3OkFkYsiqA==", "27b2e6c6-1107-4aa3-b0b9-aa73743779cc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "isAdmin",
                table: "Utilizadores");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d685f710-b5ae-4682-897e-f2bae0d231f5", "AQAAAAIAAYagAAAAEAMRCCouNb4rHbBvgC56clo+l/LmiRA0V82SiHODKqvlsMJ+cqLz8DC01gxu0tK0pg==", "8f4833d8-35de-4872-bce2-f83d64664e8d" });
        }
    }
}
