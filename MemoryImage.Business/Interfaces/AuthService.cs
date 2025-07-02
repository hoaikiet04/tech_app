using MemoryImage.Models;
using MemoryImage.Models.ViewModels;
using MemoryImage.Data.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity; // Thêm using
using System; // Thêm using

namespace MemoryImage.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> LoginAsync(LoginViewModel model)
        {
            var user = await _userRepository.GetByEmailAsync(model.Email);
            if (user != null && user.IsActive)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    user.LastLoginAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);
                    return user;
                }
            }
            return null;
        }

        public async Task<User> RegisterAsync(RegisterViewModel model)
        {
            if (await _userRepository.ExistsAsync(model.Email))
                return null;

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsActive = true,
                ProfilePicture = "/images/pf.png"
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            return await _userRepository.CreateAsync(user);
        }
    }
}