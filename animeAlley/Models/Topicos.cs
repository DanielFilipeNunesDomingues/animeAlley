using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace animeAlley.Models
{
    public class Topicos
    {
        [Key, ForeignKey("ForumTopico")]
        public int id { get; set; }

        [MaxLength(80)] // MaxLength(80) para o campo título
        public String? titulo { get; set; }

        [MaxLength(500)] // MaxLength(500) para o campo Comentário
        public String? comentario { get; set; }

        [DataType(DataType.Date)] // DataType.Date para o campo dataPost
        public DateTime dataPost { get; set; }

        //As duas em baixo vão ser FK
        [ForeignKey("Utilizador")]
        public String? utilizadorId { get; set; } // ID do utilizador que criou o tópico
        [ForeignKey("comentariosID")]
        public String? comentariosID { get; set; } // ID do comentário associado ao tópico
    }
}
