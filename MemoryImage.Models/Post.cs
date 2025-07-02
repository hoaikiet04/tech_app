using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace MemoryImage.Models
{
    public class Post
    {
        public int Id { get; set; }

        public int UserId { get; set; } // Người đăng bài

        [StringLength(500)]
        public string? Content { get; set; } // Nội dung bài viết (có thể null nếu chỉ đăng ảnh)

        public string? ImageUrl { get; set; } // Đường dẫn tới ảnh (có thể null nếu chỉ đăng text)

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User User { get; set; } // Người dùng đã đăng bài này
        
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}