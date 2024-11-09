using System.Text.Json.Serialization;

namespace Player.Interfaces.DTO
{
    public interface ICurrentlyPlayingDTO
    {
        [JsonPropertyName("device")]
        object Device { get; set; }
        //Device Device { get; set; }

        [JsonPropertyName("repeatState")]
        string RepeatState { get; set; }

        [JsonPropertyName("shuffleState")]
        bool ShuffleState { get; set; }

        [JsonPropertyName("context")]
        IContextDTO Context { get; }

        [JsonPropertyName("timestamp")]
        long Timestamp { get; set; }

        [JsonPropertyName("progressMs")]
        int ProgressMs { get; set; }

        [JsonPropertyName("isPlaying")]
        bool IsPlaying { get; set; }

        [JsonPropertyName("item")]
        object Item { get; set; }
        //IPlayableItem Item { get; set; }

        [JsonPropertyName("currentlyPlayingType")]
        string CurrentlyPlayingType { get; set; }

        [JsonPropertyName("actions")]
        object Actions { get; set; }
        //Actions Actions { get; set; }
    }
}