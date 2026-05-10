using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatbotClient.Data;
using ChatbotClient.Models;
using CommunityToolkit.Mvvm.Input;
using Prism.Mvvm;

namespace ChatbotClient.ViewModels
{
    public class AiModelEntryViewModel : BindableBase
    {
        private readonly IAiModelRepository aiModelRepository;
        private string newModelName = string.Empty;
        private ObservableCollection<AiModel> aiModels = new ();

        public AiModelEntryViewModel(IAiModelRepository aiModelRepository)
        {
            this.aiModelRepository = aiModelRepository;
            _ = InitializeAsync();
        }

        public string NewModelName { get => newModelName; set => SetProperty(ref newModelName, value); }

        public ObservableCollection<AiModel> AiModels { get => aiModels; set => SetProperty(ref aiModels, value); }

        public ICommand AddModelCommand => new AsyncRelayCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(NewModelName))
            {
                return;
            }

            var newModel = new AiModel { ModelName = NewModelName.Trim(), };
            await aiModelRepository.AddAsync(newModel);
            AiModels.Add(newModel);
            NewModelName = string.Empty;
        });

        private async Task InitializeAsync()
        {
            var models = await aiModelRepository.GetAllAsync();
            AiModels = new ObservableCollection<AiModel>(models);
        }
    }
}