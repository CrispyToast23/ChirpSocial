namespace ChirpSocial.Data
{
    public class Chirp
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<ChirpPeep> ChirpPeeps { get; set; } = new List<ChirpPeep>();
    }
}
