using Microsoft.AspNetCore.Mvc;
using ItineroApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ItineroApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Add this to require authentication
    public class FriendsController : ControllerBase
    {
        private readonly MyContext _context;

        public FriendsController(MyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetFriends()
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
            {
                return Unauthorized("User is not authenticated");
            }

            var friends = await _context.Friends
                .Where(f => f.User_Id == currentUserId)
                .Join(_context.Users,
                    friend => friend.Friend_Id,
                    user => user.Id,
                    (friend, user) => new
                    {
                        id = user.Id.ToString(),
                        name = $"{user.name} {user.surname}",
                        avatar = user.profile_picture ?? "profile.jpg",
                        created_at = friend.created_at
                    })
                .ToListAsync();

            return Ok(friends);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Friend>> GetFriend(int id)
        {
            var friend = await _context.Friends.FindAsync(id);

            if (friend == null)
            {
                return NotFound();
            }

            return friend;
        }

        [HttpPost]
        public async Task<ActionResult<object>> AddFriend([FromBody] FriendRequest request)
        {
            if (string.IsNullOrEmpty(request.friendId))
            {
                return BadRequest("Friend ID is required");
            }

            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
            {
                return Unauthorized("User is not authenticated");
            }

            int friendUserId;
            if (!int.TryParse(request.friendId, out friendUserId))
            {
                return BadRequest("Invalid friend ID format");
            }

            var friendUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == friendUserId);

            if (friendUser == null)
            {
                return NotFound("User not found");
            }

            // Check if they're already friends
            var existingFriend = await _context.Friends
                .FirstOrDefaultAsync(f => f.User_Id == currentUserId && f.Friend_Id == friendUser.Id);

            if (existingFriend != null)
            {
                return BadRequest("You are already friends with this user");
            }

            var friend = new Friend
            {
                User_Id = currentUserId,
                Friend_Id = friendUser.Id,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetFriend),
                new { id = friend.Id },
                new
                {
                    id = friendUser.Id.ToString(),
                    name = $"{friendUser.name} {friendUser.surname}",
                    avatar = friendUser.profile_picture ?? "profile.jpg"
                }
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
            {
                return Unauthorized("User is not authenticated");
            }

            int friendUserId;
            if (!int.TryParse(id, out friendUserId))
            {
                return BadRequest("Invalid friend ID format");
            }

            var friend = await _context.Friends
                .FirstOrDefaultAsync(f => f.User_Id == currentUserId && f.Friend_Id == friendUserId);

            if (friend == null)
            {
                return NotFound();
            }

            _context.Friends.Remove(friend);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            // Alternative ways to get the user ID
            var nameIdClaim = User.FindFirst("nameid");
            if (nameIdClaim != null && int.TryParse(nameIdClaim.Value, out userId))
            {
                return userId;
            }

            // If all else fails, check if we have any claims that might contain the user ID
            foreach (var claim in User.Claims)
            {
                if (int.TryParse(claim.Value, out userId))
                {
                    return userId;
                }
            }

            return -1; // Return an invalid ID to indicate no user found
        }
    }

    public class FriendRequest
    {
        public string friendId { get; set; } // Changed to lowercase to match Angular request
    }
}