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
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do show
    /// </summary>
    [Required]
    [MaxLength(10000)]
    public string Sinopse { get; set; } = string.Empty;

    /// <summary>
    /// Estado atual do show
    /// </summary>
    [Required]
    public Status Status { get; set; }

    /// <summary>
    /// Nota do show (0.0-10.0)
    /// </summary>
    public decimal Nota { get; set; }

    /// <summary>
    /// Nota auxiliar para recolher o valor da Nota do show
    /// </summary>
    [NotMapped]
    [Display(Name = "Nota")]
    [Required]
    [StringLength(4)]
    [RegularExpression(@"^(\d{1}[.,]\d{1,2}|10[.,]0{1,2})$", ErrorMessage = "Nota inválida. O valor deve estar entre 0,0 e 10,0 (casas decimais obrigatórias).")]
    public string NotaAux { get; set; } = string.Empty;

    /// <summary>
    /// Ano em que o show foi lançado
    /// </summary>
    public int Ano { get; set; }

    /// <summary>
    /// Capa do show
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Imagem { get; set; } = string.Empty;

    /// <summary>
    /// Banner do show
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Banner { get; set; } = string.Empty;

    /// <summary>
    /// Url do trailer do show
    /// </summary>
    [MaxLength(500)]
    public string Trailer { get; set; } = string.Empty;

    /// <summary>
    /// Número de visualizações do show
    /// </summary>
    public int Views { get; set; }

    /// <summary>
    /// Fonte onde a obra é originada
    /// </summary>
    [Required]
    public Fonte Fonte { get; set; }

    /// <summary>
    /// Data de criação do registro
    /// </summary>
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    /// <summary>
    /// Data da última atualização
    /// </summary>
    public DateTime? DataAtualizacao { get; set; }

    //FK 1-N
    /// <summary>
    /// FK para a tabela dos Estúdios
    /// </summary>
    [Display(Name = "Estúdio")]
    [ForeignKey(nameof(Studio))]
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