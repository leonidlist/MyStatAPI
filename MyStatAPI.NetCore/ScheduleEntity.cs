using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MyStatAPI
{
    [JsonObject]
    public class ScheduleEntity
    {
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("lesson")]
        public string LessonNumber { get; set; }
        [JsonProperty("started_at")]
        public string StartTime { get; set; }
        [JsonProperty("finished_at")]
        public string FinishTime { get; set; }
        [JsonProperty("teacher_name")]
        public string Teacher { get; set; }
        [JsonProperty("subject_name")]
        public string Subject { get; set; }
        [JsonProperty("room_name")]
        public string Room { get; set; }
    }
}
