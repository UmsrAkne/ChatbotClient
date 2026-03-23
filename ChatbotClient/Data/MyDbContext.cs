using System;
using System.IO;
using ChatbotClient.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotClient.Data
{
    public class MyDbContext : DbContext
    {
        public DbSet<TalkSession> TalkSessions { get; set; } = null!;

        public DbSet<TalkEntry> TalkEntries { get; set; } = null!;

        public DbSet<SystemPromptEntry> SystemPrompts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var baseDir = AppContext.BaseDirectory;
                var dbPath = Path.Combine(baseDir, "talk_history.db");
                Console.WriteLine($"DB Path: {dbPath}");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
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
                      .HasForeignKey(e => e.TalkSessionGuid)
                      .HasPrincipalKey(e => e.Guid)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TalkEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();

                entity.Property(e => e.SystemPromptGuid).IsRequired(false);
                entity.HasOne(e => e.SystemPrompt)
                    .WithMany()
                    .HasForeignKey(e => e.SystemPromptGuid)
                    .HasPrincipalKey(s => s.Guid);
            });
        }
    }
}