using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MyStatAPI
{
    [JsonObject]
    public class UserActivityEntity
    {
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("current_point")]
        public string Points { get; set; }
        [JsonProperty("point_types_id")]
        public string PointTypeId { get; set; }
        [JsonProperty("point_types_name")]
        public string PointTypeName { get; set; }
        [JsonProperty("achievements_id")]
        public string AchievementsId { get; set; }
        [JsonProperty("achievements_name")]
        public string AchievementsName { get; set; }
        [JsonProperty("achievements_type")]
        public string AchievementsType { get; set; }
        [JsonProperty("badge")]
        public string Badge { get; set; }
        [JsonProperty("old_competition")]
        public string OldCompetition { get; set; }
    }
}
