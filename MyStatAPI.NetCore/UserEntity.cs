using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MyStatAPI
{
    [JsonObject]
    public class UserEntity
    {
        [JsonProperty("student_id")]
        public string Id { get; set; }
        [JsonProperty("group_id")]
        public string GroupId { get; set; }
        [JsonProperty("current_group_id")]
        public string CurrentGroupId { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("university_group")]
        public string UniversityGroup { get; set; }
        [JsonProperty("achieves_count")]
        public string AchieverCount { get; set; }
        [JsonProperty("stream_id")]
        public string StreamId { get; set; }
        [JsonProperty("stream_name")]
        public string StreamName { get; set; }
        [JsonProperty("group_name")]
        public string GroupName { get; set; }
        [JsonProperty("level")]
        public string Level { get; set; }
        [JsonProperty("photo")]
        public string PhotoUri { get; set; }
    }
}
