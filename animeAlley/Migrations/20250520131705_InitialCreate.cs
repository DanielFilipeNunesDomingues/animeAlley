using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace animeAlley.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Autores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateNasc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sobre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Foruns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tema = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foruns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stuidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Sobre = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stuidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilizadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Sinopse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Nota = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    Imagem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Trailer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    Fonte = table.Column<int>(type: "int", nullable: false),
                    IsAnime = table.Column<bool>(type: "bit", nullable: false),
                    StudioFK = table.Column<int>(type: "int", nullable: false),
                    AutorFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shows_Autores_AutorFK",
                        column: x => x.AutorFK,
                        principalTable: "Autores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shows_Stuidos_StudioFK",
                        column: x => x.StudioFK,
                        principalTable: "Stuidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Listas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Listas_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Topicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DataPost = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UtilizadorId = table.Column<int>(type: "int", nullable: false),
                    ForumId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topicos_Foruns_ForumId",
                        column: x => x.ForumId,
                        principalTable: "Foruns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Topicos_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Generos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GeneroNome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShowId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Generos_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Personagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoPersonagem = table.Column<int>(type: "int", nullable: false),
                    Sinopse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ShowId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personagens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personagens_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ListasShows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ShowId = table.Column<int>(type: "int", nullable: false),
                    ListaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasShows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasShows_Listas_ListaId",
                        column: x => x.ListaId,
                        principalTable: "Listas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListasShows_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mensagem = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DataComent = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UtilizadorId = table.Column<int>(type: "int", nullable: false),
                    TopicoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comentarios_Topicos_TopicoId",
                        column: x => x.TopicoId,
                        principalTable: "Topicos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comentarios_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Obras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShowID = table.Column<int>(type: "int", nullable: false),
                    AutorId = table.Column<int>(type: "int", nullable: true),
                    PersonagemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Obras_Autores_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Autores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Obras_Personagens_PersonagemId",
                        column: x => x.PersonagemId,
                        principalTable: "Personagens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Obras_Shows_ShowID",
                        column: x => x.ShowID,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_TopicoId",
                table: "Comentarios",
                column: "TopicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UtilizadorId",
                table: "Comentarios",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Generos_ShowId",
                table: "Generos",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Listas_UtilizadorId",
                table: "Listas",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasShows_ListaId",
                table: "ListasShows",
                column: "ListaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasShows_ShowId",
                table: "ListasShows",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_AutorId",
                table: "Obras",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_PersonagemId",
                table: "Obras",
                column: "PersonagemId");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_ShowID",
                table: "Obras",
                column: "ShowID");

            migrationBuilder.CreateIndex(
                name: "IX_Personagens_ShowId",
                table: "Personagens",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_AutorFK",
                table: "Shows",
                column: "AutorFK");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_StudioFK",
                table: "Shows",
                column: "StudioFK");

            migrationBuilder.CreateIndex(
                name: "IX_Topicos_ForumId",
                table: "Topicos",
                column: "ForumId");

            migrationBuilder.CreateIndex(
                name: "IX_Topicos_UtilizadorId",
                table: "Topicos",
                column: "UtilizadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "Generos");

            migrationBuilder.DropTable(
                name: "ListasShows");

            migrationBuilder.DropTable(
                name: "Obras");

            migrationBuilder.DropTable(
                name: "Topicos");

            migrationBuilder.DropTable(
                name: "Listas");

            migrationBuilder.DropTable(
                name: "Personagens");

            migrationBuilder.DropTable(
                name: "Foruns");

            migrationBuilder.DropTable(
                name: "Utilizadores");

            migrationBuilder.DropTable(
                name: "Shows");

            migrationBuilder.DropTable(
                name: "Autores");

            migrationBuilder.DropTable(
                name: "Stuidos");
        }
    }
}
