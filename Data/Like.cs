namespace ChirpSocial.Data
{
    public class Like
    {
        public int Id { get; set; }
        public int ChirpId { get; set; }
        public Chirp Chirp { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
