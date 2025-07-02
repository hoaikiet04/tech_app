using System;

namespace MemoryImage.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}