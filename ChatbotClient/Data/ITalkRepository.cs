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
        Task<List<TalkEntry>> GetEntriesBySessionIdAsync(Guid sessionGuid);

        // Create
        Task<TalkSession> AddSessionAsync(TalkSession session);
        Task<TalkEntry> AddEntryAsync(Guid sessionGuid, TalkEntry entry);

        // Delete
        Task DeleteSessionAsync(int sessionId);

        Task<SystemPromptEntry> GetOrAddSystemPromptEntryAsync(SystemPromptEntry entry);

        Task<IEnumerable<SystemPromptEntry>> GetRecentSystemPromptHistoryAsync(int count);
    }
}