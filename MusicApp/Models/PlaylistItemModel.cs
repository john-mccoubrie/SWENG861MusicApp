namespace MusicApp.Models
{
    public class PlaylistItemModel
    {
        public int PlaylistItemId { get; set; } // Primary key
        public int PlaylistId { get; set; } // Part of composite key
        public int SongId { get; set; } // Part of composite key
        public PlaylistModel Playlist { get; set; }
        public SongModel Song { get; set; }
    }
}
