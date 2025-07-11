using System.ComponentModel.DataAnnotations;

namespace animeAlley.Models.ViewModels.StudiosDTO
{
    public class StudioCreateUpdateDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Url(ErrorMessage = "Foto deve ser uma URL válida")]
        [MaxLength(500)]
        public string? Foto { get; set; }

        [StringLength(1000, ErrorMessage = "Sobre deve ter no máximo 1000 caracteres")]
        public string? Sobre { get; set; }

        public DateTime? Fundado { get; set; }
        public DateTime? Fechado { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        public string Status { get; set; } = string.Empty;
    }
}
