using System.Collections.Generic;

namespace animeAlley.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public Utilizador User { get; set; }
        public IEnumerable<ListaShows> UserShows { get; set; }
        public Status? SelectedStatusFilter { get; set; } // For filtering on the client-side
    }
}