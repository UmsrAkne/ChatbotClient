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

        public Task<List<TalkEntry>> GetEntriesBySessionIdAsync(int sessionId)
        {
            return db.TalkEntries
                .AsNoTracking()
                .Where(e => e.TalkSessionId == sessionId)
                .OrderBy(e => e.Timestamp)
                .ToListAsync();
        }

        // Create
        public async Task<TalkSession> AddSessionAsync(TalkSession session)
        {
            await db.TalkSessions.AddAsync(session).ConfigureAwait(false);
            await db.SaveChangesAsync().ConfigureAwait(false);
            return session;
        }

        public async Task<TalkEntry> AddEntryAsync(int sessionId, TalkEntry entry)
        {
            // ensure entry is linked to the specified session
            entry.TalkSessionId = sessionId;
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