using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MemoryImage.Models.ViewModels
{
    public class ProfilePictureViewModel
    {
        [Required(ErrorMessage = "Please select an image file.")]
        [DataType(DataType.Upload)]
        // Bạn có thể thêm các validation tùy chỉnh ở đây, ví dụ: kích thước file, loại file
        public IFormFile ImageFile { get; set; }
    }
}