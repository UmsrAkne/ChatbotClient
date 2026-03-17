using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChatbotClient.Core;
using ChatbotClient.Data;
using ChatbotClient.Models;
using ChatbotClient.Utils;
using CommunityToolkit.Mvvm.Input;
using Prism.Mvvm;

namespace ChatbotClient.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private readonly AppVersionInfo appVersionInfo = new ();
    private readonly RequestDispatcher requestDispatcher = new ();
    private readonly ITalkRepository talkRepository;
    private string inputText;
    private string responseText;
    private AiModelType currentModel;
    private TalkSession currentSession;

    public MainWindowViewModel(ITalkRepository talkRepository)
    {
        AvailableModels = new ObservableCollection<AiModelType>()
        {
            AiModelType.GeminiFlashLite,
            AiModelType.GeminiFlash,
            AiModelType.GemmaFree,
            AiModelType.LlamaFree,
            AiModelType.ChatGPT4o,
        };

        CurrentModel = AvailableModels.First();
        this.talkRepository = talkRepository;

        _ = InitializeAsync();
        DebugDummyData();
    }

    /// <summary>
    /// Used for XAML design-time only.
    /// </summary>
    public MainWindowViewModel()
    {
        DebugDummyData();
    }

    public string Title => appVersionInfo.Title;

    public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

    public string ResponseText { get => responseText; set => SetProperty(ref responseText, value); }

    public ObservableCollection<AiModelType> AvailableModels { get; init; }

    public AiModelType CurrentModel { get => currentModel; set => SetProperty(ref currentModel, value); }

    public ObservableCollection<TalkSession> Sessions { get; set; } = new ();

    public TalkSession CurrentSession { get => currentSession; set => SetProperty(ref currentSession, value); }

    public AsyncRelayCommand SendRequestCommand => new (async () =>
    {
        Console.WriteLine("コマンドが実行されました");

        try
        {
            var modelName = OpenRouterModels.GetModelId(CurrentModel);
            var request = new TalkRequest { Message = InputText, ModelName = modelName, };
            var result = await requestDispatcher.SendRequest(request);
            ResponseText = string.IsNullOrWhiteSpace(result) ? "(空の応答)" : result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            ResponseText = $"エラー: {e.Message}";
        }
    });

    private async Task InitializeAsync()
    {
        try
        {
            await talkRepository.AddSessionAsync(new TalkSession
            {
                Title = $"Session {DateTime.Now.ToShortTimeString()}",
            });

            Console.WriteLine("Session added successfully.");

            var ss = await talkRepository.GetSessionsAsync();
            Sessions.AddRange(ss);
            CurrentSession = Sessions.FirstOrDefault();
        }
        catch (Exception ex)
        {
            // ここでようやくエラーが表面化する！
            Console.WriteLine($"DB Error: {ex.Message}");
        }
    }

    [Conditional("DEBUG")]
    private void DebugDummyData()
    {
        InputText = "Hello";
        ResponseText = "ここに応答が表示されます";
    }
}