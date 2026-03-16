using System.Collections.Generic;
using System.Threading.Tasks;
using ChatbotClient.Models;

namespace ChatbotClient.Data
{
    /// <summary>
    /// Repository interface to manage TalkSession and TalkEntry entities asynchronously.
    /// </summary>
    public interface ITalkRepository
    {
        // Read
        Task<List<TalkSession>> GetSessionsAsync();
        Task<List<TalkEntry>> GetEntriesBySessionIdAsync(int sessionId);

        // Create
        Task<TalkSession> AddSessionAsync(TalkSession session);
        Task<TalkEntry> AddEntryAsync(int sessionId, TalkEntry entry);

        // Delete
        Task DeleteSessionAsync(int sessionId);
    }
}