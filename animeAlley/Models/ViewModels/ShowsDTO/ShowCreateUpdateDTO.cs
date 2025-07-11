using System.ComponentModel.DataAnnotations;

namespace animeAlley.Models.ViewModels.ShowsDTO
{
    public class ShowCreateUpdateDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sinopse é obrigatória")]
        [MaxLength(10000, ErrorMessage = "Sinopse deve ter no máximo 10000 caracteres")]
        public string Sinopse { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status é obrigatório")]
        public string Status { get; set; } = string.Empty;

        [Range(0.0, 10.0, ErrorMessage = "Nota deve estar entre 0.0 e 10.0")]
        public decimal? Nota { get; set; }

        [Range(1900, 2100, ErrorMessage = "Ano deve estar entre 1900 e 2100")]
        public int Ano { get; set; }

        [MaxLength(500, ErrorMessage = "URL da imagem deve ter no máximo 500 caracteres")]
        public string? Imagem { get; set; }

        [MaxLength(500, ErrorMessage = "URL do banner deve ter no máximo 500 caracteres")]
        public string? Banner { get; set; }

        [MaxLength(500, ErrorMessage = "URL do trailer deve ter no máximo 500 caracteres")]
        public string? Trailer { get; set; }

        [Required(ErrorMessage = "Fonte é obrigatória")]
        public string Fonte { get; set; } = string.Empty;

        [Required(ErrorMessage = "StudioId é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "StudioId deve ser maior que zero")]
        public int StudioId { get; set; }

        [Required(ErrorMessage = "AutorId é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "AutorId deve ser maior que zero")]
        public int AutorId { get; set; }

        public List<int> GeneroIds { get; set; } = new();
    }
}
