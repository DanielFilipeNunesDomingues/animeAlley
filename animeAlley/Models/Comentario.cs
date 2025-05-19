namespace animeAlley.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Comentario
{
    /// <summary>
    /// Identificador único do comentário.
    /// </summary>
    [Key]
    public int Id { get; set; } //Primary Key

    /// <summary>
    /// Conteúdo do comentário.
    /// </summary>
    [MaxLength(500)] // MaxLength(500) para o campo Comentário
    [Required]
    public string Mensagem { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação do comentário.
    /// </summary>
    [DataType(DataType.Date)] // DataType.Date para o campo dataComent para aparecer quando um tópico foi comentado
    public DateTime DataComent { get; set; }

    //FK 1-N

    /// <summary>
    /// FK para a tabela utilizador que criou o comentário.
    /// </summary>
    [ForeignKey(nameof(Utilizador))]
    public int UtilizadorId { get; set; } // ID do utilizador que criou o comentário
    /// <summary>
    /// FK para Utilizador
    /// </summary>
    [ValidateNever]
    public Utilizador Utilizador { get; set; } = null!; // Utilizador que criou o comentário
}
