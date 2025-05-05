using System.ComponentModel.DataAnnotations;
namespace animeAlley.Models
{
    public class PersonagemObras
    {
        [Key] //Primary Key
        public int id { get; set; }

        //FK
        public String? obras { get; set; }
    }
}
