using System.ComponentModel.DataAnnotations;
namespace animeAlley.Models
{
    public class Obras
    {
        [Key] //Primary Key
        public int id { get; set; }

        //FK
        public string? showID { get; set; }

    }
}
