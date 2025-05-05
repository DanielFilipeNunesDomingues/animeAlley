namespace animeAlley.Models;
using System.ComponentModel.DataAnnotations;

public class Studio
{
    [Key] //Primary Key
    public int Id { get; set; }

    [MaxLength(200)]
    public string? nome { get; set; } // Nome do estúdio

    [MaxLength(500)]
    public string? about { get; set; } // Descrição do estúdio

    public estados status { get; set; } // Estado do estúdio (Ativo/Inativo)


}

public enum estados
{
    Ativo,
    Inativo
}
