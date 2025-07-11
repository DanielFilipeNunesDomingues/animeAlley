using System.ComponentModel.DataAnnotations;

namespace animeAlley.Models.ViewModels.AutoresDTO
{
    public class AutorCreateUpdateDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sobre é obrigatório")]
        [StringLength(10000, ErrorMessage = "Biografia deve ter no máximo 10000 caracteres")]
        public string? Sobre { get; set; }

        [Url(ErrorMessage = "Foto deve ser uma URL válida")]
        [MaxLength(200)]
        public string? Foto { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        public int? Idade { get; set; }

        public Sexualidade? Sexo { get; set; } 
    }
}
