using System.ComponentModel.DataAnnotations;
namespace animeAlley.Models
{
    public class GeneroShow
    {
        [Key] //Primary Key
        public int Id { get; set; }

        //FK
        public String? nome { get; set; } // Nome da personagem
    }
}
