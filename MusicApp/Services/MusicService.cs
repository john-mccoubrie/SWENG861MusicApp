using MusicApp.Models;
using SpotifyAPI.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicApp.ViewModels;
using System.Collections.Generic;

namespace MusicApp.Services
{
    public class MusicService
    {
        private readonly MusicDbContext _context;
        private readonly SpotifyService _spotifyService;

        public MusicService(MusicDbContext context, SpotifyService spotifyService)
        {
            _context = context;
            _spotifyService = spotifyService;
        }

        public async Task<ArtistModel> SearchArtistsAsync(string query)
        {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Name.Contains(query));
            if (artist == null)
            {
                // Fetch artist from Spotify if not found in the local database
                var artists = await _spotifyService.SearchArtistsAsync(query);
                var spotifyArtist = artists.FirstOrDefault();
                if (spotifyArtist != null)
                {
                    artist = new ArtistModel
                    {
                        Name = spotifyArtist.Name
                        // Add other properties as needed
                    };
                    _context.Artists.Add(artist);
                    await _context.SaveChangesAsync();
                }
            }
            return artist;
        }

        public async Task<SongModel> GetSongByTitleAndArtistAsync(string songTitle, string artistName)
        {
            return await _context.Songs
                                 .Include(s => s.Artist)
                                 .FirstOrDefaultAsync(s => s.Title.ToLower().Trim() == songTitle && s.Artist.Name.ToLower().Trim() == artistName);
        }

        public async Task<FullTrack> FetchSongFromSpotifyAsync(string songTitle, string artistName)
        {
            var tracks = await _spotifyService.GetTopTracksAsync(songTitle);
            return tracks.FirstOrDefault(t => t.Artists.Any(a => a.Name.Equals(artistName, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<SongModel> SaveSongAsync(SongModel newSong)
        {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Name == newSong.Artist.Name);
            if (artist == null)
            {
                _context.Artists.Add(newSong.Artist);
                await _context.SaveChangesAsync(); // Ensure this save completes before proceeding
            }
            else
            {
                newSong.Artist = artist; // Re-associate existing artist
            }

            _context.Songs.Add(newSong);
            await _context.SaveChangesAsync();
            return newSong;
        }


        public async Task<IEnumerable<SongModel>> GetAllSongsAsync()
        {
            return await _context.Songs.Include(s => s.Artist).ToListAsync();
        }

        public async Task<PlaylistModel> GetOrCreateDefaultPlaylistAsync()
        {
            var playlist = await _context.Playlists.FirstOrDefaultAsync();
            if (playlist == null)
            {
                playlist = new PlaylistModel { Name = "My Playlist" };
                _context.Playlists.Add(playlist);
                await _context.SaveChangesAsync();
            }
            return playlist;
        }

        public async Task<PlaylistModel> CreatePlaylistAsync(string name)
        {
            var playlist = new PlaylistModel { Name = name };
            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }

        public async Task<IEnumerable<PlaylistModel>> GetAllPlaylistsAsync()
        {
            return await _context.Playlists
        .Include(p => p.Items)
            .ThenInclude(i => i.Song)
                .ThenInclude(s => s.Artist)
        .ToListAsync();
        }

        public async Task<PlaylistModel> GetPlaylistByIdAsync(int id)
        {
            return await _context.Playlists
                .Include(p => p.Items)
                .ThenInclude(i => i.Song)
                .FirstOrDefaultAsync(p => p.PlaylistId == id);
        }

        public async Task AddSongToPlaylistAsync(int playlistId, int songId)
        {
            var playlist = await _context.Playlists.FindAsync(playlistId);
            var song = await _context.Songs.FindAsync(songId);
            if (playlist != null && song != null)
            {
                var playlistItem = new PlaylistItemModel { PlaylistId = playlistId, SongId = songId };
                _context.PlaylistItems.Add(playlistItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> RemoveSongFromPlaylistAsync(int playlistId, int songId)
        {
            var item = await _context.PlaylistItems.FindAsync(playlistId, songId);
            if (item != null)
            {
                _context.PlaylistItems.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }



        public async Task UpdatePlaylistNameAsync(int playlistId, string newName)
        {
            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist != null)
            {
                playlist.Name = newName;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeletePlaylistAsync(int playlistId)
        {
            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<SongViewModel>> GetTopTracksAsync(string query)
        {
            var tracks = await _spotifyService.GetTopTracksAsync(query);
            return tracks.Take(5).Select(track => new SongViewModel
            {
                SongId = 0, // Assuming SongId is not relevant here
                Title = track.Name,
                Lyrics = "Lyrics not available", // Assume lyrics aren't available
                ReleaseDate = DateTime.Parse(track.Album.ReleaseDate),
                Artist = new ArtistViewModel { Name = track.Artists.FirstOrDefault()?.Name }
            });
        }

        public async Task<SearchResultsViewModel> SearchSongsAndArtistsAsync(string query)
        {
            var viewModel = new SearchResultsViewModel();

            // Get top 5 tracks from Spotify
            viewModel.Songs = (await GetTopTracksAsync(query)).ToList();

            // Search for artists in the local database
            var artist = await _context.Artists
                                       .Where(a => a.Name.Contains(query))
                                       .Select(a => new ArtistViewModel { Name = a.Name })
                                       .FirstOrDefaultAsync();

            if (artist != null)
            {
                viewModel.Artists = new List<ArtistViewModel> { artist };
            }
            else
            {
                viewModel.Artists = new List<ArtistViewModel>();
            }

            return viewModel;
        }

        public async Task<PlaylistItemModel> GetPlaylistItemByIdAsync(int playlistItemId)
        {
            return await _context.PlaylistItems
                                 .Include(pi => pi.Song)
                                 .ThenInclude(s => s.Artist)
                                 .FirstOrDefaultAsync(pi => pi.PlaylistItemId == playlistItemId);
        }

        public async Task UpdateSongAsync(EditSongViewModel viewModel)
        {
            var playlistItem = await GetPlaylistItemByIdAsync(viewModel.PlaylistItemId);
            if (playlistItem != null)
            {
                playlistItem.Song.Title = viewModel.Title;
                playlistItem.Song.Artist.Name = viewModel.ArtistName;
                playlistItem.Song.ReleaseDate = viewModel.ReleaseDate;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateSongFieldAsync(int playlistItemId, string field, string value)
        {
            var item = await _context.PlaylistItems.Include(pi => pi.Song).ThenInclude(s => s.Artist)
                                                   .FirstOrDefaultAsync(pi => pi.PlaylistItemId == playlistItemId);
            if (item == null)
                return false;

            switch (field.ToLower())
            {
                case "title":
                    item.Song.Title = value;
                    break;
                case "artist":
                    item.Song.Artist.Name = value;
                    break;
                default:
                    return false;
            }

            _context.Update(item);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
