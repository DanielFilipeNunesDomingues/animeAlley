using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace animeAlley.Models
{
    public class Forum
    {
        /// <summary>
        /// Identificador único do fórum.
        /// </summary>
        [Key]
        public int Id { get; set; } //Primary Key

        /// <summary>
        /// Temas específicos para cada forum existente.
        /// </summary>
        [MaxLength(80)] // MaxLength(80) para o campo Tema
        [Required]
        public string Tema { get; set; } = string.Empty; // Tema do fórum

        // FK M-N

        /// <summary>
        /// Lista de tópicos associados a este fórum.
        /// </summary>
        public ICollection<Topico> Topicos { get; set; } = []; // Lista de tópicos associados a este fórum

    }
}
