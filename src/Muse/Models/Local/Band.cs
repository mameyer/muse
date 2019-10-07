using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Muse.Models.Local
{
    [Table("artist")]
    public class Band : IBaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        public Band()
        {
        }

        public Band(SpotifyApi.NetCore.Artist artist)
        {
            if (artist == null) return;

            this.Id = artist.Id;
            this.Name = artist.Name;
            this.Popularity = artist.Popularity;
            this.ImageUri = artist.Images?.First()?.Url;
        }

        public string Name { get; set; }

        [NotMapped]
        public int Rank { get; set; }

        [NotMapped]
        public string Bio { get; set; }

        public virtual ICollection<BandGenre> BandGenre { get; set; }

        public int Popularity { get; set; }

        [NotMapped]
        public string ImageUri { get; set; }

        public object[] KeyValues => new object[] { Id };
    }
}