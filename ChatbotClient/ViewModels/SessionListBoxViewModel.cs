using System.Collections.ObjectModel;
using ChatbotClient.Models;
using Prism.Mvvm;

namespace ChatbotClient.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SessionListBoxViewModel : BindableBase
    {
        private TalkSession currentSession;

        public TalkSession CurrentSession { get => currentSession; set => SetProperty(ref currentSession, value); }

        public ObservableCollection<TalkSession> Sessions { get; set; } = new ();
    }
}