using System;
using MusicApp.Models;

namespace MusicApp.ViewModels
{
    public class SongViewModel
    {
        public int SongId { get; set; } // Primary key
        public string Title { get; set; }
        public string Lyrics { get; set; }
        public DateTime ReleaseDate { get; set; }
        public ArtistViewModel Artist { get; set; }
    }
}
