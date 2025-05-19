namespace animeAlley.Models;

using System.ComponentModel.DataAnnotations;

public class Autor
{
    /// <summary>
    /// Identificador único do model Autor
    /// </summary>
    [Key]
    public int Id { get; set; } //Primary Key

    /// <summary>
    /// Nome do autor
    /// </summary>
    [Required] // Campo obrigatório
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty; // Nome do autor

    /// <summary>
    /// Data de nascimento do autor
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime DateNasc { get; set; }

    /// <summary>
    /// Descrição do autor
    /// </summary>
    [MaxLength(200)]
    public string Sobre { get; set; } = string.Empty; // Descrição do autor

    /// <summary>
    /// Foto do autor
    /// </summary>
    [Required] // Campo obrigatório
    [MaxLength(200)]
    public string Foto { get; set; } = string.Empty; // URL da foto do autor

    //FK M-N
    public ICollection<Obra> ObrasAutor { get; set; } = []; // FK para a obra a que o autor pertence 
}
