using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace animeAlley.Models
{
    public class Obras
    {
        /// <summary>
        /// Identificador único do model Obras
        /// </summary>
        [Key]
        public int Id { get; set; } //Primary Key

        /// <summary>
        /// FK para a tabela dos Shows
        /// </summary>
        [ForeignKey(nameof(Shows))]
        public int ShowID { get; set; } // ID do show a que a obra pertence
        /// <summary>
        /// FK para os Shows
        /// </summary>
        [ValidateNever]
        public Shows Shows { get; set; } = null!;

    }
}
