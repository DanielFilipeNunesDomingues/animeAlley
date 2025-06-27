namespace animeAlley.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    [MaxLength(100)]
    [Required]
    public string Nome { get; set; } = string.Empty; // Nome da personagem

    /// <summary>
    /// Função da personagem na obra (Protagonista, Antagonista, Secundário, Figurante)
    /// </summary>
    [Display(Name = "Tipo de Personagem")]
    public TiposPersonagem TipoPersonagem { get; set; } // Tipo de personagem (Protagonista, Antagonista, Secundário, Figurante)

    /// <summary>
    /// Gênero da personagem (Masculino, Feminino, Outro)
    /// </summary>
    [Display(Name = "Gênero")]
    public Sexualidade? PersonagemSexualidade { get; set; } // Gênero da personagem (Masculino, Feminino, Outro)

    /// <summary>
    /// Idade da personagem (pode ser null se não for especificada)
    /// </summary>
    [Display(Name = "Idade")]
    public int? Idade { get; set; }

    /// <summary>
    /// Data de nascimento da personagem (pode ser null se não for especificada)
    /// </summary>
    [Display(Name = "Data de Nascimento")]
    public DateTime? DataNasc { get; set; }

    /// <summary>
    /// Descrição da personagem
    /// </summary>
    [MaxLength(10000)] // para o campo sinopse
    public string Sinopse { get; set; } = string.Empty; // Sinopse da personagem

    /// <summary>
    /// Foto da personagem
    /// </summary>
    [MaxLength(500)] // para o campo historia
    public string Foto { get; set; } = string.Empty; // URL da foto da personagem

    /// <summary>
    /// Lista de shows em que este personagem aparece (relação N-M)
    /// </summary>
    [ValidateNever]
    public ICollection<Show> Shows { get; set; } = new List<Show>();

    /// <summary>
    /// Lista de IDs dos shows selecionados no formulário (não mapeada na BD)
    /// </summary>
    [NotMapped]
    public List<int> SelectedShowIds { get; set; } = new List<int>();
}

public class PersonagemShow
{
    [Key, Column(Order = 0)]
    public int PersonagemId { get; set; }

    [Key, Column(Order = 1)]
    public int ShowId { get; set; }

    [ForeignKey("PersonagemId")]
    public Personagem Personagem { get; set; } = null!;

    [ForeignKey("ShowId")]
    public Show Show { get; set; } = null!;

    /// <summary>
    /// Papel específico do personagem neste show
    /// </summary>
    public string? PapelNoShow { get; set; }

    /// <summary>
    /// Data de primeira aparição neste show
    /// </summary>
    public DateTime? PrimeiraAparicao { get; set; }
}

public enum TiposPersonagem
{
    [Display(Name = "Protagonista")]
    Protagonista,
    [Display(Name = "Antagonista")]
    Antagonista,
    [Display(Name = "Secundário")]
    Secundario,
    [Display(Name = "Figurante")]
    Figurante
}

public enum Sexualidade
{
    [Display(Name = "Masculino")]
    Masculino,
    [Display(Name = "Feminino")]
    Feminino,
    [Display(Name = "Outro")]
    Outro
}