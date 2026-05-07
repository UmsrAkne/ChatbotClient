using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatbotClient.Models;

namespace ChatbotClient.Data
{
    public interface IAiModelRepository
    {
        Task<List<AiModel>> GetAllAsync();
        Task<AiModel?> GetByIdAsync(Guid id);
        Task AddAsync(AiModel model);
        Task UpdateAsync(AiModel model);
        Task DeleteAsync(Guid id);
    }
}