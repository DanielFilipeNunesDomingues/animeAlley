namespace animeAlley.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Utilizador
{
    /// <summary>
    /// Identificador único do utilizador.
    /// </summary>
    [Key]
    public int Id { get; set; } //Primary Key

    /// <summary>
    /// Nome do utilizador.
    /// </summary>
    [MaxLength(50)]
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = string.Empty; // Nome do utilizador

    /// <summary>
    /// Foto do utilizador.
    /// </summary>
    [MaxLength(200)]
    public string? Foto { get; set; } // Foto do Utilizador

    [Required]
    public bool isAdmin { get; set; } = false;

    /// <summary>
    /// Este atributo servirá para fazer a 'ponte' 
    /// entre a tabela dos Utilizadores e a 
    /// tabela da Autenticação da Microsoft Identity
    /// </summary>
    [StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    // FK M-N
    /// <summary>
    /// Lista do utilizador.
    /// </summary>
    public ICollection<Lista> Listas { get; set; } = [];

}

