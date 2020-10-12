using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Comment
    {
        public Comment() 
        {
            Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ConcurrencyStamp = Guid.NewGuid().ToString();
        }

        public Comment(string webuserId, string newId) : this()
        {
            WebUserId = webuserId;
            NewId = newId;
        }

        public int Id { get; set; }
        public string ConcurrencyStamp { get; set; }

        [Required, StringLength(4000)]
        public string Content { get; set; }
        public long Date { get; set; }
        public bool Moderation { get; set; }

        [StringLength(256)]
        public string UserName { get; set; }

        public string WebUserId { get; set; }
        public WebUser WebUser { get; set; }

        public string NewId { get; set; }
        public New New { get; set; }
    }
}
