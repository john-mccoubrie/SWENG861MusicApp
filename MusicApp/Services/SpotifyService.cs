using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicApp.Services
{
    public class SpotifyService
    {
        private readonly SpotifyClient _spotify;

        public SpotifyService(string clientId, string clientSecret)
        {
            var config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret));
            _spotify = new SpotifyClient(config);
        }

        // Adjusted to map FullArtist to SimpleArtist
        public async Task<IEnumerable<SimpleArtist>> SearchArtistsAsync(string artistName)
        {
            var searchResponse = await _spotify.Search.Item(new SearchRequest(SearchRequest.Types.Artist, artistName));
            if (searchResponse.Artists != null && searchResponse.Artists.Items.Any())
            {
                return searchResponse.Artists.Items.Select(a => new SimpleArtist
                {
                    Id = a.Id,
                    Name = a.Name,
                    Href = a.Href
                }).ToList();
            }
            return new List<SimpleArtist>();
        }

        public async Task<IEnumerable<FullTrack>> GetTopTracksAsync(string trackName)
        {
            var searchResponse = await _spotify.Search.Item(new SearchRequest(SearchRequest.Types.Track, trackName));
            return searchResponse.Tracks.Items.Take(5);
        }
    }
}
