using animeAlley.Models;

namespace animeAlley.ViewModels
{
    /// <summary>
    /// ViewModel para a página inicial com diferentes seções
    /// </summary>
    public class BrowseViewModel
    {
        public Show? RandomBannerShow { get; set; }
        public ICollection<Show> TopShows { get; set; } = new List<Show>();
        public ICollection<Show> RecentShows { get; set; } = new List<Show>();
        public ICollection<Personagem> RecentPersonagens { get; set; } = new List<Personagem>();
        public ICollection<Autor> RecentAutores { get; set; } = new List<Autor>();
        public ICollection<Studio> RecentStudios { get; set; } = new List<Studio>();
    }

    /// <summary>
    /// ViewModel para os resultados da pesquisa universal
    /// </summary>
    public class SearchResultsViewModel
    {
        public Show? RandomBannerShow { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public string FilterType { get; set; } = "all";
        public ICollection<Show> Shows { get; set; } = new List<Show>();
        public ICollection<Personagem> Personagens { get; set; } = new List<Personagem>();
        public ICollection<Autor> Autores { get; set; } = new List<Autor>();
        public ICollection<Studio> Studios { get; set; } = new List<Studio>();

        /// <summary>
        /// Retorna o total de resultados encontrados
        /// </summary>
        public int TotalResults => Shows.Count + Personagens.Count + Autores.Count + Studios.Count;

        /// <summary>
        /// Verifica se há resultados
        /// </summary>
        public bool HasResults => TotalResults > 0;
    }
}