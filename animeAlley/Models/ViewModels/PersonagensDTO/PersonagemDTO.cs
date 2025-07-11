using animeAlley.Models.ViewModels.ShowsDTO;

namespace animeAlley.Models.ViewModels.PersonagensDTO
{
    public class PersonagemDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string TipoPersonagem { get; set; } = string.Empty;
        public string? Sexualidade { get; set; }
        public int? Idade { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Sinopse { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
        public List<ShowResumoDTO> Shows { get; set; } = new();
    }
}
