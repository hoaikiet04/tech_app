using MemoryImage.Models;
using System.Collections.Generic;

namespace MemoryImage.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Post> RecentPosts { get; set; }
    }
}