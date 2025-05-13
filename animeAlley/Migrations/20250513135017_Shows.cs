using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class Shows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    sinopse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    tipo = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    avaliação = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    ano = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    imagem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    trailer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    views = table.Column<int>(type: "int", nullable: false),
                    fonte = table.Column<int>(type: "int", nullable: false),
                    is_Anime = table.Column<bool>(type: "bit", nullable: false),
                    estudio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    autor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    personagens = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    generos = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shows");
        }
    }
}
