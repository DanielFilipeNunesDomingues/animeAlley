using System.ComponentModel.DataAnnotations;

namespace animeAlley.Models
{
    // ================== CLASSE PRINCIPAL DE RESPOSTA DA API ==================
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<string>? Errors { get; set; }
        public PaginationInfo? Pagination { get; set; }
    }

    // ================== INFORMAÇÕES DE PAGINAÇÃO ==================
    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }

    // ================== REQUESTS PARA SHOWS ==================
    public class CreateShowRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sinopse é obrigatória")]
        [MaxLength(10000, ErrorMessage = "Sinopse deve ter no máximo 10000 caracteres")]
        public string Sinopse { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status é obrigatório")]
        public Status Status { get; set; }

        [Range(0.0, 10.0, ErrorMessage = "Nota deve estar entre 0.0 e 10.0")]
        public decimal? Nota { get; set; }

        [RegularExpression(@"^(\d{1}[.,]\d{1,2}|10[.,]0{1,2})$",
            ErrorMessage = "Nota deve estar no formato X,XX ou XX,XX (0,0 a 10,0)")]
        public string? NotaAux { get; set; }

        [Range(1900, 2100, ErrorMessage = "Ano deve estar entre 1900 e 2100")]
        public int Ano { get; set; }

        [MaxLength(200, ErrorMessage = "URLs da imagem deve ter no máximo 200 caracteres")]
        public string? Imagem { get; set; }

        [MaxLength(200, ErrorMessage = "URL do banner deve ter no máximo 200 caracteres")]
        public string? Banner { get; set; }

        [MaxLength(500, ErrorMessage = "URL do trailer deve ter no máximo 500 caracteres")]
        public string? Trailer { get; set; }

        [Required(ErrorMessage = "Fonte é obrigatória")]
        public Fonte Fonte { get; set; }

        [Required(ErrorMessage = "StudioFK é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "StudioFK deve ser maior que zero")]
        public int StudioFK { get; set; }

        [Required(ErrorMessage = "AutorFK é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "AutorFK deve ser maior que zero")]
        public int AutorFK { get; set; }

        public IEnumerable<int>? GeneroIds { get; set; }
    }

    public class UpdateShowRequest
    {
        [Required(ErrorMessage = "Id é obrigatório")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sinopse é obrigatória")]
        [MaxLength(10000, ErrorMessage = "Sinopse deve ter no máximo 10000 caracteres")]
        public string Sinopse { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status é obrigatório")]
        public Status Status { get; set; }

        [Range(0.0, 10.0, ErrorMessage = "Nota deve estar entre 0.0 e 10.0")]
        public decimal? Nota { get; set; }

        [RegularExpression(@"^(\d{1}[.,]\d{1,2}|10[.,]0{1,2})$",
            ErrorMessage = "Nota deve estar no formato X,XX ou XX,XX (0,0 a 10,0)")]
        public string? NotaAux { get; set; }

        [Range(1900, 2100, ErrorMessage = "Ano deve estar entre 1900 e 2100")]
        public int Ano { get; set; }

        [MaxLength(200, ErrorMessage = "URL da imagem deve ter no máximo 200 caracteres")]
        public string? Imagem { get; set; }

        [MaxLength(200, ErrorMessage = "URL do banner deve ter no máximo 200 caracteres")]
        public string? Banner { get; set; }

        [MaxLength(500, ErrorMessage = "URL do trailer deve ter no máximo 500 caracteres")]
        public string? Trailer { get; set; }

        [Required(ErrorMessage = "Fonte é obrigatória")]
        public Fonte Fonte { get; set; }

        [Required(ErrorMessage = "StudioFK é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "StudioFK deve ser maior que zero")]
        public int StudioFK { get; set; }

        [Required(ErrorMessage = "AutorFK é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "AutorFK deve ser maior que zero")]
        public int AutorFK { get; set; }

        public IEnumerable<int>? GeneroIds { get; set; }
    }

    // ================== REQUESTS PARA GÉNEROS ==================
    public class CreateGeneroRequest
    {
        [Required(ErrorMessage = "Nome do gênero é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome do gênero deve ter no máximo 100 caracteres")]
        public string GeneroNome { get; set; } = string.Empty;
    }

    public class UpdateGeneroRequest
    {
        [Required(ErrorMessage = "Id é obrigatório")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome do gênero é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome do gênero deve ter no máximo 100 caracteres")]
        public string GeneroNome { get; set; } = string.Empty;
    }

    // ================== REQUESTS PARA STUDIOS ==================
    public class CreateStudioRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Url(ErrorMessage = "Foto deve ser uma URL válida")]
        public string? Foto { get; set; }

        [StringLength(1000, ErrorMessage = "Sobre deve ter no máximo 1000 caracteres")]
        public string? Sobre { get; set; }

        public DateTime? Fundado { get; set; }

        public DateTime? Fechado { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        public Estado Status { get; set; }
    }

    public class UpdateStudioRequest
    {
        [Required(ErrorMessage = "Id é obrigatório")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Url(ErrorMessage = "Foto deve ser uma URL válida")]
        public string? Foto { get; set; }

        [StringLength(1000, ErrorMessage = "Sobre deve ter no máximo 1000 caracteres")]
        public string? Sobre { get; set; }

        public DateTime? Fundado { get; set; }

        public DateTime? Fechado { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        public Estado Status { get; set; }
    }

    // ================== REQUESTS PARA PERSONAGENS ==================
    public class CreatePersonagemRequest
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public TiposPersonagem TipoPersonagem { get; set; }

        public Sexualidade? PersonagemSexualidade { get; set; }

        public int? Idade { get; set; }

        public DateTime? DataNasc { get; set; }

        [MaxLength(10000)]
        public string? Sinopse { get; set; }

        [MaxLength(500)]
        public string? Foto { get; set; }

        public List<int>? ShowIds { get; set; }
    }

    public class UpdatePersonagemRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public TiposPersonagem TipoPersonagem { get; set; }

        public Sexualidade? PersonagemSexualidade { get; set; }

        public int? Idade { get; set; }

        public DateTime? DataNasc { get; set; }

        [MaxLength(10000)]
        public string? Sinopse { get; set; }

        [MaxLength(500)]
        public string? Foto { get; set; }

        public List<int>? ShowIds { get; set; }
    }
}