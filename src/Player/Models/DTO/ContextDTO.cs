using System.Collections.Generic;
using Player.Interfaces.DTO;

namespace Player.Models.DTO
{
    public class ContextDTO : IContextDTO 
    {
        public object ExternalUrls { get; set; }
        //public Dictionary<string, string> ExternalUrls { get; set; }

        public string Href { get; set; }

        public string Type { get; set; }

        public string Uri { get; set; }
    }
}