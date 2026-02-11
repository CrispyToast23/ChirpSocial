using Microsoft.AspNetCore.Identity;

namespace ChirpSocial.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? Bio { get; set; }
        public ICollection<Chirp> Chirps { get; set; } = new List<Chirp>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
