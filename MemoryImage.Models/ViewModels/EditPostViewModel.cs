// MemoryImage.Models/ViewModels/EditPostViewModel.cs
using Microsoft.AspNetCore.Http; // Cho IFormFile
using System.ComponentModel.DataAnnotations;

namespace MemoryImage.Models.ViewModels // ĐẢM BẢO NAMESPACE NÀY CHÍNH XÁC
{
    public class EditPostViewModel
    {
        public int Id { get; set; } // ID của bài viết cần chỉnh sửa

        [StringLength(500, ErrorMessage = "Content cannot exceed 500 characters.")]
        public string? Content { get; set; }

        public string? ExistingImageUrl { get; set; } // Để hiển thị ảnh hiện có
        public IFormFile? NewImageFile { get; set; } // Để upload ảnh mới
        public bool RemoveExistingImage { get; set; } // Để xóa ảnh hiện có
    }
}