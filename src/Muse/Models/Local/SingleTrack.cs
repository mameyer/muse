using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Muse.Models.Local
{
    [Table("track")]
    public class SingleTrack : IBaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public SingleTrack()
        {
        }

        public static IEnumerable<SingleTrack> CreateFromPlaylistTrack(SpotifyApi.NetCore.PlaylistTrack playlistTrack)
        {
            if (playlistTrack == null) return null;

            List<SingleTrack> singleTracks = new List<SingleTrack>();
            foreach (var item in playlistTrack.Track.Artists)
            {
                singleTracks.Add(new SingleTrack
                {
                    SongId = playlistTrack.Track?.Id,
                    Song = new Song(playlistTrack.Track),
                    BandId = item.Id,
                    Band = new Band(item),
                    AlbumId = playlistTrack.Track?.Album?.Id,
                    Album = new Album(playlistTrack.Track?.Album)
                });
            }

            return singleTracks;
        }

        [ForeignKey("Song")]
        public string SongId { get; set; }

        [ForeignKey("Band")]
        public string BandId { get; set; }

        [ForeignKey("Album")]
        public string AlbumId { get; set; }


        public virtual Song Song { get; set; }
        public virtual Band Band { get; set; }
        public virtual Album Album { get; set; }

        public ICollection<Genre> Genres { get; set; }

        public object[] KeyValues => new object[] { Id };
    }
}