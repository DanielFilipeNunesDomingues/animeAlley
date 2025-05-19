using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace animeAlley.Models
{
    public class Lista
    {
        /// <summary>
        /// Identificador único da lista.
        /// </summary>
        [Key]
        public int Id { get; set; } //Primary Key

        // FK 1-N

        /// <summary>
        /// FK para a tabela utilizador que criou a lista.
        /// </summary>
        [ForeignKey(nameof(Utilizador))]
        public int UtilizadorId { get; set; }

        /// <summary>
        /// FK para Utilizador
        /// </summary>
        [ValidateNever]
        public Utilizador Utilizador { get; set; } = null!;

        //  FK M-N
        public ICollection<ListaShows> ListaShows { get; set; } = []; // Lista de shows associados a esta lista
    }
}
