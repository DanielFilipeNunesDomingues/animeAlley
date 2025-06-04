using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class ShowNewBannerData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Banner",
                table: "Shows",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "73a333fc-0288-48f3-ad77-6e3505af0c17", "AQAAAAIAAYagAAAAEIxOUue5fX3WqAFcewt/LHmiYwKzkwLsbKQIZVIo9KuqYipd4Su5kQ/tT+ewr3sWTg==", "aa45054b-cc1b-49a6-a274-e6697bfb5c5c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Banner",
                table: "Shows");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "229aaa13-a3d5-4c08-8229-e41bd20d6548", "AQAAAAIAAYagAAAAEAV3mdJxKwbFEZ9igXJDJQu/WrOqHHsVBtog4zR4hM4FmVn+0O9vJX58BhlRSVE9Zg==", "f3dc72a9-fda5-43d7-a895-cece85d49856" });
        }
    }
}
