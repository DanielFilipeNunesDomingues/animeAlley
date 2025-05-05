using System.ComponentModel.DataAnnotations;
namespace animeAlley.Models
{
    public class Lista
    {
        [Key] //Primary Key
        public int Id { get; set; }

        public int utilizadorId { get; set; } // FK para o utilizador

    }
}
