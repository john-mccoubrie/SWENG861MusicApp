namespace MusicApp.ViewModels
{
    public class SearchResultsViewModel
    {
        public List<SongViewModel> Songs { get; set; }
        public List<ArtistViewModel> Artists { get; set; }

        public SearchResultsViewModel()
        {
            Songs = new List<SongViewModel>();
            Artists = new List<ArtistViewModel>();
        }
    }
}
