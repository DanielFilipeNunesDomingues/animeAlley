using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace animeAlley.Models
{
    public class TopicosComentario
    {
        [Key, ForeignKey("Topico")] //Primary Key
        public int id { get; set; }

        //FK
        [ForeignKey("comentariosID")]
        public String? comentariosID { get; set; } // ID do comentário associado ao tópico
    }
}
