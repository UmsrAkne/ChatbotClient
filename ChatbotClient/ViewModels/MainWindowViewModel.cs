using System;
using System.Diagnostics;
using ChatbotClient.Core;
using ChatbotClient.Utils;
using CommunityToolkit.Mvvm.Input;
using Prism.Mvvm;

namespace ChatbotClient.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private readonly AppVersionInfo appVersionInfo = new ();
    private readonly RequestDispatcher requestDispatcher = new ();
    private string inputText;
    private string responseText;

    public MainWindowViewModel()
    {
        DebugDummyData();
    }

    public string Title => appVersionInfo.Title;

    public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

    public string ResponseText { get => responseText; set => SetProperty(ref responseText, value); }

    public AsyncRelayCommand SendRequestCommand => new (async () =>
    {
        Console.WriteLine("コマンドが実行されました");

        try
        {
            var result = await requestDispatcher.SendRequest(InputText);
            ResponseText = string.IsNullOrWhiteSpace(result) ? "(空の応答)" : result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            ResponseText = $"エラー: {e.Message}";
        }
    });

    [Conditional("DEBUG")]
    private void DebugDummyData()
    {
        InputText = "Hello";
        ResponseText = "ここに応答が表示されます";
    }
}