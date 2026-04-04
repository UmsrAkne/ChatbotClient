using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ChatbotClient.Core;
using ChatbotClient.Data;
using ChatbotClient.Models;
using ChatbotClient.Utils;
using CommunityToolkit.Mvvm.Input;
using Prism.Commands;
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
    private int messageLimit = 10;
    private SystemPromptEntry currentSystemPrompt;
    private int currentHistoryIndex;

    public MainWindowViewModel(
        SessionListBoxViewModel sessionListBoxViewModel,
        ITalkRepository talkRepository)
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
        SessionListBoxViewModel = sessionListBoxViewModel;

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

    public SystemPromptEntry CurrentSystemPrompt
    {
        get => currentSystemPrompt;
        private set => SetProperty(ref currentSystemPrompt, value);
    }

    public ObservableCollection<TalkEntry> Talks { get; set; } = new ();

    public ObservableCollection<AttachedFile> AttachedFiles { get; set; } = new ();

    public int CurrentHistoryIndex
    {
        get => currentHistoryIndex;
        set => SetProperty(ref currentHistoryIndex, value);
    }

    public SessionListBoxViewModel SessionListBoxViewModel { get; set; }

    public AsyncRelayCommand LoadSessionAsyncCommand => new (async () =>
    {
        if (SessionListBoxViewModel.CurrentSession == null)
        {
            return;
        }

        var ts = await talkRepository.GetEntriesBySessionIdAsync(SessionListBoxViewModel.CurrentSession.Guid);
        var ordered = ts.OrderBy(t => t.Timestamp.ToLocalTime());
        await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            Talks.Clear();
            await Task.Delay(100);

            foreach (var t in ordered)
            {
                Talks.Add(t);
                await Task.Delay(50);
            }
        });
    });

    public DelegateCommand<object> BrowseHistoryCommand => new ((direction) =>
    {
        var d = int.Parse((string)direction);
        MapsToHistoryIndex(d);
    });

    public AsyncRelayCommand<string> SendRequestCommand => new (async text =>
    {
        Logger.Log("コマンドが実行されました");

        // 1. まず自分の発言を UI (Talks) に追加
        var userEntry = new TalkEntry(text, true);
        var systemPrompt = new SystemPromptEntry()
        {
            PromptText = SystemPromptFormatter.BuildSystemPrompt(CurrentSystemPrompt.PromptText, AttachedFiles.ToList()),
        };

        var sp = await talkRepository.GetOrAddSystemPromptEntryAsync(systemPrompt);
        userEntry.SystemPromptGuid = sp.Guid;

        Talks.Add(userEntry);
        await talkRepository.AddEntryAsync(SessionListBoxViewModel.CurrentSession.Guid, userEntry);

        // 2. 入力欄をクリア（連打防止）
        InputText = string.Empty;

        try
        {
            var modelName = OpenRouterModels.GetModelId(CurrentModel);
            var request = new TalkRequest
            {
                Message = text,
                ModelName = modelName,
                SystemPrompt = sp.PromptText,
                History = Talks.ToList(),
                MessageLimit = MessageLimit,
            };

            // 3. 通信開始
            var result = await requestDispatcher.SendRequest(request);
            result.DisplayDocument = RichTextBoxHelper.ConvertMarkdown(result.Content);
            result.AiModelType = CurrentModel;

            // 4. AIの返答を UI と DB に登録
            await RegisterChat(result);
        }
        catch (Exception e)
        {
            // 5. 異常系のハンドリング
            // 失敗したことを示す「システムメッセージ」を Talks に入れると親切
            Talks.Add(new TalkEntry($"エラーが発生しました: {e.Message}", false)
            {
                Role = "System",
            });
        }
    });

    public AsyncRelayCommand AddSessionCommand => new (async () =>
    {
        try
        {
            var newSession = await talkRepository.AddSessionAsync(new TalkSession
            {
                Title = $"Session {DateTime.Now.ToShortTimeString()}",
            });

            Logger.Log("Session added successfully.");

            await ReloadSessionsAsync(selectionId: newSession.Id);
        }
        catch (Exception e)
        {
            Logger.Log(e.ToString());
            throw;
        }
    });

    public RelayCommand AddDummyTalkCommand => new (() =>
    {
        AddDummyTalk();
    });

    public int MessageLimit { get => messageLimit; set => SetProperty(ref messageLimit, value); }

    private List<SystemPromptEntry> PromptHistory { get; set; }

    private void MapsToHistoryIndex(int direction)
    {
        if (PromptHistory == null || !PromptHistory.Any())
        {
            return;
        }

        var nextIndex = CurrentHistoryIndex + direction;

        if (nextIndex >= 0 && nextIndex < PromptHistory.Count)
        {
            CurrentHistoryIndex = nextIndex;
            CurrentSystemPrompt = new SystemPromptEntry()
                { PromptText = PromptHistory[CurrentHistoryIndex].PromptText, };
        }
    }

    private async Task RegisterChat(TalkEntry talkEntry)
    {
        if (SessionListBoxViewModel.CurrentSession == null)
        {
            // 基本的に CurrentSession は Null にならないはずだけどガードする。
            Logger.Log("Warning: CurrentSession is null. Cannot register chat.");
            return;
        }

        await talkRepository.AddEntryAsync(SessionListBoxViewModel.CurrentSession.Guid, talkEntry);
        Talks.Add(talkEntry);
    }

    private async Task InitializeAsync()
    {
        try
        {
            var list = await talkRepository.GetSessionsAsync();

            // セッションが一つもなければデフォルトを作成
            if (list.Count == 0)
            {
                await talkRepository.AddSessionAsync(new TalkSession()
                {
                    Title = "New Session",
                });
            }

            await ReloadSessionsAsync();

            // 前段の処理で最低一つは入っている前提なので First()
            SessionListBoxViewModel.CurrentSession ??= SessionListBoxViewModel.Sessions.First();

            await LoadSessionAsyncCommand.ExecuteAsync(null);

            CurrentSystemPrompt = new SystemPromptEntry
            {
                PromptText = "あなたは親切で優秀なアシスタントです。回答は簡潔に日本語で行ってください。",
            };

            var spList = await talkRepository.GetRecentSystemPromptHistoryAsync(5);
            PromptHistory = spList.ToList();
            CurrentHistoryIndex = PromptHistory.Count - 1;
        }
        catch (Exception ex)
        {
            // ここでようやくエラーが表面化する！
            Logger.Log($"DB Error: {ex.Message}");
        }
    }

    /// <summary>
    /// セッションのリストをリロードします。引数を入力した場合、該当IDのセッションを CurrentSession に代入します。
    /// </summary>
    /// <param name="selectionId">リロード直後に選択するセッションIDを指定します。</param>
    private async Task ReloadSessionsAsync(int selectionId = -1)
    {
        var ss = await talkRepository.GetSessionsAsync();
        SessionListBoxViewModel.Sessions.Clear();
        SessionListBoxViewModel.Sessions.AddRange(ss.OrderBy(s => s.CreatedAt));
        Logger.Log("Reload Sessions");

        if (selectionId >= 0)
        {
            SessionListBoxViewModel.CurrentSession = SessionListBoxViewModel.Sessions.FirstOrDefault(s => s.Id == selectionId);
        }
    }

    [Conditional("DEBUG")]
    private void DebugDummyData()
    {
        InputText = "Hello";
        ResponseText = "ここに応答が表示されます";

        SessionListBoxViewModel.Sessions.Add(new TalkSession() { Title = "Session 1", });
        SessionListBoxViewModel.Sessions.Add(new TalkSession() { Title = "Session 2", });

        Talks.Add(new TalkEntry("Hello", true));
        Talks.Add(new TalkEntry("Hello! How are you?", false));
    }

    [Conditional("DEBUG")]
    private void AddDummyTalk()
    {
        var dummyEntry = new TalkEntry(
            $"これはデバッグ用のダミーメッセージです。時刻: {DateTime.Now:HH:mm:ss}\n長い文章のテストも兼ねています。あああああああああああああああああああああああ。",
            Talks.Count % 2 == 0);

        Talks.Add(dummyEntry);
    }
}