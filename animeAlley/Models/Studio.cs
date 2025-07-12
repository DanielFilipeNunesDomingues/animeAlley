namespace animeAlley.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

public class Studio
{
    /// <summary>
    /// Identificador único do model Studio
    /// </summary>
    [Key]
    public int Id { get; set; } // Identificador único do estúdio

    /// <summary>
    /// Nome do estúdio
    /// </summary>
    [MaxLength(200, ErrorMessage = "O nome do estúdio não pode exceder 200 caracteres.")]
    [Required(ErrorMessage = "O nome do estúdio é obrigatório.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty!; // Nome do estúdio

    /// <summary>
    /// Foto do Estúdio
    /// </summary>
    [Required(ErrorMessage = "A foto do estúdio é obrigatória.")]
    [MaxLength(200, ErrorMessage = "A URL da foto não pode exceder 200 caracteres.")]
    [Display(Name = "Foto")]
    public string Foto { get; set; } = string.Empty; // URL da foto do estúdio

    /// <summary>
    /// Descrição do estúdio
    /// </summary>
    [MaxLength(10000, ErrorMessage = "A descrição não pode exceder 10.000 caracteres.")]
    [Display(Name = "Sobre")]
    public string? Sobre { get; set; } // Descrição do estúdio

    /// <summary>
    /// Quando foi fundado o estúdio
    /// </summary>
    [Display(Name = "Fundado")]
    [DataType(DataType.Date, ErrorMessage = "Por favor, insira uma data válida.")]
    public DateTime? Fundado { get; set; } // Data de fundação do estúdio

    /// <summary>
    /// Quando foi fechado o estúdio
    /// </summary>
    [Display(Name = "Fechado")]
    [DataType(DataType.Date, ErrorMessage = "Por favor, insira uma data válida.")]
    public DateTime? Fechado { get; set; } // Data de fechamento do estúdio

    /// <summary>
    /// Estado atual do estúdio, pode estar Ativo (ainda em funcionamento) ou Inativo (não está mais em funcionamento)
    /// </summary>
    [Display(Name = "Status")]
    [Required(ErrorMessage = "O status do estúdio é obrigatório.")]
    [EnumDataType(typeof(Estado), ErrorMessage = "Por favor, selecione um status válido.")]
    public Estado? Status { get; set; } // Estado do estúdio (Ativo/Inativo)

    /// <summary>
    /// Shows desenvolvidos pelo estúdio (relação 1-N com Show)
    /// </summary>
    [ValidateNever]
    public ICollection<Show> ShowsDesenvolvidos { get; set; } = new List<Show>();
}

/// <summary>
/// Enumeração que representa o estado atual do estúdio.
/// </summary>
public enum Estado
{
    [Display(Name = "Ativo")]
    Ativo,
    [Display(Name = "Inativo")]
    Inativo
}