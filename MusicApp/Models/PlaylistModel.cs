namespace MusicApp.Models
{
    public class PlaylistModel
    {
        public int PlaylistId { get; set; } // Primary key
        public string Name { get; set; }
        public List<PlaylistItemModel> Items { get; set; } = new List<PlaylistItemModel>();
    }
}
