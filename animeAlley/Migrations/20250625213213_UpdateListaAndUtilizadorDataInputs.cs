using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class UpdateListaAndUtilizadorDataInputs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListaShows_Shows_ShowId",
                table: "ListaShows");

            migrationBuilder.DropIndex(
                name: "IX_Listas_UtilizadorId",
                table: "Listas");

            migrationBuilder.AddColumn<int>(
                name: "ShowId1",
                table: "ListaShows",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b8df3925-851a-4414-881e-af34887154a9", "AQAAAAIAAYagAAAAEE/zc+rIkN7Nuh9NnoO10ekxd9T6IiHF/SIlBIDuKcErfIGlEOqbp/s7QMgWk7e36Q==", "3d6401cb-e514-4b4f-b416-9d3fa7afe112" });

            migrationBuilder.CreateIndex(
                name: "IX_ListaShows_ShowId1",
                table: "ListaShows",
                column: "ShowId1");

            migrationBuilder.CreateIndex(
                name: "IX_Listas_UtilizadorId",
                table: "Listas",
                column: "UtilizadorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ListaShows_Shows_ShowId",
                table: "ListaShows",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListaShows_Shows_ShowId1",
                table: "ListaShows",
                column: "ShowId1",
                principalTable: "Shows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListaShows_Shows_ShowId",
                table: "ListaShows");

            migrationBuilder.DropForeignKey(
                name: "FK_ListaShows_Shows_ShowId1",
                table: "ListaShows");

            migrationBuilder.DropIndex(
                name: "IX_ListaShows_ShowId1",
                table: "ListaShows");

            migrationBuilder.DropIndex(
                name: "IX_Listas_UtilizadorId",
                table: "Listas");

            migrationBuilder.DropColumn(
                name: "ShowId1",
                table: "ListaShows");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e43d6db8-b46b-4ade-ba26-8d2e28ee9ea4", "AQAAAAIAAYagAAAAEMtE8rT+oQvMaFd9I8WQBnJJn55i3PL5rfY7yC8QCEMHD7XyFr5qsyiSutuvUTNJuQ==", "e335c47d-c8f7-4f11-aa32-090ad6de4ef6" });

            migrationBuilder.CreateIndex(
                name: "IX_Listas_UtilizadorId",
                table: "Listas",
                column: "UtilizadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListaShows_Shows_ShowId",
                table: "ListaShows",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
