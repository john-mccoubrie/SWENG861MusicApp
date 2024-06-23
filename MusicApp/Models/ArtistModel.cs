namespace MusicApp.Models
{
    public class ArtistModel
    {
        public int ArtistId { get; set; } // Primary key
        public string Name { get; set; }
        public List<SongModel> Songs { get; set; }
    }
}
