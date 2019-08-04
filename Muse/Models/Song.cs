using System.Collections.Generic;
using System.Linq;

namespace Muse.Models
{
    public class Song : IBaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Album { get; set; }

        public int Rank { get; set; }

        public IEnumerable<string> Artists { get; set; }
        public string ArtistsString
        {
            get
            {
                return Artists != null ? string.Join(", ", Artists.ToArray()) : "";
            }
        }

        public List<string> Genres { get; set; }
        public string GenresString
        {
            get
            {
                return Genres != null ? string.Join(", ", Genres.ToArray()) : "";
            }
        }

        public List<int> Popularities { get; set; }
        public string PopularitiesString
        {
            get
            {
                return Popularities != null ? string.Join(", ", Popularities.ToArray()) : "";
            }
        }

        public string Uri { get; set; }
    }
}