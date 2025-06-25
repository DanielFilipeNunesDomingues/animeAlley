using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace animeAlley.Models
{
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

        /// <summary>
        /// Banner do utilizador.
        /// </summary>
        [MaxLength(200)]
        public string? Banner { get; set; } // Banner do Utilizador

        [Required]
        public bool isAdmin { get; set; } = false;

        /// <summary>
        /// Este atributo servirá para fazer a 'ponte' 
        /// entre a tabela dos Utilizadores e a 
        /// tabela da Autenticação da Microsoft Identity
        /// </summary>
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Propriedade de navegação para a Lista do utilizador
        /// Relação 1:1 - Um utilizador tem uma lista
        /// </summary>
        [ValidateNever]
        public Lista? Lista { get; set; } // Propriedade de navegação para a Lista
    }
}