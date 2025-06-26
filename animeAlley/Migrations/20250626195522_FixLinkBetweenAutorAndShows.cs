using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class FixLinkBetweenAutorAndShows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cb6a8a12-ac5f-4c64-a223-ca2d1d2b0f41", "AQAAAAIAAYagAAAAEIC7wnWsQrEM60sh6+8Wy45n9mcWHe56rxHv8AQ/4fpMPmX83HhWyP/GKMoIbMHmSA==", "12d39c6a-cc39-4b6a-ba78-ded829a2a880" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7b3d63c4-1be5-4c9f-84d6-fad4bb26492e", "AQAAAAIAAYagAAAAEItrR6KIW29PnahH4dONhZxN1IhpNEKtXj5LAS/fwHzUS3IKQ5KDRff8pAKi/G3lxQ==", "108125f0-72a0-499c-af6b-ccc8b9e5d8c9" });
        }
    }
}
