﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        [MaxLength(50, ErrorMessage = "O nome não pode ter mais de 50 caracteres.")]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MinLength(2, ErrorMessage = "O nome deve ter pelo menos 2 caracteres.")]
        public string Nome { get; set; } = string.Empty; // Nome do utilizador

        /// <summary>
        /// Foto do utilizador.
        /// </summary>
        [MaxLength(200, ErrorMessage = "O caminho da foto não pode ter mais de 200 caracteres.")]
        public string? Foto { get; set; } // Foto do Utilizador

        /// <summary>
        /// Banner do utilizador.
        /// </summary>
        [MaxLength(200, ErrorMessage = "O caminho do banner não pode ter mais de 200 caracteres.")]
        public string? Banner { get; set; } // Banner do Utilizador

        /// <summary>
        /// Define se o utilizador é administrador.
        /// </summary>
        [Required(ErrorMessage = "O campo isAdmin é obrigatório.")]
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