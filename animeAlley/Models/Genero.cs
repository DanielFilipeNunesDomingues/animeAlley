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
        [Display(Name = "Gênero")]
        public string GeneroNome { get; set; } = string.Empty; // Nome do gênero

        // FK M-N com Show
        public ICollection<Show> Shows { get; set; } = [];
    }
}
