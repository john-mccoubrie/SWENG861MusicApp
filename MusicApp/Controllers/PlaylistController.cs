using Microsoft.AspNetCore.Mvc;
using MusicApp.Models;
using MusicApp.Services;
using System.Threading.Tasks;

namespace MusicApp.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly MusicService _musicService;

        public PlaylistController(MusicService musicService)
        {
            _musicService = musicService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var playlists = await _musicService.GetAllPlaylistsAsync();
            return View(playlists);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSong(int playlistId, int songId)
        {
            bool success = await _musicService.RemoveSongFromPlaylistAsync(playlistId, songId);
            if (success)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Failed to remove song." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditPlaylistName(int playlistId, string newName)
        {
            await _musicService.UpdatePlaylistNameAsync(playlistId, newName);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePlaylist(int playlistId)
        {
            await _musicService.DeletePlaylistAsync(playlistId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditSong(int playlistItemId, string field, string value)
        {
            bool success = await _musicService.UpdateSongFieldAsync(playlistItemId, field, value);
            return Json(new { success = success });
        }

    }
}
