namespace ChirpSocial.Business.Models
{
    public class UserProfileDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public int TotalChirps { get; set; }
        public int LikesReceived { get; set; }
        public int LikesGiven { get; set; }
        public List<ChirpDto> RecentChirps { get; set; } = new();
    }
}
