using ChirpSocial.Business.Models;
using ChirpSocial.Data;
using Microsoft.EntityFrameworkCore;

namespace ChirpSocial.Business.Services
{
    public class UserProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly ChirpService _chirpService;

        public UserProfileService(ApplicationDbContext context, ChirpService chirpService)
        {
            _context = context;
            _chirpService = chirpService;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId, string? currentUserId = null)
        {
            var user = await _context.Users
                .Include(u => u.Chirps)
                    .ThenInclude(c => c.Likes)
                .Include(u => u.Chirps)
                    .ThenInclude(c => c.ChirpPeeps)
                        .ThenInclude(cp => cp.Peep)
                .Include(u => u.Likes)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            var recentChirps = user.Chirps
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .Select(c => new ChirpDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = user.UserName ?? "Unknown",
                    LikeCount = c.Likes.Count,
                    IsLikedByCurrentUser = currentUserId != null && c.Likes.Any(l => l.UserId == currentUserId),
                    Peeps = c.ChirpPeeps.Select(cp => cp.Peep.Tag).ToList()
                })
                .ToList();

            return new UserProfileDto
            {
                UserId = user.Id,
                UserName = user.UserName ?? "Unknown",
                Bio = user.Bio,
                TotalChirps = user.Chirps.Count,
                LikesReceived = user.Chirps.Sum(c => c.Likes.Count),
                LikesGiven = user.Likes.Count,
                RecentChirps = recentChirps
            };
        }

        public async Task<bool> UpdateBioAsync(string userId, string? bio)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.Bio = bio;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
