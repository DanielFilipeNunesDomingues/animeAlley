namespace animeAlley.Models;
using System.ComponentModel.DataAnnotations;

public class Utilizador
{
    [Key] //Primary Key
    public int Id { get; set; }

    [MaxLength(80)]
    public String? nome { get; set; } // Nome do utilizador

    [MaxLength(200)]
    public String? email { get; set; } // Email do utilizador

    [MaxLength(24)]
    public String? password { get; set; } // Password do utilizador

    [MaxLength(200)]
    public String? arquivo { get; set; } // Biografia do utilizador
}

