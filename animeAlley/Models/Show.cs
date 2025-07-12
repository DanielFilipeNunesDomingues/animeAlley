namespace animeAlley.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Show
{
    /// <summary>
    /// Identificador único do model Shows
    /// </summary>
    [Key]
    public int Id { get; set; } //Primary Key

    /// <summary>
    /// Nome do show
    /// </summary>
    [Required(ErrorMessage = "O nome do show é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O nome do show não pode exceder 200 caracteres.")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do show
    /// </summary>
    [Required(ErrorMessage = "A sinopse do show é obrigatória.")]
    [MaxLength(10000, ErrorMessage = "A sinopse não pode exceder 10.000 caracteres.")]
    [MinLength(20, ErrorMessage = "A sinopse deve ter pelo menos 20 caracteres.")]
    [Display(Name = "Sinopse")]
    public string Sinopse { get; set; } = string.Empty;

    /// <summary>
    /// Estado atual do show
    /// </summary>
    [Required(ErrorMessage = "O status do show é obrigatório.")]
    [Display(Name = "Status")]
    public Status Status { get; set; }

    /// <summary>
    /// Nota do show (0.0-10.0)
    /// </summary>
    [Range(0.0, 10.0, ErrorMessage = "A nota deve estar entre 0,0 e 10,0.")]
    [Display(Name = "Nota")]
    public decimal Nota { get; set; }

    /// <summary>
    /// Nota auxiliar para recolher o valor da Nota do show
    /// </summary>
    [NotMapped]
    [Display(Name = "Nota")]
    [Required(ErrorMessage = "A nota é obrigatória.")]
    [StringLength(4, ErrorMessage = "A nota deve ter no máximo 4 caracteres.")]
    [RegularExpression(@"^(\d{1}[.,]\d{1,2}|10[.,]0{1,2})$", ErrorMessage = "Nota inválida. O valor deve estar entre 0,0 e 10,0 (casas decimais obrigatórias).")]
    public string NotaAux { get; set; } = string.Empty;

    /// <summary>
    /// Ano em que o show foi lançado
    /// </summary>
    [Required(ErrorMessage = "O ano de lançamento é obrigatório.")]
    [Range(1900, 2100, ErrorMessage = "O ano deve estar entre 1900 e 2100.")]
    [Display(Name = "Ano de Lançamento")]
    public int Ano { get; set; }

    /// <summary>
    /// Capa do show
    /// </summary>
    [Required(ErrorMessage = "A imagem de capa é obrigatória.")]
    [MaxLength(200, ErrorMessage = "O caminho da imagem não pode exceder 200 caracteres.")]
    [Display(Name = "Imagem de Capa")]
    public string Imagem { get; set; } = string.Empty;

    /// <summary>
    /// Banner do show
    /// </summary>
    [Required(ErrorMessage = "O banner é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O caminho do banner não pode exceder 200 caracteres.")]
    [Display(Name = "Banner")]
    public string Banner { get; set; } = string.Empty;

    /// <summary>
    /// Url do trailer do show
    /// </summary>
    [MaxLength(500, ErrorMessage = "A URL do trailer não pode exceder 500 caracteres.")]
    [Display(Name = "Trailer (URL)")]
    public string Trailer { get; set; } = string.Empty;

    /// <summary>
    /// Número de visualizações do show
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "O número de visualizações deve ser um valor positivo.")]
    [Display(Name = "Visualizações")]
    public int Views { get; set; }

    /// <summary>
    /// Fonte onde a obra é originada
    /// </summary>
    [Required(ErrorMessage = "A fonte da obra é obrigatória.")]
    [Display(Name = "Fonte")]
    public Fonte Fonte { get; set; }

    /// <summary>
    /// Data de criação do registro
    /// </summary>
    [Display(Name = "Data de Criação")]
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    /// <summary>
    /// Data da última atualização
    /// </summary>
    [Display(Name = "Data de Atualização")]
    public DateTime? DataAtualizacao { get; set; }

    //FK 1-N
    /// <summary>
    /// FK para a tabela dos Estúdios
    /// </summary>
    [Display(Name = "Estúdio")]
    [ForeignKey(nameof(Studio))]
    [Required(ErrorMessage = "O estúdio é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "Por favor, selecione um estúdio válido.")]
    public int StudioFK { get; set; }

    /// <summary>
    /// FK para os Estúdios
    /// </summary>
    [ValidateNever]
    public Studio Studio { get; set; } = null!;

    /// <summary>
    /// FK para a tabela dos Autores
    /// </summary>
    [Display(Name = "Autor")]
    [ForeignKey(nameof(Autor))]
    [Required(ErrorMessage = "O autor é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "Por favor, selecione um autor válido.")]
    public int AutorFK { get; set; }

    /// <summary>
    /// FK para os Autores
    /// </summary>
    [ValidateNever]
    public Autor Autor { get; set; } = null!;

    //FK M-N
    /// <summary>
    /// Lista de personagens que aparecem neste show (relação N-M)
    /// </summary>
    public ICollection<Personagem> Personagens { get; set; } = new List<Personagem>();

    /// <summary>
    /// Lista de generos que tem no show
    /// </summary>
    public ICollection<Genero> GenerosShows { get; set; } = new List<Genero>();

    /// <summary>
    /// Lista de shows nas listas dos utilizadores
    /// </summary>
    public ICollection<ListaShows> ListaShows { get; set; } = new List<ListaShows>();
}

/// <summary>
/// Enumeração que representa o estado atual do show.
/// </summary>
public enum Status
{
    [Display(Name = "Ainda Ativo")]
    Ainda_ativo,
    [Display(Name = "Finalizado")]
    Finalizado
}

/// <summary>
/// Enumeração que representa a fonte original (adaptação) do show.
/// </summary>
public enum Fonte
{
    [Display(Name = "Original")]
    Original,
    [Display(Name = "Mangá")]
    Manga,
    [Display(Name = "Light Novel")]
    LightNovel,
    [Display(Name = "Visual Novel")]
    VisualNovel,
    [Display(Name = "Jogo")]
    Jogo
}