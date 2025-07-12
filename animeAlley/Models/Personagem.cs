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
    [MaxLength(100, ErrorMessage = "O nome da personagem não pode exceder 100 caracteres.")]
    [Required(ErrorMessage = "O nome da personagem é obrigatório.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Função da personagem na obra (Protagonista, Antagonista, Secundário, Figurante)
    /// </summary>
    [Display(Name = "Tipo de Personagem")]
    [Required(ErrorMessage = "O tipo de personagem é obrigatório.")]
    public TiposPersonagem TipoPersonagem { get; set; }

    /// <summary>
    /// Gênero da personagem (Masculino, Feminino, Outro)
    /// </summary>
    [Display(Name = "Gênero")]
    public Sexualidade? PersonagemSexualidade { get; set; }

    /// <summary>
    /// Idade da personagem (pode ser null se não for especificada)
    /// </summary>
    [Display(Name = "Idade")]
    [Range(0, 9999, ErrorMessage = "A idade deve estar entre 0 e 9999 anos.")]
    public int? Idade { get; set; }

    /// <summary>
    /// Data de nascimento da personagem (pode ser null se não for especificada)
    /// </summary>
    [Display(Name = "Data de Nascimento")]
    [DataType(DataType.Date, ErrorMessage = "Por favor, insira uma data válida.")]
    public DateTime? DataNasc { get; set; }

    /// <summary>
    /// Descrição da personagem
    /// </summary>
    [MaxLength(10000, ErrorMessage = "A sinopse não pode exceder 10.000 caracteres.")]
    [MinLength(10, ErrorMessage = "A sinopse deve ter pelo menos 10 caracteres.")]
    [Display(Name = "Sinopse")]
    public string Sinopse { get; set; } = string.Empty;

    /// <summary>
    /// Foto da personagem
    /// </summary>
    [MaxLength(500, ErrorMessage = "O caminho da foto não pode exceder 500 caracteres.")]
    [Display(Name = "Foto")]
    public string Foto { get; set; } = string.Empty;

    /// <summary>
    /// Lista de shows em que este personagem aparece (relação N-M)
    /// </summary>
    [ValidateNever]
    public ICollection<Show> Shows { get; set; } = new List<Show>();

    /// <summary>
    /// Lista de IDs dos shows selecionados no formulário (não mapeada na BD)
    /// </summary>
    [NotMapped]
    [Display(Name = "Shows Selecionados")]
    public List<int> SelectedShowIds { get; set; } = new List<int>();
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