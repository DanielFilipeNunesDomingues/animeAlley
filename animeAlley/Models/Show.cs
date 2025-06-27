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
    [Required]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty; // Nome do show

    /// <summary>
    /// Descrição do show
    /// </summary>
    [Required]
    [MaxLength(10000)]
    public string Sinopse { get; set; } = string.Empty; // Sinopse do show

    /// <summary>
    /// Tipo do show (Manga ou Anime)
    /// </summary>
    [Required]
    public Tipo Tipo { get; set; } // Tipo do show (Manga/Anime)

    /// <summary>
    /// Estado atual do show, pode estar Ainda Ativo (ainda em exibição) ou Finalizado (já terminou)
    /// </summary>
    [Required]
    public Status Status { get; set; } // Estado do show (Ainda ativo/Finalizado)

    /// <summary>
    /// Nota do show (0.0-10.0)
    /// 
    /// ATENÇÃO: Deve existir uma validação para garantir que na base de dados esteja como decimal(4,2)
    /// </summary>
    public decimal Nota { get; set; } // Nota do show (0.0-10.0)

    /// <summary>
    /// Nota auxiliar para recolher o valor da Nota do show
    /// </summary>
    [NotMapped]
    [Display(Name = "Nota")]
    [Required]
    [StringLength(4)]
    [RegularExpression(@"^(\d{1}[.,]\d{1,2}|10[.,]0{1,2})$", ErrorMessage = "Nota inválida. O valor deve estar entre 0,0 e 10,0 (casas decimais obrigatórias).")]
    public string NotaAux { get; set; } = string.Empty; // Auxiliar durante o armazenamento da nota (0.0-10.0)

    /// <summary>
    /// Ano em que o show foi lançado
    /// </summary>
    public int Ano { get; set; } // Ano de lançamento do show

    /// <summary>
    /// Capa do show
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Imagem { get; set; } = string.Empty; // URL da capa do show

    /// <summary>
    /// Banner do show
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Banner { get; set; } = string.Empty; // URL do banner do show

    /// <summary>
    /// Url do trailer do show
    /// </summary>
    [MaxLength(500)]
    public string Trailer { get; set; } = string.Empty; // URL do trailer do show

    /// <summary>
    /// Número de visualizações do show
    /// </summary>
    public int Views { get; set; } // Número de visualizações do show

    /// <summary>
    /// Fonte onde a obra é originada (Original/Manga/LightNovel/VisualNovel/Jogo)
    /// </summary>
    [Required]
    public Fonte Fonte { get; set; } // Fonte do show (Original/Manga/LightNovel/VisualNovel/Jogo)

    //FK 1-N
    /// <summary>
    /// FK para a tabela dos Estúdios
    /// </summary>
    [Display(Name = "Estúdio")]
    [ForeignKey(nameof(Studio))]
    public int StudioFK { get; set; } // Estúdio do show
    /// <summary>
    /// FK para os Estúdios
    /// </summary>
    [ValidateNever]
    public Studio Studio { get; set; } = null!; // FK para os estúdios do show

    /// <summary>
    /// FK para a tabela dos Autores
    /// </summary>
    [Display(Name = "Autor")]
    [ForeignKey(nameof(Autor))]
    public int AutorFK { get; set; } // Autor do show
    /// <summary>
    /// FK para os Autores
    /// </summary>
    [ValidateNever]
    public Autor Autor { get; set; } = null!; // FK para o autor do show

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
/// Enumeração que representa o tipo do show.
/// </summary>
public enum Tipo
{
    [Display(Name = "Manga")]
    Manga,
    [Display(Name = "Anime")]
    Anime
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