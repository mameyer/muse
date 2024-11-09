using System.Text.Json.Serialization;
using Player.Interfaces.DTO;

namespace Player.Models.DTO
{
    public class CurrentlyPlayingDTO : ICurrentlyPlayingDTO
    {
        //public Device Device { get; set; }
        public object Device { get; set; }

        public string RepeatState { get; set; }

        public bool ShuffleState { get; set; }

        [JsonPropertyName("context")]
        public ContextDTO Context { get; set; }

        public long Timestamp { get; set; }

        public int ProgressMs { get; set; }

        public bool IsPlaying { get; set; }

        //public IPlayableItem Item { get; set; }
        public object Item { get; set; }

        public string CurrentlyPlayingType { get; set; }

        //public Actions Actions { get; set; }
        public object Actions { get; set; }

        [JsonIgnore]
        IContextDTO ICurrentlyPlayingDTO.Context => this.Context;
    }
}