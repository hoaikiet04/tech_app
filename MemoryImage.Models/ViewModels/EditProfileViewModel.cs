using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MemoryImage.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [StringLength(250, ErrorMessage = "Bio cannot exceed 250 characters.")]
        public string? Bio { get; set; }

        public IFormFile? ImageFile { get; set; }

        // Dùng để hiển thị ảnh hiện tại trong modal
        public string? CurrentProfilePicture { get; set; }
    }
}