namespace animeAlley.Models;

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Comentario
{
    [Key, ForeignKey("TopicoComentario")] //Primary Key
    public int id { get; set; }

    [MaxLength(500)] // MaxLength(500) para o campo Comentário
    public String? mensagem { get; set; }

    [DataType(DataType.Date)] // DataType.Date para o campo dataComent para aparecer quando um tópico foi comentado
    public DateTime dataComent { get; set; }

    //Vai ser uma FK

    [ForeignKey("Utilizador")]
    public String? utilizadorId { get; set; } // ID do utilizador que criou o comentário
}
