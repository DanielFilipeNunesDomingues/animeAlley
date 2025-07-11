using animeAlley.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Chave estrangeira para a tabela Utilizador
    /// </summary>
    public int? UtilizadorId { get; set; }

    /// <summary>
    /// Propriedade de navegação para o Utilizador
    /// </summary>
    public virtual Utilizador? Utilizador { get; set; }

    /// <summary>
    /// Data de criação da conta
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Último login
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Status da conta
    /// </summary>
    public bool IsActive { get; set; } = true;
}