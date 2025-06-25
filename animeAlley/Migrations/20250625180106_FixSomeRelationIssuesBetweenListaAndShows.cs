using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class FixSomeRelationIssuesBetweenListaAndShows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListasShows_Listas_ListaId",
                table: "ListasShows");

            migrationBuilder.DropForeignKey(
                name: "FK_ListasShows_Shows_ShowId",
                table: "ListasShows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListasShows",
                table: "ListasShows");

            migrationBuilder.RenameTable(
                name: "ListasShows",
                newName: "ListaShows");

            migrationBuilder.RenameIndex(
                name: "IX_ListasShows_ShowId",
                table: "ListaShows",
                newName: "IX_ListaShows_ShowId");

            migrationBuilder.RenameIndex(
                name: "IX_ListasShows_ListaId",
                table: "ListaShows",
                newName: "IX_ListaShows_ListaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListaShows",
                table: "ListaShows",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "71b796a1-0f82-459a-8b56-cc19356ef9ea", "AQAAAAIAAYagAAAAEI+6GOSfD55x+zXP/hl+1yeoU3YTC+0fvwYvFTmFji9WsQP9I4ecqcSrJb9v1Lvy4g==", "0c39caec-eecf-4ad8-8073-7599fc3c03df" });

            migrationBuilder.AddForeignKey(
                name: "FK_ListaShows_Listas_ListaId",
                table: "ListaShows",
                column: "ListaId",
                principalTable: "Listas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ListaShows_Shows_ShowId",
                table: "ListaShows",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListaShows_Listas_ListaId",
                table: "ListaShows");

            migrationBuilder.DropForeignKey(
                name: "FK_ListaShows_Shows_ShowId",
                table: "ListaShows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListaShows",
                table: "ListaShows");

            migrationBuilder.RenameTable(
                name: "ListaShows",
                newName: "ListasShows");

            migrationBuilder.RenameIndex(
                name: "IX_ListaShows_ShowId",
                table: "ListasShows",
                newName: "IX_ListasShows_ShowId");

            migrationBuilder.RenameIndex(
                name: "IX_ListaShows_ListaId",
                table: "ListasShows",
                newName: "IX_ListasShows_ListaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListasShows",
                table: "ListasShows",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "51ea0400-b54c-47d6-9bcc-220d3c0076d3", "AQAAAAIAAYagAAAAEL00rxsqS6REz+ft0zw56Y/8tADEewA2lbysBpLd8tJo5/SetAcafJ92NcF50phuPg==", "6001fd07-9d20-4440-a065-296b208297c4" });

            migrationBuilder.AddForeignKey(
                name: "FK_ListasShows_Listas_ListaId",
                table: "ListasShows",
                column: "ListaId",
                principalTable: "Listas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ListasShows_Shows_ShowId",
                table: "ListasShows",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
