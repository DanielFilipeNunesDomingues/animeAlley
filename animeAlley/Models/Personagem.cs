namespace animeAlley.Models;
using System.ComponentModel.DataAnnotations;

public class Personagem
{
    /// <summary>
    /// Identificador único do model Personagens
    /// </summary>
    [Key] //Primary Key
    public int Id { get; set; }

    /// <summary>
    /// Nome da personagem
    /// </summary>
    [MaxLength(100)] // MaxLength(100) para o campo nome
    [Required] // Campo obrigatório
    public string Nome { get; set; } = string.Empty; // Nome da personagem

    /// <summary>
    /// Função da personagem na obra (Protagonista, Antagonista, Secundário, Figurante)
    /// </summary>
    public TipoPersonagem TipoPersonagem { get; set; } // Tipo de personagem (Protagonista, Antagonista, Secundário, Figurante)

    /// <summary>
    /// Descrição da personagem
    /// </summary>
    [MaxLength(500)] // para o campo sinopse
    public string Sinopse { get; set; } = string.Empty; // Sinopse da personagem

    /// <summary>
    /// Foto da personagem
    /// </summary>
    [MaxLength(500)] // para o campo historia
    public string Foto { get; set; } = string.Empty; // URL da foto da personagem


    //FK M-N

    /// <summary>
    /// Lista de obras a que a personagem pertence
    /// </summary>
    public ICollection<Obra> ObrasPersonagem { get; set; } = []; // FK para as obras a que a personagem pertence
}

public enum TipoPersonagem
{
    Protagonista,
    Antagonista,
    Secundario,
    Figurante
}