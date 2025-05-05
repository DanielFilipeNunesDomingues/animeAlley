namespace animeAlley.Models;

using System.ComponentModel.DataAnnotations;

public class Autor
{
    [Key] //Primary Key
    public int Id { get; set; }

    [MaxLength(200)]
    public String? nome { get; set; } // Nome do autor

    [DataType(DataType.Date)]
    public DateTime date_nasc { get; set; }

    [MaxLength(200)]
    public String? about { get; set; } // Descrição do autor

    [MaxLength(200)]
    public String? foto { get; set; } // URL da foto do autor

    //FK
    public String? obras { get; set; } // FK para a obra a que o autor pertence
}
