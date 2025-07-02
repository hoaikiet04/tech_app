using System.ComponentModel.DataAnnotations;

namespace MemoryImage.Models.ViewModels
{
    public class EditBioViewModel
    {
        [StringLength(250, ErrorMessage = "Bio cannot exceed 250 characters.")]
        public string? Bio { get; set; }
    }
}