namespace animeAlley.Models.ViewModels.PersonagensDTO
{
    public class PersonagemResumoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string TipoPersonagem { get; set; } = string.Empty;
        public string? Foto { get; set; }
    }
}
