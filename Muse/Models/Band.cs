using System.Collections.Generic;
using System.Linq;

namespace Muse.Models
{
    public class Band : IBaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public int Rank { get; set; }

        public string Bio { get; set; }

        public IEnumerable<string> Genres { get; set; }
        public string GenresString
        {
            get
            {
                return Genres != null ? string.Join(", ", Genres.ToArray()) : "";
            }
        }

        public int Popularity { get; set; }
        public string ImageUri { get; set; }
    }
}