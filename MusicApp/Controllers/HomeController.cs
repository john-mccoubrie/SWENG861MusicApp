using Microsoft.AspNetCore.Mvc;
using MusicApp.Models;
using MusicApp.Services;
using System.Diagnostics;
using MusicApp.ViewModels;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MusicApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MusicService _musicService;

        public HomeController(ILogger<HomeController> logger, MusicService musicService)
        {
            _logger = logger;
            _musicService = musicService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View(new SearchResultsViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchQuery)
        {
            var results = await _musicService.SearchSongsAndArtistsAsync(searchQuery);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_SearchResults", results);
            }
            return View(results);
        }

        [HttpPost]
        public async Task<IActionResult> AddSongToPlaylist(string songTitle, string artistName)
        {
            _logger.LogInformation($"Attempting to add song '{songTitle}' by '{artistName}' to the default playlist");

            // Normalize input parameters
            string normalizedSongTitle = songTitle.ToLower().Trim();
            string normalizedArtistName = artistName.ToLower().Trim();

            _logger.LogInformation($"Normalized input parameters: songTitle = '{normalizedSongTitle}', artistName = '{normalizedArtistName}'");

            // Attempt to find the song in the database
            var song = await _musicService.GetSongByTitleAndArtistAsync(normalizedSongTitle, normalizedArtistName);
            if (song == null)
            {
                // Song not found, fetch from Spotify
                _logger.LogInformation($"Song '{songTitle}' by '{artistName}' not found in database. Fetching from Spotify.");
                var trackDetails = await _musicService.FetchSongFromSpotifyAsync(songTitle, artistName);

                if (trackDetails != null)
                {
                    // Create a new song model
                    var newSong = new SongModel
                    {
                        Title = trackDetails.Name,
                        Lyrics = "Lyrics not available", // Assume lyrics aren't available
                        ReleaseDate = DateTime.Parse(trackDetails.Album.ReleaseDate),
                        Artist = new ArtistModel { Name = trackDetails.Artists.FirstOrDefault()?.Name }
                    };

                    // Save the new song and artist to the database
                    song = await _musicService.SaveSongAsync(newSong);
                }
                else
                {
                    _logger.LogWarning($"Song '{songTitle}' by '{artistName}' not found on Spotify.");
                    return Json(new { success = false, message = "Song not found." });
                }
            }

            if (song.Artist != null)
            {
                _logger.LogInformation($"Song '{song.Title}' by '{song.Artist.Name}' found/added. Adding to playlist.");
            }
            else
            {
                _logger.LogWarning($"Song '{song.Title}' found/added but artist is null.");
            }

            var playlist = await _musicService.GetOrCreateDefaultPlaylistAsync();
            await _musicService.AddSongToPlaylistAsync(playlist.PlaylistId, song.SongId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            await _musicService.DeletePlaylistAsync(id);
            return RedirectToAction("Playlists");
        }

        [HttpGet]
        public async Task<IActionResult> EditSong(int playlistItemId)
        {
            var item = await _musicService.GetPlaylistItemByIdAsync(playlistItemId);
            if (item == null) return NotFound();

            var viewModel = new EditSongViewModel
            {
                PlaylistItemId = item.PlaylistItemId,
                Title = item.Song.Title,
                ArtistName = item.Song.Artist.Name,
                ReleaseDate = item.Song.ReleaseDate
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditSong(EditSongViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await _musicService.UpdateSongAsync(viewModel);
                return RedirectToAction("Playlists");
            }

            return View(viewModel);
        }

        public IActionResult CreatePlaylist()
        {
            // Returns a view to create a new playlist
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist(string name)
        {
            await _musicService.CreatePlaylistAsync(name);
            return RedirectToAction("Playlists");
        }

        public async Task<IActionResult> Playlists()
        {
            var playlists = await _musicService.GetAllPlaylistsAsync();
            return View(playlists);
        }

        public async Task<IActionResult> EditPlaylist(int id)
        {
            var playlist = await _musicService.GetPlaylistByIdAsync(id);
            if (playlist == null) return NotFound();
            return View(playlist);
        }
    }
}
