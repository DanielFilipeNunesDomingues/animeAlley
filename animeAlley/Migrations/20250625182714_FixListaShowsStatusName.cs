using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class FixListaShowsStatusName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ListaShows",
                newName: "ListaStatus");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e43d6db8-b46b-4ade-ba26-8d2e28ee9ea4", "AQAAAAIAAYagAAAAEMtE8rT+oQvMaFd9I8WQBnJJn55i3PL5rfY7yC8QCEMHD7XyFr5qsyiSutuvUTNJuQ==", "e335c47d-c8f7-4f11-aa32-090ad6de4ef6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ListaStatus",
                table: "ListaShows",
                newName: "Status");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "71b796a1-0f82-459a-8b56-cc19356ef9ea", "AQAAAAIAAYagAAAAEI+6GOSfD55x+zXP/hl+1yeoU3YTC+0fvwYvFTmFji9WsQP9I4ecqcSrJb9v1Lvy4g==", "0c39caec-eecf-4ad8-8073-7599fc3c03df" });
        }
    }
}
