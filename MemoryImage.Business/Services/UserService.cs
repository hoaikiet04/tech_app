using MemoryImage.Models;
using MemoryImage.Data.Repositories;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MemoryImage.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;

        public UserService(IUserRepository userRepository, IFileStorageService fileStorageService)
        {
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<bool> UpdateProfilePictureAsync(int userId, IFormFile imageFile)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            // Xóa ảnh cũ
            _fileStorageService.DeleteFile(user.ProfilePicture);

            // Lưu ảnh mới
            string? newImageUrl = await _fileStorageService.SaveFileAsync(imageFile, AppConstants.ProfilePicturesFolderName);
            user.ProfilePicture = newImageUrl;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> RemoveProfilePictureAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            _fileStorageService.DeleteFile(user.ProfilePicture);

            user.ProfilePicture = "/images/pf.png"; // Quay về ảnh mặc định
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UpdateUserBioAsync(int userId, string bio)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.Bio = bio;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteAccountAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            _fileStorageService.DeleteFile(user.ProfilePicture);
            return await _userRepository.DeletePermanentlyAsync(userId);
        }
    }
}