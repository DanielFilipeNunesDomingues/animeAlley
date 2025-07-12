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
    [Required(ErrorMessage = "O nome do autor é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O nome do autor não pode exceder 200 caracteres.")]
    [MinLength(2, ErrorMessage = "O nome do autor deve ter pelo menos 2 caracteres.")]
    [Display(Name = "Nome")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ\s\.\-']+$", ErrorMessage = "O nome deve conter apenas letras, espaços, pontos, hífens e apóstrofes.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Data de nascimento do autor
    /// </summary>
    [DataType(DataType.Date, ErrorMessage = "Por favor, insira uma data válida.")]
    [Display(Name = "Data de Nascimento")]
    public DateTime? DateNasc { get; set; }

    /// <summary>
    /// Gênero do Autor (Masculino, Feminino, Outro)
    /// </summary>
    [Display(Name = "Gênero")]
    public Sexualidade? AutorSexualidade { get; set; }

    /// <summary>
    /// Idade do Autor (pode ser null se não for especificada)
    /// </summary>
    [Display(Name = "Idade")]
    [Range(0, 150, ErrorMessage = "A idade deve estar entre 0 e 150 anos.")]
    public int? Idade { get; set; }

    /// <summary>
    /// Descrição do autor
    /// </summary>
    [MaxLength(10000, ErrorMessage = "A descrição não pode exceder 10.000 caracteres.")]
    [MinLength(10, ErrorMessage = "A descrição deve ter pelo menos 10 caracteres.")]
    [Display(Name = "Sobre")]
    public string Sobre { get; set; } = string.Empty;

    /// <summary>
    /// Foto do autor
    /// </summary>
    [Required(ErrorMessage = "A foto do autor é obrigatória.")]
    [MaxLength(200, ErrorMessage = "O caminho da foto não pode exceder 200 caracteres.")]
    [Display(Name = "Foto")]
    public string Foto { get; set; } = string.Empty;

    /// <summary>
    /// Shows criados pelo autor (relação 1-N com Show)
    /// </summary>
    [ValidateNever]
    public ICollection<Show> ShowsCriados { get; set; } = new List<Show>();
}