using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SpotifyApi.NetCore
{
    /// <summary>
    /// The full album.
    /// </summary>
    public class FullAlbum : Album
    {
        /// <summary>
        /// Gets or sets the copyrights.
        /// </summary>
        /// <value>
        /// The copyrights.
        /// </value>
        [JsonProperty(PropertyName = "copyrights")]
        public Copyright[] Copyrights { get; set; }

        /// <summary>
        /// Gets or sets the external ids.
        /// </summary>
        /// <value>
        /// The external ids.
        /// </value>
        [JsonProperty(PropertyName = "external_ids")]
        public IDictionary<string, string> ExternalIds { get; set; }

        /// <summary>
        /// Gets or sets the genres.
        /// </summary>
        /// <value>
        /// The genres.
        /// </value>
        [JsonProperty(PropertyName = "genres")]
        public string[] Genres { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the popularity.
        /// </summary>
        /// <value>
        /// The popularity.
        /// </value>
        [JsonProperty(PropertyName = "popularity")]
        public int Popularity { get; set; }

        /// <summary>
        /// Gets or sets the tracks.
        /// </summary>
        /// <value>
        /// The tracks.
        /// </value>
        [JsonProperty(PropertyName = "tracks")]
        public Page<Track> Tracks { get; set; }
    }
}