using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public class New
    {
        public New()
        {
            Id = Guid.NewGuid().ToString();
        }
        public New (string ID)
        {
            Id = ID;
        }

        [JsonIgnore]
        public string Id { get; set; }

        [JsonIgnore]
        [Required, StringLength(4000)]
        public string Head { get; set; }

        [Required]
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonIgnore]
        [Url]
        public string SourceURL { get; set; }

        [JsonIgnore]
        [Required, StringLength(128)]
        public string Author { get; set; }

        [JsonIgnore]
        public long DatePosted { get; set; }

        public int Score { get; set; }

        [JsonIgnore]
        public List<Comment> Comments { get; set; }
        
    }
}
