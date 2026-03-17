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

    public ObservableCollection<TalkEntry> Talks { get; set; } = new ();

    public AsyncRelayCommand SendRequestCommand => new (async () =>
    {
        Console.WriteLine("コマンドが実行されました");

        // 1. まず自分の発言を UI (Talks) に追加
        var userEntry = new TalkEntry
        {
            Content = InputText,
            Role = "User",
            Timestamp = DateTime.Now,
        };

        Talks.Add(userEntry);
        await talkRepository.AddEntryAsync(CurrentSession.Id, userEntry);

        // 2. 入力欄をクリア（連打防止）
        var originalText = InputText;
        InputText = string.Empty;

        try
        {
            var modelName = OpenRouterModels.GetModelId(CurrentModel);
            var request = new TalkRequest { Message = originalText, ModelName = modelName };

            // 3. 通信開始
            var result = await requestDispatcher.SendRequest(request);

            // 4. AIの返答を UI と DB に登録
            await RegisterChat(result);
        }
        catch (Exception e)
        {
            // 5. 異常系のハンドリング
            // 失敗したことを示す「システムメッセージ」を Talks に入れると親切
            Talks.Add(new TalkEntry
            {
                Content = $"エラーが発生しました: {e.Message}",
                Role = "System",
            });
        }
    });

    private async Task RegisterChat(string chatMessage)
    {
        if (CurrentSession == null)
        {
            // 基本的に CurrentSession は Null にならないはずだけどガードする。
            Console.WriteLine("Warning: CurrentSession is null. Cannot register chat.");
            return;
        }

        var entry = new TalkEntry
        {
            Content = string.IsNullOrWhiteSpace(chatMessage) ? "Empty message" : chatMessage,
            Role = "assistant",
        };

        await talkRepository.AddEntryAsync(CurrentSession.Id, entry);
        Talks.Add(entry);
    }

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

            if (CurrentSession != null)
            {
                var ts = await talkRepository.GetEntriesBySessionIdAsync(CurrentSession.Id);
                Talks.AddRange(ts);
            }
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