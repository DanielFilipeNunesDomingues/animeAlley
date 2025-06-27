namespace animeAlley.DTOs
{
    // DTO para Show (Anime/Manga) - Versão resumida para listas
    public class ShowResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? ImagemUrl { get; set; }
        public string Tipo { get; set; } = string.Empty; // "Anime" ou "Manga"
        public decimal? Rating { get; set; }
        public int? DataLancamento { get; set; }
        public string? Status { get; set; } // "Em Exibição", "Concluído", etc.
        public List<string> Generos { get; set; } = new();
        public string Studios { get; set; } = string.Empty;
    }

    // DTO para Show - Versão completa para detalhes
    public class ShowDetalheDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? ImagemUrl { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal? Rating { get; set; }
        public int? DataLancamento { get; set; }
        //public DateTime? DataFim { get; set; }
        public string? Status { get; set; }
        //public int? NumeroEpisodios { get; set; }
        //public int? NumeroCapitulos { get; set; }
        public List<GeneroDto> Generos { get; set; } = new();
        public string Studios { get; set; } = string.Empty;
        public List<PersonagemDto> Personagens { get; set; } = new();
        public ObraDto? Obra { get; set; }
    }

    // DTO para Género
    public class GeneroDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    // DTO para Studio
    public class StudioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? Website { get; set; }
    }

    // DTO para Personagem
    public class PersonagemDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? ImagemUrl { get; set; }
        public string? Papel { get; set; } // "Principal", "Secundário", etc.
    }

    // DTO para Obra
    public class ObraDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime? DataPublicacao { get; set; }
        public List<AutorDto> Autores { get; set; } = new();
    }

    // DTO para Autor
    public class AutorDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Biografia { get; set; }
    }

    // DTO para Lista de Utilizador
    public class ListaDto
    {
        public int Id { get; set; }
        public int UtilizadorId { get; set; }
        public string NomeUtilizador { get; set; } = string.Empty;
        public List<ListaShowDto> Shows { get; set; } = new();
    }

    // DTO para item da lista (Show + Status)
    public class ListaShowDto
    {
        public ShowResumoDto Show { get; set; } = new();
        public string Status { get; set; } = string.Empty; // "Assistindo", "Concluído", "Pausado", "Planejo Assistir"
        public DateTime DataAdicao { get; set; }
        public int? Progresso { get; set; } // Episódio/Capítulo atual
        public decimal? NotaUsuario { get; set; }
    }

    // DTO para resposta paginada
    public class PaginatedResponseDto<T>
    {
        public List<T> Data { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;
    }

    // DTO para pesquisa/filtros
    public class ShowFiltroDto
    {
        public string? Search { get; set; }
        public List<int>? GeneroIds { get; set; }
        public List<int>? StudioIds { get; set; }
        public string? Tipo { get; set; } // "Anime", "Manga"
        public string? Status { get; set; }
        public decimal? RatingMinimo { get; set; }
        public int? AnoLancamento { get; set; }
        public string? OrdenarPor { get; set; } = "Nome"; // "Nome", "Rating", "DataLancamento"
        public string? Direcao { get; set; } = "ASC"; // "ASC", "DESC"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // DTO para estatísticas
    public class EstatisticasDto
    {
        public int TotalShows { get; set; }
        public int TotalAnimes { get; set; }
        public int TotalMangas { get; set; }
        public int TotalUtilizadores { get; set; }
        public int TotalGeneros { get; set; }
        public int TotalStudios { get; set; }
        public List<GeneroPopularDto> GenerosPopulares { get; set; } = new();
        public List<ShowPopularDto> ShowsPopulares { get; set; } = new();
    }

    public class GeneroPopularDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int TotalShows { get; set; }
    }

    public class ShowPopularDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public decimal? Rating { get; set; }
        public int TotalNasListas { get; set; }
    }

    // DTO para criação/atualização de Show
    public class ShowCreateUpdateDto
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? ImagemUrl { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public DateTime? DataLancamento { get; set; }
        public DateTime? DataFim { get; set; }
        public string? Status { get; set; }
        public int? NumeroEpisodios { get; set; }
        public int? NumeroCapitulos { get; set; }
        public string? Sinopse { get; set; }
        public List<int> GeneroIds { get; set; } = new();
        public List<int> StudioIds { get; set; } = new();
    }

    // DTO para adicionar show à lista
    public class AdicionarShowListaDto
    {
        public int ShowId { get; set; }
        public string Status { get; set; } = "Planejo Assistir";
        public int? Progresso { get; set; }
        public decimal? NotaUsuario { get; set; }
    }

    // DTO para atualizar status na lista
    public class AtualizarStatusListaDto
    {
        public string Status { get; set; } = string.Empty;
        public int? Progresso { get; set; }
        public decimal? NotaUsuario { get; set; }
    }
}