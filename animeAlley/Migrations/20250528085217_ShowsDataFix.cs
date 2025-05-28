using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class ShowsDataFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAnime",
                table: "Shows");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "229aaa13-a3d5-4c08-8229-e41bd20d6548", "AQAAAAIAAYagAAAAEAV3mdJxKwbFEZ9igXJDJQu/WrOqHHsVBtog4zR4hM4FmVn+0O9vJX58BhlRSVE9Zg==", "f3dc72a9-fda5-43d7-a895-cece85d49856" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAnime",
                table: "Shows",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f4a901a9-bfa3-48c7-b95d-106af1381303", "AQAAAAIAAYagAAAAEGNhvklKz4H52MQadn6/mAtcEsEyhNwbAxSWynnu0tnfxsKTNObES+WnGViDPgA1AQ==", "3b23b22e-7ee4-494e-bd28-58b0caf2f5ba" });
        }
    }
}
