using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MyStatAPI
{
    [JsonObject]
    public class GroupUserEntity
    {
        [JsonProperty("amount")]
        public string TotalPoints { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("photo_path")]
        public string PhotoUri { get; set; }
        [JsonProperty("position")]
        public string Position { get; set; }
    }
}
