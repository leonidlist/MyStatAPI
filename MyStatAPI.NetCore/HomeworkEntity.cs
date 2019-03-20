using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MyStatAPI
{
    [JsonObject]
    public class HomeworkEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("id_spec")]
        public string SubjectId { get; set; }
        [JsonProperty("id_teach")]
        public string TeacherId { get; set; }
        [JsonProperty("id_group")]
        public string GroupId { get; set; }
        [JsonProperty("fio_teach")]
        public string TeacherName { get; set; }
        [JsonProperty("theme")]
        public string Theme { get; set; }
        [JsonProperty("completion_time")]
        public string CompletionTime { get; set; }
        [JsonProperty("creation_time")]
        public string CreationTime { get; set; }
        [JsonProperty("overdue_time")]
        public string OverdueTime { get; set; }
        [JsonProperty("filename")]
        public string Filename { get; set; }
        [JsonProperty("file_path")]
        public string Filepath { get; set; }
        [JsonProperty("comment")]
        public string Commentary { get; set; }
        [JsonProperty("name_spec")]
        public string SubjectName { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("common_status")]
        public string CommonStatus { get; set; }
        //TODO: Uploaded homework parsing
        //[JsonProperty("homework_stud")]
        //public string HomeworkUploaded { get; set; }
        //[JsonProperty("homework_comment")]
        //public string HomeworkCommentary { get; set; }
    }
}
