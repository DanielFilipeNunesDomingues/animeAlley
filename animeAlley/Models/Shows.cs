namespace animeAlley.Models;
using System.ComponentModel.DataAnnotations;

public class Shows
{
    [Key] //Primary Key
    public int Id { get; set; }

    [MaxLength(200)]
    public String? nome { get; set; } // Nome do show

    [MaxLength(500)]
    public String? sinopse { get; set; } // Sinopse do show

    public Tipo tipo { get; set; } // Tipo do show (Manga/Anime)

    public Status status { get; set; } // Estado do show (Ainda ativo/Acabado)

    [MaxLength(4)]
    public String? avaliação { get; set; } // Nota do show (0.0-100.0)

    [MaxLength(4)]
    public String? ano { get; set; } // Ano de lançamento do show

    [MaxLength(200)]
    public String? imagem { get; set; } // URL da imagem do show

    [MaxLength(500)]
    public String? trailer { get; set; } // URL do trailer do show

    public int views { get; set; } // Número de visualizações do show

    public Fonte fonte { get; set; } // Fonte do show (Original/Manga/LightNovel/VisualNovel/Jogo)

    public Boolean is_Anime { get; set; } // Se o show é um anime ou não

    //FK

    public string estudio { get; set; } // Estúdio do show

    public string autor { get; set; } // Autor do show

    public string personagens { get; set; } // Personagens do show

    public string generos { get; set; } // Gêneros do show

}

public enum Tipo
{
    Manga,
    Anime
}

public enum Status
{
    Ainda_ativo,
    Acabado
}

public enum Fonte
{
    Original,
    Manga,
    LightNovel,
    VisualNovel,
    Jogo
}
