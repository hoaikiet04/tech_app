using MemoryImage.Models;
using MemoryImage.Models.ViewModels;
using System.Threading.Tasks;

namespace MemoryImage.Business.Services
{
    public interface IAuthService
    {
        Task<User> LoginAsync(LoginViewModel model);
        Task<User> RegisterAsync(RegisterViewModel model);
    }
}