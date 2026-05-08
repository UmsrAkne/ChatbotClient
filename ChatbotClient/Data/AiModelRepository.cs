#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatbotClient.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotClient.Data
{
    public class AiModelRepository : IAiModelRepository
    {
        private readonly MyDbContext context;

        public AiModelRepository(MyDbContext context)
        {
            this.context = context;
        }

        public async Task<List<AiModel>> GetAllAsync()
        {
            return await context.AiModels.AsNoTracking().ToListAsync();
        }

        public async Task<AiModel?> GetByIdAsync(Guid id)
        {
            return await context.AiModels.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(AiModel model)
        {
            await context.AiModels.AddAsync(model);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AiModel model)
        {
            context.AiModels.Update(model);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var model = await context.AiModels.FindAsync(id);
            if (model != null)
            {
                context.AiModels.Remove(model);
                await context.SaveChangesAsync();
            }
        }
    }
}