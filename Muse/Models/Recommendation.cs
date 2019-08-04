using System.Collections.Generic;

namespace Muse.Models
{
    public class Recommendation
    {
        public List<Song> Songs { get; set; }
        public List<string> SeedArtists { get; set; }
        public List<string> SeedTracks { get; set; }
        public List<string> SeedGenres { get; set; }
    }
}