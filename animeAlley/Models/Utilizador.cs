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
    /// E-mail do utilizador.
    /// </summary>
    [MaxLength(200)]
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    public string Email { get; set; } = string.Empty; // E-mail do utilizador

    [MaxLength(24)]
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", ErrorMessage = "A senha deve conter pelo menos 8 caracteres, uma letra maiúscula, uma letra minúscula e um número.")]
    public string Password { get; set; } = string.Empty; // Password do utilizador

    /// <summary>
    /// Foto do utilizador.
    /// </summary>
    [MaxLength(200)]
    public string Foto { get; set; } = string.Empty; // Foto do Utilizador

    // FK M-N
    /// <summary>
    /// Lista do utilizador.
    /// </summary>
    public ICollection<Lista> Listas { get; set; } = [];

}

