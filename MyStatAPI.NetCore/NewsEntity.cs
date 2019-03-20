using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MyStatAPI
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
