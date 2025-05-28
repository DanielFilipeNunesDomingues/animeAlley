using animeAlley.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace animeAlley.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // este código deve ser colocado dentro da classe 'ApplicationDbContext'
        // serve para adicionar às Migrações um conjunto de registos que devem estar sempre presentes na 
        // base de dados do projeto, desde o seu início.
        // Esta técnica NÃO É ADEQUADA para a criação de dados de teste!!!

        /// <summary>
        /// este método é executado imediatamente antes 
        /// da criação da base de dados. <br />
        /// É utilizado para adicionar as últimas instruções
        /// à criação do modelo
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 'importa' todo o comportamento do método, 
            // aquando da sua definição na SuperClasse
            base.OnModelCreating(modelBuilder);

            // criar os perfis de utilizador da nossa app
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "a", Name = "admin", NormalizedName = "ADMIN" });

            // criar um utilizador para funcionar como ADMIN
            // função para codificar a password
            var hasher = new PasswordHasher<IdentityUser>();
            // criação do utilizador
            modelBuilder.Entity<IdentityUser>().HasData(
                new IdentityUser
                {
                    Id = "admin",
                    UserName = "admin@mail.pt",
                    NormalizedUserName = "ADMIN@MAIL.PT",
                    Email = "admin@mail.pt",
                    NormalizedEmail = "ADMIN@MAIL.PT",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PasswordHash = hasher.HashPassword(null, "Aa0_aa")
                }
            );
            // Associar este utilizador à role ADMIN
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = "admin", RoleId = "a" });
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

        public DbSet<Studio> Studios { get; set; }

        public DbSet<Topico> Topicos { get; set; }

        public DbSet<Utilizador> Utilizadores { get; set; }
    }
}
