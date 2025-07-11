using animeAlley.Models.ViewModels.AutoresDTO;
using animeAlley.Models.ViewModels.GenerosDTO;
using animeAlley.Models.ViewModels.PersonagensDTO;
using animeAlley.Models.ViewModels.StudiosDTO;

namespace animeAlley.Models.ViewModels.ShowsDTO
{
    public class ShowDetalheDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Sinopse { get; set; }
        public string? Imagem { get; set; }
        public string? Banner { get; set; }
        public string? Trailer { get; set; }
        public decimal? Nota { get; set; }
        public int Ano { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Fonte { get; set; } = string.Empty;
        public List<GeneroDTO> Generos { get; set; } = new();
        public StudioDTO Studio { get; set; } = new();
        public AutorDTO Autor { get; set; } = new();
        public List<PersonagemResumoDTO> Personagens { get; set; } = new();
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
