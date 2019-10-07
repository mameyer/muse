using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Muse.Models.Local
{
    [Table("playlist_track")]
    public class PlaylistTrack : IBaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("Playlist")]
        public long PlaylistId { get; set; }

        [ForeignKey("Track")]
        public long TrackId { get; set; }

        public DateTimeOffset? DateAdded { get; set;}
        
        public int CurrentPopularity { get; set; }

        public virtual Playlist Playlist { get; set; }
        public virtual SingleTrack Track { get; set; }

        public object[] KeyValues => new object[] { Id };
    }
}