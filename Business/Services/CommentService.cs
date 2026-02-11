using ChirpSocial.Business.Models;
using ChirpSocial.Data;
using Microsoft.EntityFrameworkCore;

namespace ChirpSocial.Business.Services
{
    public class CommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommentDto>> GetCommentsForChirpAsync(int chirpId)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.ChirpId == chirpId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                ChirpId = c.ChirpId,
                UserId = c.UserId,
                UserName = c.User?.UserName ?? "Unknown"
            }).ToList();
        }

        public async Task<CommentDto?> CreateCommentAsync(int chirpId, string content, string userId)
        {
            if (string.IsNullOrWhiteSpace(content) || content.Length > 280)
                return null;

            var comment = new Comment
            {
                Content = content,
                ChirpId = chirpId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            await _context.Entry(comment).Reference(c => c.User).LoadAsync();

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                ChirpId = comment.ChirpId,
                UserId = comment.UserId,
                UserName = comment.User?.UserName ?? "Unknown"
            };
        }

        public async Task<CommentDto?> UpdateCommentAsync(int commentId, string content, string userId)
        {
            if (string.IsNullOrWhiteSpace(content) || content.Length > 280)
                return null;

            var comment = await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
            
            if (comment == null)
                return null;

            comment.Content = content;
            await _context.SaveChangesAsync();

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                ChirpId = comment.ChirpId,
                UserId = comment.UserId,
                UserName = comment.User?.UserName ?? "Unknown"
            };
        }

        public async Task<bool> DeleteCommentAsync(int commentId, string userId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
            
            if (comment == null)
                return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
