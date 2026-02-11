using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChirpSocial.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Chirp> Chirps { get; set; }
        public DbSet<Peep> Peeps { get; set; }
        public DbSet<Like> Likes { get; set; }

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
        }
    }
}
