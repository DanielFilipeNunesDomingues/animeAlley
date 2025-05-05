using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace animeAlley.Models
{
    public class forumTopico
    {
        [Key, ForeignKey("Forum")] //Primary Key
        public int id { get; set; }

        // FK
        [ForeignKey("topicoID")]
        public String? topicoID { get; set; }
    }
}
