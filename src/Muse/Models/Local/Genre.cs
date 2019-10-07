using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Muse.Models.Local
{
    [Table("genre")]
    public class Genre : IBaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; }
        
        public int Rank { get; set; }

        public virtual ICollection<BandGenre> BandGenre { get; set; }

        public object[] KeyValues => new object[] { Id };
    }
}