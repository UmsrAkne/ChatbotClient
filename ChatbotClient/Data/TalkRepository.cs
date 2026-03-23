using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatbotClient.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotClient.Data
{
    /// <summary>
    /// Concrete repository implementation using Entity Framework Core.
    /// </summary>
    public class TalkRepository : ITalkRepository
    {
        private readonly MyDbContext db;

        public TalkRepository(MyDbContext db)
        {
            this.db = db;
        }

        // Read
        public Task<List<TalkSession>> GetSessionsAsync()
        {
            return db.TalkSessions
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public Task<List<TalkEntry>> GetEntriesBySessionIdAsync(Guid sessionGuid)
        {
            return db.TalkEntries
                .AsNoTracking()
                .Where(e => e.TalkSessionGuid == sessionGuid)
                .Include(e => e.SystemPrompt)
                .OrderBy(e => e.Index)
                .ThenBy(e => e.Timestamp)
                .ToListAsync();
        }

        // Create
        public async Task<TalkSession> AddSessionAsync(TalkSession session)
        {
            await db.TalkSessions.AddAsync(session).ConfigureAwait(false);
            await db.SaveChangesAsync().ConfigureAwait(false);
            return session;
        }

        public async Task<TalkEntry> AddEntryAsync(Guid sessionGuid, TalkEntry entry)
        {
            // ensure entry is linked to the specified session
            entry.TalkSessionGuid = sessionGuid;

            // Determine the next Index within this TalkSession to preserve order
            var currentMaxIndex = await db.TalkEntries
                .Where(e => e.TalkSessionGuid == sessionGuid)
                .Select(e => (int?)e.Index)
                .MaxAsync()
                .ConfigureAwait(false);

            entry.Index = currentMaxIndex.HasValue ? currentMaxIndex.Value + 1 : 0;

            await db.TalkEntries.AddAsync(entry).ConfigureAwait(false);
            await db.SaveChangesAsync().ConfigureAwait(false);
            return entry;
        }

        public async Task<SystemPromptEntry> GetOrAddSystemPromptEntryAsync(SystemPromptEntry entry)
        {
            var hash = SystemPromptEntry.NormalizeAndHash(entry.PromptText);

            // 既存のハッシュがあるかチェック
            var existingEntry = await db.SystemPrompts
                .FirstOrDefaultAsync(x => x.Hash == hash)
                .ConfigureAwait(false);

            if (existingEntry != null)
            {
                // 既存があればそれを返す（追加はしない）
                return existingEntry;
            }

            // 既存ハッシュなしの場合は追加
            entry.Hash = hash;
            await db.SystemPrompts.AddAsync(entry).ConfigureAwait(false);
            await db.SaveChangesAsync().ConfigureAwait(false);

            return entry;
        }

        // Delete
        public async Task DeleteSessionAsync(int sessionId)
        {
            var session = await db.TalkSessions.FindAsync(sessionId).ConfigureAwait(false);
            if (session == null)
            {
                return; // nothing to delete
            }

            db.TalkSessions.Remove(session);
            await db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}