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
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O nome do gênero deve ter entre 2 e 50 caracteres.")]
        [Required(ErrorMessage = "O nome do gênero é obrigatório.")]
        [Display(Name = "Gênero")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s\-]+$", ErrorMessage = "O nome do gênero deve conter apenas letras, espaços e hífens.")]
        public string GeneroNome { get; set; } = string.Empty; // Nome do gênero

        // FK M-N com Show
        public ICollection<Show> Shows { get; set; } = [];
    }
}