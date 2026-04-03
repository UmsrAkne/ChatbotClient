using System.Collections.ObjectModel;
using ChatbotClient.Data;
using ChatbotClient.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ChatbotClient.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SessionListBoxViewModel : BindableBase
    {
        private TalkSession currentSession;
        private readonly ITalkRepository talkRepository;

        public SessionListBoxViewModel(ITalkRepository talkRepository)
        {
            this.talkRepository = talkRepository;
        }

        public TalkSession CurrentSession { get => currentSession; set => SetProperty(ref currentSession, value); }

        public ObservableCollection<TalkSession> Sessions { get; set; } = new ();

        public DelegateCommand<TalkSession> ToggleRenameModeCommand => new (target =>
        {
            foreach (var talkSession in Sessions)
            {
                talkSession.IsEditing = false;
            }

            target.IsEditing = true;
        });

        public DelegateCommand<TalkSession> ConfirmRenameCommand => new ((target) =>
        {
            target.IsEditing = false;
            talkRepository.UpdateSessionTitleAsync(target.Guid, target.Title);
        });
    }
}