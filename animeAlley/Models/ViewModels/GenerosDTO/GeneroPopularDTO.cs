using System.ComponentModel.DataAnnotations;

namespace animeAlley.Models.ViewModels.GenerosAPI
{
    public class GeneroPopularDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal MediaNotas { get; set; }
        public int TotalShows { get; set; }
    }
}
