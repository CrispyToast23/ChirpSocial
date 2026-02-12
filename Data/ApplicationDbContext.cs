using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChirpSocial.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Chirp> Chirps { get; set; }
        public DbSet<Peep> Peeps { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Mention> Mentions { get; set; }
        public DbSet<ChirpImage> ChirpImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChirpPeep>()
                .HasKey(cp => new { cp.ChirpId, cp.PeepId });

            modelBuilder.Entity<ChirpPeep>()
                .HasOne(cp => cp.Chirp)
                .WithMany(c => c.ChirpPeeps)
                .HasForeignKey(cp => cp.ChirpId);

            modelBuilder.Entity<ChirpPeep>()
                .HasOne(cp => cp.Peep)
                .WithMany(p => p.ChirpPeeps)
                .HasForeignKey(cp => cp.PeepId);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.ChirpId, l.UserId })
                .IsUnique();

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mention>()
                .HasOne(m => m.MentionedUser)
                .WithMany()
                .HasForeignKey(m => m.MentionedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mention>()
                .HasOne(m => m.Chirp)
                .WithMany(c => c.Mentions)
                .HasForeignKey(m => m.ChirpId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChirpImage>()
                .HasOne(ci => ci.Chirp)
                .WithMany(c => c.Images)
                .HasForeignKey(ci => ci.ChirpId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
