namespace ChirpSocial.Business.Models
{
    public class ChirpDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public List<string> Peeps { get; set; } = new();
    }
}
