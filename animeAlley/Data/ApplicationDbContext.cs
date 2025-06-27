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

        /// <summary>
        /// este método é executado imediatamente antes 
        /// da criação da base de dados.
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

            // Configurar a relação 1:1 entre Utilizador e Lista
            modelBuilder.Entity<Utilizador>()
                .HasOne(u => u.Lista)
                .WithOne(l => l.Utilizador)
                .HasForeignKey<Lista>(l => l.UtilizadorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração da relação Show -> ListaShows
            modelBuilder.Entity<ListaShows>()
                .HasOne(ls => ls.Show)
                .WithMany(s => s.ListaShows)
                .HasForeignKey(ls => ls.ShowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração da relação Lista -> ListaShows
            modelBuilder.Entity<ListaShows>()
                .HasOne(ls => ls.Lista)
                .WithMany(l => l.ListaShows)
                .HasForeignKey(ls => ls.ListaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração many-to-many entre Personagem e Show
            modelBuilder.Entity<Personagem>()
                .HasMany(p => p.Shows)
                .WithMany(s => s.Personagens)
                .UsingEntity<Dictionary<string, object>>(
                    "PersonagemShow",
                    j => j
                        .HasOne<Show>()
                        .WithMany()
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Personagem>()
                        .WithMany()
                        .HasForeignKey("PersonagemId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("PersonagemId", "ShowId");
                        j.ToTable("PersonagemShow");
                        j.Property<int>("PersonagemId").HasColumnName("PersonagemId");
                        j.Property<int>("ShowId").HasColumnName("ShowId");
                    });

            // Configuração many-to-many para Shows e Generos
            modelBuilder.Entity<Show>()
                .HasMany(s => s.GenerosShows)
                .WithMany(g => g.Shows)
                .UsingEntity<Dictionary<string, object>>(
                    "ShowGeneros",
                    j => j
                        .HasOne<Genero>()
                        .WithMany()
                        .HasForeignKey("GenerosId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Show>()
                        .WithMany()
                        .HasForeignKey("ShowsId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("ShowsId", "GenerosId");
                        j.ToTable("ShowGeneros");
                        j.Property<int>("ShowsId").HasColumnName("ShowsId");
                        j.Property<int>("GenerosId").HasColumnName("GenerosId");
                    });
        }

        public DbSet<Autor> Autores { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Lista> Listas { get; set; }
        public DbSet<Personagem> Personagens { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<Studio> Studios { get; set; }
        public DbSet<ListaShows> ListaShows { get; set; }
        public DbSet<Utilizador> Utilizadores { get; set; }
    }
}