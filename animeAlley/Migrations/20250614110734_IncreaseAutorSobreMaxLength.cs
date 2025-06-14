using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseAutorSobreMaxLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Sobre",
                table: "Autores",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f0f63688-7330-45db-87e5-020da8646521", "AQAAAAIAAYagAAAAEFHFrU28qU164m0wuK9GAbbxc8xZogdRUgvCp22ToECoWtBs6KCwThG2dc9NkR5xCg==", "e5611eca-1a82-401e-8784-913398bca785" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Sobre",
                table: "Autores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "24e67d77-3f65-47cf-8e5c-ff3e5ff23e36", "AQAAAAIAAYagAAAAEGfNdkgg8F36+X2cejIwws25lJJhdWQax7JCKSSIzHWA9uQbg/o7vknn0VewDcFrRA==", "e9b3c518-d170-42b9-a53c-2e50b9029108" });
        }
    }
}
