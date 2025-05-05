using System.ComponentModel.DataAnnotations;
namespace animeAlley.Models
{
    public class AutorObras
    {
        [Key] //Primary Key
        public int Id { get; set; }

        //FK
        public String? obras { get; set; } // FK para a obra a que o autor pertence
    }
}
