namespace animeAlley.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

public class Autor
{
    /// <summary>
    /// Identificador único do model Autor
    /// </summary>
    [Key]
    public int Id { get; set; } //Primary Key

    /// <summary>
    /// Nome do autor
    /// </summary>
    [Required] // Campo obrigatório
    [MaxLength(200)]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty; // Nome do autor

    /// <summary>
    /// Data de nascimento do autor
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? DateNasc { get; set; }

    /// <summary>
    /// Gênero do Autor (Masculino, Feminino, Outro)
    /// </summary>
    [Display(Name = "Gênero")]
    public Sexualidade? AutorSexualidade { get; set; } // Gênero da personagem (Masculino, Feminino, Outro)

    /// <summary>
    /// Idade do Autor (pode ser null se não for especificada)
    /// </summary>
    [Display(Name = "Idade")]
    public int? Idade { get; set; }

    /// <summary>
    /// Descrição do autor
    /// </summary>
    [MaxLength(1000)]
    [Display(Name = "Sobre")]
    public string Sobre { get; set; } = string.Empty; // Descrição do autor

    /// <summary>
    /// Foto do autor
    /// </summary>
    [Required] // Campo obrigatório
    [MaxLength(200)]
    [Display(Name = "Foto")]
    public string Foto { get; set; } = string.Empty; // URL da foto do autor

    [ValidateNever]
    public ICollection<Show> ShowsCriados { get; set; } = new List<Show>();
}