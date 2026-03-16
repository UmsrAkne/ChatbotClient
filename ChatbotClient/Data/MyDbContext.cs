using ChatbotClient.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotClient.Data
{
    public class MyDbContext : DbContext
    {
        public DbSet<TalkSession> TalkSessions { get; set; } = null!;

        public DbSet<TalkEntry> TalkEntries { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=talk_history.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TalkSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasMany(e => e.Entries)
                      .WithOne(e => e.TalkSession!)
                      .HasForeignKey(e => e.TalkSessionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TalkEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
            });
        }
    }
}