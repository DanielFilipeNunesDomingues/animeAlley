using System.ComponentModel.DataAnnotations;
namespace animeAlley.Models
{
    public class ListaShows
    {
        [Key] //Primary Key
        public int Id { get; set; }

        public Status status { get; set; } // Estado do show (Ativo/Inativo)

        //FK

        public int listaId { get; set; } // FK para da Lista

        public int showId { get; set; } // FK para o Show
    }
}

public enum Status
{
    Assistir,
    Terminei,
    Pausa,
    Desisti,
    Pensar_Assistir
}