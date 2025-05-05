using System.ComponentModel.DataAnnotations;
namespace animeAlley.Models
{
    public class Personagens_Show
    {
        [Key] //Primary Key
        public int id { get; set; }

        //FK
        public String? perosnagem { get; set; } // Nome da personagem
    }
}
