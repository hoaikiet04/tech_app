using MemoryImage.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MemoryImage.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PostInteractionController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostInteractionController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost("like")]
        public async Task<IActionResult> LikePost([FromBody] LikeRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var userId = int.Parse(userIdString);

            var result = await _postService.LikeOrUnlikePostAsync(request.PostId, userId);
            if (result.success)
            {
                var likeCount = (await _postService.GetLikeCountsAsync(new[] { request.PostId })).GetValueOrDefault(request.PostId);
                return Ok(new { isLiked = result.isLiked, likeCount });
            }
            return BadRequest();
        }

        [HttpPost("comment")]
        public async Task<IActionResult> AddComment([FromBody] CommentRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var userId = int.Parse(userIdString);

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Comment content cannot be empty.");
            }

            var newComment = await _postService.AddCommentAsync(request.PostId, userId, request.Content);
            
            if(newComment?.User == null) return BadRequest("Could not retrieve user info for the comment.");

            return Ok(new 
            {
                userId = newComment.User.Id,
                content = newComment.Content,
                userName = newComment.User.FullName,
                profilePicture = newComment.User.ProfilePicture,
                createdAt = newComment.CreatedAt.ToLocalTime().ToString("HH:mm")
            });
        }
    }

    public class LikeRequest { public int PostId { get; set; } }
    public class CommentRequest { public int PostId { get; set; } public string? Content { get; set; } }
}
