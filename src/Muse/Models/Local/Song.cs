using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SpotifyApi.NetCore;

namespace Muse.Models.Local
{
    [Table("song")]
    public class Song : IBaseEntity
    {
        public Song()
        {
        }

        public Song(SpotifyApi.NetCore.Track track)
        {
            if (track == null) return;

            this.Id = track.Id;
            this.Name = track.Name;
            this.Artists = track.Artists;
            this.Album = track.Album.Name;
            this.Uri = track.Uri?.Split(':').Last();
        }

        public Song(SpotifyApi.NetCore.PlaylistTrack playlistTrack)
        {
            if (playlistTrack == null) return;

            this.Id = playlistTrack.Track.Id;
            this.Name = playlistTrack.Track.Name;
            this.Artists = playlistTrack.Track.Artists;
            this.Album = playlistTrack.Track.Album.Name;
            this.Uri = playlistTrack.Track.Uri?.Split(':').Last();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        
        public string Name { get; set; }

        [NotMapped]
        public string Album { get; set; }

        [NotMapped]
        public int Rank { get; set; }

        [NotMapped]

        public IEnumerable<Artist> Artists { get; set; }

        [NotMapped]
        public string ArtistsString
        {
            get
            {
                return Artists != null ? string.Join(", ", Artists.Select(e => e.Name).ToArray()) : "";
            }
        }

        [NotMapped]
        public List<string> Genres { get; set; }

        [NotMapped]
        public string GenresString
        {
            get
            {
                return Genres != null ? string.Join(", ", Genres.ToArray()) : "";
            }
        }

        [NotMapped]
        public List<int> Popularities { get; set; }

        [NotMapped]
        public string PopularitiesString
        {
            get
            {
                return Popularities != null ? string.Join(", ", Popularities.ToArray()) : "";
            }
        }

        public string Uri { get; set; }

        public object[] KeyValues => new object[] { Id };
    }
}