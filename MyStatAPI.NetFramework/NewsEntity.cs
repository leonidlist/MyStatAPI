using Newtonsoft.Json;

namespace MyStatAPI.Full
{
    [JsonObject]
    public class NewsEntity
    {
        [JsonProperty("id_bbs")]
        public string Id { get; set; }
        [JsonProperty("theme")]
        public string Theme { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
    }
}