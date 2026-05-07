using ChatbotClient.Data;
using Prism.Mvvm;

namespace ChatbotClient.ViewModels
{
    public class AiModelEntryViewModel : BindableBase
    {
        private readonly IAiModelRepository aiModelRepository;

        public AiModelEntryViewModel(IAiModelRepository aiModelRepository)
        {
            this.aiModelRepository = aiModelRepository;
        }
    }
}