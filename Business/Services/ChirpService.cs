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
                .Include(c => c.ChirpPeeps)
                    .ThenInclude(cp => cp.Peep)
                .OrderByDescending(c => c.CreatedAt)
                .Take(count)
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

            await _context.SaveChangesAsync();

            await _context.Entry(chirp).Reference(c => c.User).LoadAsync();

            return MapToDto(chirp, userId);
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

        private List<string> ExtractPeeps(string content)
        {
            var regex = new Regex(@"<([^>]+)>");
            var matches = regex.Matches(content);
            return matches.Select(m => m.Groups[1].Value.ToLower()).Distinct().ToList();
        }

        private ChirpDto MapToDto(Chirp chirp, string? currentUserId)
        {
            return new ChirpDto
            {
                Id = chirp.Id,
                Content = chirp.Content,
                CreatedAt = chirp.CreatedAt,
                UserId = chirp.UserId,
                UserName = chirp.User?.UserName ?? "Unknown",
                LikeCount = chirp.Likes?.Count ?? 0,
                IsLikedByCurrentUser = currentUserId != null && (chirp.Likes?.Any(l => l.UserId == currentUserId) ?? false),
                Peeps = chirp.ChirpPeeps?.Select(cp => cp.Peep.Tag).ToList() ?? new List<string>()
            };
        }
    }
}
