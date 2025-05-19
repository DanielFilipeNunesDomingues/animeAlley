using animeAlley.Migrations;
using animeAlley.Models;
using Microsoft.EntityFrameworkCore;

namespace animeAlley.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Autor> Autores { get; set; }

        public DbSet<Comentario> Comentarios { get; set; }

        public DbSet<Forum> Foruns { get; set; }

        public DbSet<Genero> Generos { get; set; }

        public DbSet<Lista> Listas { get; set; }

        public DbSet<ListaShows> ListasShows { get; set; }

        public DbSet<Obra> Obras { get; set; }

        public DbSet<Personagem> Personagens { get; set; }

        public DbSet<Show> Shows { get; set; }

        public DbSet<Studio> Stuidos { get; set; }

        public DbSet<Topico> Topicos { get; set; }

        public DbSet<Utilizador> Utilizadores { get; set; }
    }
}
