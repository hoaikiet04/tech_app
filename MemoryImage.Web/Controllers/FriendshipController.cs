using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MemoryImage.Business.Services;
using System.Security.Claims;
using System.Threading.Tasks; // Thêm using này

namespace MemoryImage.Web.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly IFriendService _friendService;

        public FriendsController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        // ACTION MỚI ĐỂ HIỂN THỊ TRANG DANH SÁCH BẠN BÈ
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var userId = int.Parse(userIdString);

            var friends = await _friendService.GetFriendsAsync(userId);
            return View(friends);
        }

        [HttpPost]
        public async Task<IActionResult> SendRequest(int receiverId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var userId = int.Parse(userIdString);
            var result = await _friendService.SendFriendRequestAsync(userId, receiverId);

            return Json(new { success = result });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int friendshipId)
        {
            var result = await _friendService.AcceptFriendRequestAsync(friendshipId);
            return Json(new { success = result });
        }

        [HttpPost]
        public async Task<IActionResult> DeclineRequest(int friendshipId)
        {
            var result = await _friendService.DeclineFriendRequestAsync(friendshipId);
            return Json(new { success = result });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int friendId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var userId = int.Parse(userIdString);
            var result = await _friendService.RemoveFriendAsync(userId, friendId);

            return Json(new { success = result });
        }
    }
}