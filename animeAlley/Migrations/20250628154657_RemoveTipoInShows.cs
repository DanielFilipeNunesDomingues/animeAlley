using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTipoInShows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Shows");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "03b9203c-da5d-4e37-925f-51a615704b25", "AQAAAAIAAYagAAAAEFW3ZeCpGMnWf+u8ooHXqzB/cazekZZCTIrXkVpRTPf+kJXARh3RvTPeDEx4S67V1g==", "0b74d822-fa8d-4878-a525-41518e0bd6df" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Shows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a0cbd366-5ca1-47d9-a013-9cb895f7cc18", "AQAAAAIAAYagAAAAEOXAatLWqKdoSiLcPgc9AIfx6NnXip31v8fGfzy6O0rt0VBlPFwjVzZ8nSEZbl6TNw==", "a1ec4f24-19f3-4e0f-8938-f2cf41aabf8d" });
        }
    }
}
