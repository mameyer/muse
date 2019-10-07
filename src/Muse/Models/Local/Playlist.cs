using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Muse.Models.Local
{
    [Table("playlist")]
    public class Playlist : IBaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string SpotifyId { get; set; }

        public string Name { get; set; }

        public string Uri { get; set; }

        public string Owner { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        public int CurrentPopularity { get; set; }

        public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; }

        public object[] KeyValues => new object[] { Id };
    }
}