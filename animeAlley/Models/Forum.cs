using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace animeAlley.Models
{
    public class Forum
    {
        [Key] //Primary Key
        public int id { get; set; }

        [MaxLength(80)] // MaxLength(80) para o campo Tema
        public String? tema { get; set; }

        // FK
        [ForeignKey("topicosID")]
        public String? topicosID { get; set; }


    }
}
