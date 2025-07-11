namespace animeAlley.Models.ViewModels.StudiosDTO
{
    public class StudioPopularDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int TotalShows { get; set; }
        public decimal NotaMedia { get; set; }
    }
}
