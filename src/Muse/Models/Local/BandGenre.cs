using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Muse.Models.Local
{
    [Table("band_genre")]
    public class BandGenre : IBaseEntity
    {
        [ForeignKey("Band")]
        public string BandId { get; set; }

        [ForeignKey("Genre")]
        public long GenreId { get; set; }

        public virtual Band Band { get; set; }
        public virtual Genre Genre { get; set; }

        public object[] KeyValues => new object[] { BandId, GenreId };
    }
}