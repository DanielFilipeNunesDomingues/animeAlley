namespace animeAlley.Models.ViewModels.ShowsDTO
{
    public class ShowPopularDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal? Nota { get; set; }
        public int TotalNasListas { get; set; }
        public string? Imagem { get; set; }
    }
}
