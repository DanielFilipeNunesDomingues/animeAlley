using System.ComponentModel.DataAnnotations;
namespace animeAlley.Models
{
    public class Genero
    {
        /// <summary>
        /// Identificador único do model Generos
        /// </summary>
        [Key]
        public int Id { get; set; } //Primary Key

        /// <summary>
        /// Nome do gênero
        /// </summary>
        [StringLength(50)]
        [Required]
        public string GeneroNome { get; set; } = string.Empty; // Nome do gênero
    }
}
