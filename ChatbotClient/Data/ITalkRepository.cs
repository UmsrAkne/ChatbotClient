using System;
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

        Task<List<TalkEntry>> GetEntriesBySessionIdAsync(Guid sessionId);

        // Create
        Task<TalkSession> AddSessionAsync(TalkSession session);

        Task<TalkEntry> AddEntryAsync(Guid sessionId, TalkEntry entry);

        // Update
        Task UpdateSessionTitleAsync(Guid sessionId, string newTitle);

        // Delete
        Task DeleteSessionAsync(Guid sessionId);

        Task<SystemPromptEntry> GetOrAddSystemPromptEntryAsync(SystemPromptEntry entry);

        Task<IEnumerable<SystemPromptEntry>> GetRecentSystemPromptHistoryAsync(int count);
    }
}