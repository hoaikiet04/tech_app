using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MemoryImage.Data.Repositories;
using System.Security.Claims;
using System.Collections.Generic;
using MemoryImage.Models;

namespace MemoryImage.Web.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly IUserRepository _userRepository;

        public SearchController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index(string query)
        {
            ViewData["SearchQuery"] = query;
            if (string.IsNullOrWhiteSpace(query))
            {
                return View(new List<User>());
            }
            
            var currentUserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserIdString == null) return Unauthorized();
            var currentUserId = int.Parse(currentUserIdString);

            var results = await _userRepository.SearchUsersAsync(query, currentUserId);
            
            return View(results);
        }
    }
}