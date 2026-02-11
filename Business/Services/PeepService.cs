using ChirpSocial.Business.Models;
using ChirpSocial.Data;
using Microsoft.EntityFrameworkCore;

namespace ChirpSocial.Business.Services
{
    public class PeepService
    {
        private readonly ApplicationDbContext _context;

        public PeepService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PeepStatsDto>> GetTopPeepsLast24HoursAsync(int count = 5)
        {
            var oneDayAgo = DateTime.UtcNow.AddDays(-1);

            var topPeeps = await _context.Chirps
                .Where(c => c.CreatedAt >= oneDayAgo)
                .SelectMany(c => c.ChirpPeeps)
                .GroupBy(cp => cp.Peep.Tag)
                .Select(g => new PeepStatsDto
                {
                    Tag = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(p => p.Count)
                .Take(count)
                .ToListAsync();

            return topPeeps;
        }
    }
}
