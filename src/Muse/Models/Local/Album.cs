using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Muse.Models.Local
{
    [Table("album")]
    public class Album : IBaseEntity
    {
        public Album() 
        {
        }

        public Album(SpotifyApi.NetCore.Album album)
        {
            if (album == null) return;

            this.Id = album.Id;
            this.Name = album.Name;
            try
            {
                this.ReleaseDate = DateTime.Parse(album.ReleaseDate);
            }
            catch { };

            this.TotalTracks = album.TotalTracks;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        public string Name { get; set; }

        public int Popularity { get; set; }

        public long TotalTracks { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public object[] KeyValues => new object[] { Id };
    }
}