namespace ChirpSocial.Data
{
    public class Mention
    {
        public int Id { get; set; }
        public int ChirpId { get; set; }
        public Chirp Chirp { get; set; } = null!;
        public string MentionedUserId { get; set; } = string.Empty;
        public ApplicationUser MentionedUser { get; set; } = null!;
    }
}
