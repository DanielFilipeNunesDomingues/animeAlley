using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAutorSexualidadeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PersonagemSexualidade",
                table: "Autores",
                newName: "AutorSexualidade");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7b3d63c4-1be5-4c9f-84d6-fad4bb26492e", "AQAAAAIAAYagAAAAEItrR6KIW29PnahH4dONhZxN1IhpNEKtXj5LAS/fwHzUS3IKQ5KDRff8pAKi/G3lxQ==", "108125f0-72a0-499c-af6b-ccc8b9e5d8c9" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AutorSexualidade",
                table: "Autores",
                newName: "PersonagemSexualidade");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "29083d09-5863-4a33-b8db-10ed0e0e1e21", "AQAAAAIAAYagAAAAEHX8coBozSAKZUVMMR7FXr+H2sWIYcyCSm0KgQ5UtG4uMA8LOiWPWCjvmvGaULpsig==", "9981cc6c-7779-45ca-8d88-635bd244b9df" });
        }
    }
}
