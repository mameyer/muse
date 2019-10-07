using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyApi.NetCore;

namespace Muse.Controllers.API
{
    public class TrackInfo
    {
        public IEnumerable<Models.Local.SingleTrack> Tracks { get; set; }
        public DateTimeOffset? AddedAt { get; set; }
        public Dictionary<string, string[]> Genres { get; set; }
    }

    public class PlaylistApiController : BaseApiController
    {
        private readonly MuseContext _context;

        public PlaylistApiController(MuseContext context)
            : base()
        {
            this._context = context;
        }

        private async Task<IEnumerable<PlaylistSimplified>> GetPlaylists(int start, int count)
        {
            List<PlaylistSimplified> playlists = new List<PlaylistSimplified>();

            if (start < 0|| count <= 0)
                return playlists;

            var accessToken = await GetAccessToken();
            
            var playlistsApi = new PlaylistsApi(this.httpClient, accessToken);

            const int stepSize = 20;
            int end = start + count;

            for (int i = start; i < end; i += stepSize)
            {
                var limit = Math.Min(stepSize, end - i);
                var playlistsRange = await playlistsApi.GetPlaylists("mmeyer_001", accessToken, stepSize, i);
                playlists.AddRange(playlistsRange.Items);
            }

            return playlists;
        }


        public async Task<IEnumerable<PlaylistSimplified>> Get(int start = 0, int count = 20)
        {
            return await GetPlaylists(start, count);
        }

        [HttpPost]
        [Route("store")]
        public async Task<bool> StorePlaylist(string playlistId)
        {
            var accessToken = await GetAccessToken();
            var playlistsApi = new PlaylistsApi(this.httpClient, accessToken);

            var spotifyPlaylist = await playlistsApi.GetPlaylist(playlistId, accessToken);
            if (spotifyPlaylist == null) return false;

            var playlist = this._context.Playlists.Where(e => e.SpotifyId == spotifyPlaylist.Id).FirstOrDefault();
            if (playlist == null)
            {
                playlist = new Models.Local.Playlist
                {
                    SpotifyId = spotifyPlaylist.Id,
                    Name = spotifyPlaylist.Name,
                    Owner = spotifyPlaylist.Owner.Name,
                    Uri = spotifyPlaylist.Uri,
                    DateCreated = DateTime.Now
                };
                this._context.Playlists.Add(playlist);
                await this._context.SaveChangesAsync();
            }

            IEnumerable<TrackInfo> singleTracks = new List<TrackInfo>();
            int offset = 0;
            int limit = 20;
            for(;;)
            {
                var t = await playlistsApi.GetTracks(playlist.SpotifyId, accessToken, limit: limit, offset: offset);

                IEnumerable<TrackInfo> tracks = t.Items
                    .Select(e => new TrackInfo
                    {
                        AddedAt = e.AddedAt,
                        Tracks = Models.Local.SingleTrack.CreateFromPlaylistTrack(e),
                        Genres = e.Track.Artists.ToDictionary(f => f.Id , f => f.Genres)
                    }).ToArray();
                if (tracks == null || tracks.Count() == 0) break;
                
                singleTracks = singleTracks.Concat(tracks);
                offset += limit;
            }
            
            foreach (var trackInfo in singleTracks)
            {
                foreach (var track in trackInfo.Tracks)
                {
                    if (!this._context.Songs.Any(e => e.Id == track.SongId)) this._context.Songs.Add(track.Song);
                    if (!this._context.Albums.Any(e => e.Id == track.AlbumId)) this._context.Albums.Add(track.Album);
                    if (!this._context.Bands.Any(e => e.Id == track.BandId)) this._context.Bands.Add(track.Band);

                    await this._context.SaveChangesAsync();

                    var singleTrack = this._context.SingleTracks
                        .Where(e => e.SongId == track.SongId && e.AlbumId == track.AlbumId && e.BandId == track.BandId)
                        .FirstOrDefault();
                    if (singleTrack == null)
                    {
                        singleTrack = new Models.Local.SingleTrack { SongId = track.SongId, AlbumId = track.AlbumId, BandId = track.BandId };
                        this._context.SingleTracks.Add(singleTrack);
                        await this._context.SaveChangesAsync();
                    }

                    if (!this._context.PlaylistTracks.Any(e => e.PlaylistId == playlist.Id && e.TrackId == singleTrack.Id))
                    {
                        this._context.PlaylistTracks.Add(new Models.Local.PlaylistTrack
                        {
                            PlaylistId = playlist.Id,
                            TrackId = singleTrack.Id,
                            DateAdded = trackInfo.AddedAt
                        });
                        await this._context.SaveChangesAsync();
                    }
                }

                foreach (var genreInfo in trackInfo.Genres)
                {
                    if (genreInfo.Value == null) continue;

                    foreach (var item in genreInfo.Value)
                    {
                        var genre = this._context.Genres.Where(e => e.Name == item).FirstOrDefault();
                        if (genre == null)
                        {
                            genre = new Models.Local.Genre
                            {
                                Name = genreInfo.Key
                            };
                            this._context.Genres.Add(genre);
                        }
                        await this._context.SaveChangesAsync();

                        if (!this._context.BandGenre.Any(e => e.BandId == genreInfo.Key && e.GenreId == genre.Id))
                        {
                            this._context.BandGenre.Add(new Models.Local.BandGenre
                            {
                                BandId = genreInfo.Key,
                                GenreId = genre.Id
                            });
                            await this._context.SaveChangesAsync();
                        }
                    }
                }
            }

            return true;
        }
    }
}