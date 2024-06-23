using System.Collections.Generic;
using MusicApp.Models;

namespace MusicApp.ViewModels
{
    public class ArtistViewModel
    {
        public string Name { get; set; }
        public List<SongViewModel> Songs { get; set; }
    }
}
