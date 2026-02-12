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

            var profilePictureUrl = user.ProfilePicture != null && user.ProfilePictureContentType != null
                ? $"data:{user.ProfilePictureContentType};base64,{Convert.ToBase64String(user.ProfilePicture)}"
                : null;

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
                    UserProfilePictureUrl = profilePictureUrl,
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
                ProfilePictureUrl = profilePictureUrl,
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

        public async Task<bool> UpdateProfilePictureAsync(string userId, byte[] imageData, string contentType)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.ProfilePicture = imageData;
            user.ProfilePictureContentType = contentType;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateUsernameAsync(string userId, string newUsername)
        {
            // Validate username format
            if (string.IsNullOrWhiteSpace(newUsername) || newUsername.Length < 3 || newUsername.Length > 20)
            {
                return (false, "Username must be between 3 and 20 characters");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(newUsername, @"^[a-zA-Z0-9_]+$"))
            {
                return (false, "Username can only contain letters, numbers, and underscores");
            }

            // Check if username is already taken
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == newUsername && u.Id != userId);
            if (existingUser != null)
            {
                return (false, "Username is already taken");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            user.UserName = newUsername;
            user.NormalizedUserName = newUsername.ToUpperInvariant();
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}
