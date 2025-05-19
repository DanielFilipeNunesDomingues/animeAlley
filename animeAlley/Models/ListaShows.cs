using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace animeAlley.Models
{
    public class ListaShows
    {
        /// <summary>
        /// Identificador único do model ItemsShows
        /// </summary>
        [Key]
        public int Id { get; set; } //Primary Key

        /// <summary>
        /// Status que o utilizador atribui ao show (Assistir, Terminei, Pausa, Desisti, Pensar Assistir)
        /// </summary>
        public Status Status { get; set; } // Estado do show (Assistir, Terminei, Pausa, Desisti, Pensar Assistir)

        //FK 1-N

        /// <summary>
        /// FK para a tabela shows que contém os shows associados a esta lista.
        /// </summary>
        [ForeignKey(nameof(Show))]
        public int ShowId { get; set; } // FK para a tabela Show

        /// <summary>
        /// FK para Shows
        /// </summary>
        [ValidateNever]
        public Shows Show { get; set; } = null!; // FK para o Show

        /// <summary>
        /// FK para a tabela lista que contém os shows associados a esta lista.
        /// </summary>
        [ForeignKey(nameof(Lista))]
        public int ListaId { get; set; } // FK para a tabela Lista

        /// <summary>
        /// FK para Lista
        /// </summary>
        [ValidateNever]
        public Lista Lista { get; set; } = null!; // FK para a Lista


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