namespace animeAlley.Models;
using System.ComponentModel.DataAnnotations;

public class Personagens
{
    [Key] //Primary Key
    public int id { get; set; }

    [MaxLength(80)] // MaxLength(80) para o campo nome
    public String? nome { get; set; }

    public TipoPersonagem tipoPersonagem { get; set; } // Tipo de personagem (Protagonista, Antagonista, Secundário, Figurante)

    [MaxLength(500)] // para o campo sinopse
    public String? sinopse { get; set; } // Sinopse da personagem

    [MaxLength(200)] // para o campo historia
    public String? foto { get; set; } // URL da foto da personagem


    //FK
    public String? obras { get; set; } // FK para a obra a que a personagem pertence
}

public enum TipoPersonagem
{
    Protagonista,
    Antagonista,
    Secundario,
    Figurante
}