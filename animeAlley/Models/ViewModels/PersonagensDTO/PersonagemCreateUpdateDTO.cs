using System.ComponentModel.DataAnnotations;

namespace animeAlley.Models.ViewModels.PersonagensDTO
{
    public class PersonagemCreateUpdateDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo de personagem é obrigatório")]
        public string TipoPersonagem { get; set; } = string.Empty;

        public string? Sexualidade { get; set; }

        [Range(0, 1000, ErrorMessage = "Idade deve estar entre 0 e 1000")]
        public int? Idade { get; set; }

        public DateTime? DataNascimento { get; set; }

        [MaxLength(10000, ErrorMessage = "Sinopse deve ter no máximo 10000 caracteres")]
        public string? Sinopse { get; set; }

        [Url(ErrorMessage = "Foto deve ser uma URL válida")]
        [MaxLength(500)]
        public string? Foto { get; set; }

        public List<int> ShowIds { get; set; } = new();
    }
}
