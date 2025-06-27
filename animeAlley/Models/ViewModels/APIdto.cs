using System.ComponentModel.DataAnnotations;

namespace animeAlley.DTOs
{
    // ================== RESPOSTA BASE DA API ==================
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<string>? Errors { get; set; }
        public PaginationInfo? Pagination { get; set; }
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }

    // ================== DTOs PARA SHOW (ANIME/MANGA) ==================

    // DTO para Show - Versão resumida para listas
    public class ShowResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Sinopse { get; set; }
        public string? Imagem { get; set; }
        public string Tipo { get; set; } = string.Empty; // "Anime" ou "Manga"
        public decimal? Nota { get; set; }
        public int Ano { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<string> Generos { get; set; } = new();
        public string Studio { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
    }

    // DTO para Show - Versão completa para detalhes
    public class ShowDetalheDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Sinopse { get; set; }
        public string? Imagem { get; set; }
        public string? Banner { get; set; }
        public string? Trailer { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal? Nota { get; set; }
        public int Ano { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Fonte { get; set; } = string.Empty;
        public List<GeneroDto> Generos { get; set; } = new();
        public StudioDto Studio { get; set; } = new();
        public AutorDto Autor { get; set; } = new();
        public List<PersonagemResumoDto> Personagens { get; set; } = new();
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }

    // DTO para criação/atualização de Show
    public class ShowCreateUpdateDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sinopse é obrigatória")]
        [MaxLength(10000, ErrorMessage = "Sinopse deve ter no máximo 10000 caracteres")]
        public string Sinopse { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo é obrigatório")]
        public string Tipo { get; set; } = string.Empty; // "Anime" ou "Manga"

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

    // ================== DTOs PARA GÉNERO ==================
    public class GeneroDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int TotalShows { get; set; }
    }

    public class GeneroCreateUpdateDto
    {
        [Required(ErrorMessage = "Nome do gênero é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome do gênero deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
    }

    // ================== DTOs PARA STUDIO ==================
    public class StudioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Foto { get; set; }
        public string? Sobre { get; set; }
        public DateTime? Fundado { get; set; }
        public DateTime? Fechado { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalShows { get; set; }
    }

    public class StudioCreateUpdateDto
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

    // ================== DTOs PARA AUTOR ==================
    public class AutorDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Biografia { get; set; }
        public string? Foto { get; set; }
        public DateTime? DataNascimento { get; set; }
        public int TotalObras { get; set; }
    }

    public class AutorCreateUpdateDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Biografia deve ter no máximo 2000 caracteres")]
        public string? Biografia { get; set; }

        [Url(ErrorMessage = "Foto deve ser uma URL válida")]
        [MaxLength(500)]
        public string? Foto { get; set; }

        public DateTime? DataNascimento { get; set; }
    }

    // ================== DTOs PARA PERSONAGEM ==================
    public class PersonagemDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string TipoPersonagem { get; set; } = string.Empty;
        public string? Sexualidade { get; set; }
        public int? Idade { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Sinopse { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
        public List<ShowResumoDto> Shows { get; set; } = new();
    }

    public class PersonagemResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string TipoPersonagem { get; set; } = string.Empty;
        public string? Foto { get; set; }
    }

    public class PersonagemCreateUpdateDto
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

    // ================== DTOs PARA LISTAS DE UTILIZADOR ==================
    public class ListaDto
    {
        public int Id { get; set; }
        public int UtilizadorId { get; set; }
        public string NomeUtilizador { get; set; } = string.Empty;
        public List<ListaShowDto> Shows { get; set; } = new();
        public int TotalShows { get; set; }
        public EstatisticasListaDto Estatisticas { get; set; } = new();
    }

    public class ListaShowDto
    {
        public ShowResumoDto Show { get; set; } = new();
        public string Status { get; set; } = string.Empty; // "Assistindo", "Concluído", "Pausado", "Planejo Assistir"
        public DateTime DataAdicao { get; set; }
        public int? Progresso { get; set; } // Episódio/Capítulo atual
        public decimal? NotaUsuario { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }
    }

    public class EstatisticasListaDto
    {
        public int TotalAssistindo { get; set; }
        public int TotalConcluido { get; set; }
        public int TotalPausado { get; set; }
        public int TotalPlanejado { get; set; }
        public decimal NotaMedia { get; set; }
        public int TotalAnimes { get; set; }
        public int TotalMangas { get; set; }
    }

    // DTO para adicionar show à lista
    public class AdicionarShowListaDto
    {
        [Required(ErrorMessage = "ShowId é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "ShowId deve ser maior que zero")]
        public int ShowId { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        public string Status { get; set; } = "Planejo Assistir";

        [Range(0, int.MaxValue, ErrorMessage = "Progresso deve ser maior ou igual a zero")]
        public int? Progresso { get; set; }

        [Range(0.0, 10.0, ErrorMessage = "Nota deve estar entre 0.0 e 10.0")]
        public decimal? NotaUsuario { get; set; }
    }

    // DTO para atualizar status na lista
    public class AtualizarStatusListaDto
    {
        [Required(ErrorMessage = "Status é obrigatório")]
        public string Status { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Progresso deve ser maior ou igual a zero")]
        public int? Progresso { get; set; }

        [Range(0.0, 10.0, ErrorMessage = "Nota deve estar entre 0.0 e 10.0")]
        public decimal? NotaUsuario { get; set; }
    }

    // ================== DTOs PARA PESQUISA E FILTROS ==================
    public class ShowFiltroDto
    {
        public string? Search { get; set; }
        public List<int>? GeneroIds { get; set; }
        public List<int>? StudioIds { get; set; }
        public List<int>? AutorIds { get; set; }
        public string? Tipo { get; set; } // "Anime", "Manga"
        public string? Status { get; set; }
        public decimal? NotaMinima { get; set; }
        public decimal? NotaMaxima { get; set; }
        public int? AnoInicio { get; set; }
        public int? AnoFim { get; set; }
        public string? Fonte { get; set; }
        public string OrdenarPor { get; set; } = "Nome"; // "Nome", "Nota", "Ano", "DataCriacao"
        public string Direcao { get; set; } = "ASC"; // "ASC", "DESC"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // ================== DTOs PARA ESTATÍSTICAS ==================
    public class EstatisticasDto
    {
        public int TotalShows { get; set; }
        public int TotalAnimes { get; set; }
        public int TotalMangas { get; set; }
        public int TotalUtilizadores { get; set; }
        public int TotalGeneros { get; set; }
        public int TotalStudios { get; set; }
        public int TotalAutores { get; set; }
        public int TotalPersonagens { get; set; }
        public decimal NotaMediaGeral { get; set; }
        public List<GeneroPopularDto> GenerosPopulares { get; set; } = new();
        public List<ShowPopularDto> ShowsPopulares { get; set; } = new();
        public List<StudioPopularDto> StudiosPopulares { get; set; } = new();
    }

    public class GeneroPopularDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int TotalShows { get; set; }
        public decimal NotaMedia { get; set; }
    }

    public class ShowPopularDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public decimal? Nota { get; set; }
        public int TotalNasListas { get; set; }
        public string? Imagem { get; set; }
    }

    public class StudioPopularDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int TotalShows { get; set; }
        public decimal NotaMedia { get; set; }
    }

    // ================== DTO PARA RESPOSTA PAGINADA ==================
    public class PaginatedResponseDto<T>
    {
        public List<T> Data { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;
        public string? NextPageUrl { get; set; }
        public string? PreviousPageUrl { get; set; }
    }

    // ================== DTOs PARA VALIDAÇÃO ==================
    public class ValidacaoErroDto
    {
        public string Campo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public object? ValorTentado { get; set; }
    }

    // ================== DTOs PARA RELATÓRIOS ==================
    public class RelatorioShowsDto
    {
        public int TotalShows { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFim { get; set; }
        public List<ShowsPorAnoDto> ShowsPorAno { get; set; } = new();
        public List<ShowsPorGeneroDto> ShowsPorGenero { get; set; } = new();
        public List<ShowsPorStatusDto> ShowsPorStatus { get; set; } = new();
    }

    public class ShowsPorAnoDto
    {
        public int Ano { get; set; }
        public int Quantidade { get; set; }
        public decimal NotaMedia { get; set; }
    }

    public class ShowsPorGeneroDto
    {
        public string Genero { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal NotaMedia { get; set; }
    }

    public class ShowsPorStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }
}