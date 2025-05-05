using System.ComponentModel.DataAnnotations;
namespace animeAlley.Models
{
    public class Generos
    {
        [Key] //Primary Key
        public int Id { get; set; }

        //FK
        public String? genero { get; set; } // Nome do gênero
    }
}
