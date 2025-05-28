namespace animeAlley.Models;
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
    [MaxLength(200)]
    [Required] // Campo obrigatório
    public string Nome { get; set; } = string.Empty!; // Nome do estúdio

    /// <summary>
    /// Descrição do estúdio
    /// </summary>
    [MaxLength(500)]
    public string Sobre { get; set; } = string.Empty; // Descrição do estúdio

    /// <summary>
    /// Estado atual do estúdio, pode estar Ativo (ainda em funcionamento) ou Inativo (não está mais em funcionamento)
    /// </summary>
    public Estado Status { get; set; } // Estado do estúdio (Ativo/Inativo)

    // FK 1-N com Show
    public ICollection<Show> Shows { get; set; } = [];

}

/// <summary>
/// Enumeração que representa o estado atual do estúdio.
/// </summary>
public enum Estado
{
    Ativo,
    Inativo
}
