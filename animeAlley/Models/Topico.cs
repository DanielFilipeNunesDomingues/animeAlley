using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace animeAlley.Models
{
    public class Topico
    {
        /// <summary>
        /// Identificador único do tópico.
        /// </summary>
        [Key]
        public int Id { get; set; } //Primary Key

        /// <summary>
        /// Título do tópico.
        /// </summary>
        [MaxLength(80)] // MaxLength(80) para o campo título
        [Required]
        public string Titulo { get; set; } = string.Empty; // Título do tópico

        /// <summary>
        /// Conteúdo do tópico.
        /// </summary>
        [MaxLength(500)] // MaxLength(500) para o campo Comentário
        public string Comentario { get; set; } = string.Empty; // Comentário do tópico

        /// <summary>
        /// Data de criação do tópico.
        /// </summary>
        [DataType(DataType.Date)] // DataType.Date para o campo dataPost
        public DateTime DataPost { get; set; } // Data de criação do tópico

        //FK 1-N

        /// <summary>
        /// FK para a tabela utilizador que criou o tópico.
        /// </summary>
        [ForeignKey(nameof(Utilizador))]
        public int UtilizadorId { get; set; }

        /// <summary>
        /// FK para Utilizador
        /// </summary>
        [ValidateNever]
        public Utilizador AutorTopico { get; set; } = null!;

        // FK M-N

        /// <summary>
        /// Lista de comentarios associados a este tópico.
        /// </summary>
        public ICollection<Comentario> Comentarios { get; set; } = []; // Lista de comentários associados a este tópico
    }
}
