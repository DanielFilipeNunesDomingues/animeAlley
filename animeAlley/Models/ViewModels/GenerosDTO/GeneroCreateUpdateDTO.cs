using System.ComponentModel.DataAnnotations;

namespace animeAlley.Models.ViewModels.GenerosAPI
{
    public class GeneroCreateUpdateDTO
    {
        [Required(ErrorMessage = "Nome do gênero é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome do gênero deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
    }
}
