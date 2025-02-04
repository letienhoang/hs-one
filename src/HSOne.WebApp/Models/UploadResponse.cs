using System.Text.Json.Serialization;

namespace HSOne.WebApp.Models
{
    public class UploadResponse
    {
        [JsonPropertyName("path")]
        public required string Path { get; set; }
    }
}
