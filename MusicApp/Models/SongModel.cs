namespace MusicApp.Models
{
    public class SongModel
    {
        public int SongId { get; set; } // Primary key
        public string Title { get; set; }
        public string Lyrics { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int ArtistId { get; set; } // Foreign key
        public ArtistModel Artist { get; set; }
        public List<PlaylistItemModel> PlaylistItems { get; set; } = new List<PlaylistItemModel>();
    }
}
