using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class StudioFotoAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Foto",
                table: "Studios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "90db9133-573e-45ea-bed0-3323faeb5e1c", "AQAAAAIAAYagAAAAEPMcL/P5O82aBCmBDz7V1W1ldaTnKgGKu6CQ0b7HQP5/yhicyjgbhlTVzUuRToYwIg==", "08f09a40-c282-4e4d-8541-cc70ae445d7c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Studios");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dfe968f4-7132-42cf-bd65-e845a364694f", "AQAAAAIAAYagAAAAEB1AZvTTkSs5PK1ACPb1Tz4u6wG/kuC0I1UcC/ijBBPfGD6d2JqZF36UK/6nlP7sIg==", "d898baf9-8270-4f89-9a9c-8a85cbf3b1c4" });
        }
    }
}
