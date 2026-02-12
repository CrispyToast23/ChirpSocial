using ChirpSocial.Business.Models;
using ChirpSocial.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ChirpSocial.Business.Services
{
    public class ChirpService
    {
        private readonly ApplicationDbContext _context;

        public ChirpService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChirpDto>> GetRecentChirpsAsync(int count = 50, string? currentUserId = null)
        {
            var chirps = await _context.Chirps
                .Include(c => c.User)
                .Include(c => c.Likes)
                .Include(c => c.Comments)
                .Include(c => c.ChirpPeeps)
                    .ThenInclude(cp => cp.Peep)
                .Include(c => c.Mentions)
                    .ThenInclude(m => m.MentionedUser)
                .Include(c => c.Images)
                .OrderByDescending(c => c.CreatedAt)
                .Take(count)
                .ToListAsync();

            return chirps.Select(c => MapToDto(c, currentUserId)).ToList();
        }

        public async Task<List<ChirpDto>> GetChirpsByPeepAsync(string peepTag, string? currentUserId = null)
        {
            var chirps = await _context.Chirps
                .Include(c => c.User)
                .Include(c => c.Likes)
                .Include(c => c.Comments)
                .Include(c => c.ChirpPeeps)
                    .ThenInclude(cp => cp.Peep)
                .Include(c => c.Mentions)
                    .ThenInclude(m => m.MentionedUser)
                .Include(c => c.Images)
                .Where(c => c.ChirpPeeps.Any(cp => cp.Peep.Tag == peepTag.ToLower()))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return chirps.Select(c => MapToDto(c, currentUserId)).ToList();
        }

        public async Task<ChirpDto?> CreateChirpAsync(string content, string userId)
        {
            if (string.IsNullOrWhiteSpace(content) || content.Length > 123)
                return null;

            var chirp = new Chirp
            {
                Content = content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Chirps.Add(chirp);

            var peepTags = ExtractPeeps(content);
            foreach (var tag in peepTags)
            {
                var peep = await _context.Peeps.FirstOrDefaultAsync(p => p.Tag == tag);
                if (peep == null)
                {
                    peep = new Peep { Tag = tag };
                    _context.Peeps.Add(peep);
                }

                chirp.ChirpPeeps.Add(new ChirpPeep { Chirp = chirp, Peep = peep });
            }

            var mentionedUsernames = ExtractMentions(content);
            foreach (var username in mentionedUsernames)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
                if (user != null)
                {
                    chirp.Mentions.Add(new Mention
                    {
                        Chirp = chirp,
                        MentionedUserId = user.Id
                    });
                }
            }

            await _context.SaveChangesAsync();

            await _context.Entry(chirp).Reference(c => c.User).LoadAsync();
            await _context.Entry(chirp).Collection(c => c.Mentions).Query().Include(m => m.MentionedUser).LoadAsync();

            return MapToDto(chirp, userId);
        }

        public async Task<ChirpDto?> UpdateChirpAsync(int chirpId, string content, string userId)
        {
            if (string.IsNullOrWhiteSpace(content) || content.Length > 123)
                return null;

            var chirp = await _context.Chirps
                .Include(c => c.ChirpPeeps)
                .Include(c => c.Mentions)
                .FirstOrDefaultAsync(c => c.Id == chirpId && c.UserId == userId);
            
            if (chirp == null)
                return null;

            chirp.Content = content;

            var existingPeeps = chirp.ChirpPeeps.ToList();
            foreach (var cp in existingPeeps)
            {
                _context.Remove(cp);
            }

            var existingMentions = chirp.Mentions.ToList();
            foreach (var m in existingMentions)
            {
                _context.Remove(m);
            }

            var peepTags = ExtractPeeps(content);
            foreach (var tag in peepTags)
            {
                var peep = await _context.Peeps.FirstOrDefaultAsync(p => p.Tag == tag);
                if (peep == null)
                {
                    peep = new Peep { Tag = tag };
                    _context.Peeps.Add(peep);
                }

                chirp.ChirpPeeps.Add(new ChirpPeep { Chirp = chirp, Peep = peep });
            }

            var mentionedUsernames = ExtractMentions(content);
            foreach (var username in mentionedUsernames)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
                if (user != null)
                {
                    chirp.Mentions.Add(new Mention
                    {
                        Chirp = chirp,
                        MentionedUserId = user.Id
                    });
                }
            }

            await _context.SaveChangesAsync();

            await _context.Entry(chirp).Reference(c => c.User).LoadAsync();
            await _context.Entry(chirp).Collection(c => c.Likes).LoadAsync();
            await _context.Entry(chirp).Collection(c => c.Comments).LoadAsync();
            await _context.Entry(chirp).Collection(c => c.ChirpPeeps).Query().Include(cp => cp.Peep).LoadAsync();
            await _context.Entry(chirp).Collection(c => c.Mentions).Query().Include(m => m.MentionedUser).LoadAsync();

            return MapToDto(chirp, userId);
        }

        public async Task<bool> DeleteChirpAsync(int chirpId, string userId)
        {
            var chirp = await _context.Chirps.FirstOrDefaultAsync(c => c.Id == chirpId && c.UserId == userId);
            
            if (chirp == null)
                return false;

            _context.Chirps.Remove(chirp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleLikeAsync(int chirpId, string userId)
        {
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.ChirpId == chirpId && l.UserId == userId);

            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
            }
            else
            {
                _context.Likes.Add(new Like
                {
                    ChirpId = chirpId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return existingLike == null;
        }

        public async Task<List<string>> GetPeepSuggestionsAsync()
        {
            var oneDayAgo = DateTime.UtcNow.AddDays(-1);
            
            var peeps = await _context.Chirps
                .Where(c => c.CreatedAt >= oneDayAgo)
                .SelectMany(c => c.ChirpPeeps.Select(cp => cp.Peep.Tag))
                .ToListAsync();

            return peeps.Distinct().OrderBy(p => p).ToList();
        }

        public async Task<bool> AddImageToChirpAsync(int chirpId, string fileName, byte[] imageData, string contentType, long fileSize, string userId)
        {
            var chirp = await _context.Chirps.FirstOrDefaultAsync(c => c.Id == chirpId && c.UserId == userId);
            if (chirp == null)
                return false;

            var image = new ChirpImage
            {
                ChirpId = chirpId,
                FileName = fileName,
                ImageData = imageData,
                ContentType = contentType,
                FileSize = fileSize,
                UploadedAt = DateTime.UtcNow
            };

            _context.ChirpImages.Add(image);
            await _context.SaveChangesAsync();
            return true;
        }

        private List<string> ExtractPeeps(string content)
        {
            var regex = new Regex(@"<([^>]+)>");
            var matches = regex.Matches(content);
            return matches.Select(m => m.Groups[1].Value.ToLower()).Distinct().ToList();
        }

        private List<string> ExtractMentions(string content)
        {
            var regex = new Regex(@"@(\w+)");
            var matches = regex.Matches(content);
            return matches.Select(m => m.Groups[1].Value).Distinct().ToList();
        }

        private ChirpDto MapToDto(Chirp chirp, string? currentUserId)
        {
            var profilePictureUrl = chirp.User?.ProfilePicture != null && chirp.User.ProfilePictureContentType != null
                ? $"data:{chirp.User.ProfilePictureContentType};base64,{Convert.ToBase64String(chirp.User.ProfilePicture)}"
                : null;

            return new ChirpDto
            {
                Id = chirp.Id,
                Content = chirp.Content,
                CreatedAt = chirp.CreatedAt,
                UserId = chirp.UserId,
                UserName = chirp.User?.UserName ?? "Unknown",
                UserProfilePictureUrl = profilePictureUrl,
                LikeCount = chirp.Likes?.Count ?? 0,
                IsLikedByCurrentUser = currentUserId != null && (chirp.Likes?.Any(l => l.UserId == currentUserId) ?? false),
                Peeps = chirp.ChirpPeeps?.Select(cp => cp.Peep.Tag).ToList() ?? new List<string>(),
                CommentCount = chirp.Comments?.Count ?? 0,
                MentionedUserNames = chirp.Mentions?.Select(m => m.MentionedUser.UserName ?? string.Empty).Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new List<string>(),
                ImageUrls = chirp.Images?.Select(i => $"data:{i.ContentType};base64,{Convert.ToBase64String(i.ImageData)}").ToList() ?? new List<string>()
            };
        }
    }
}
