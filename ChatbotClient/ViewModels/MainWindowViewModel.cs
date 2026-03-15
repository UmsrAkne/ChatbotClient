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

    public MainWindowViewModel()
    {
        DebugDummyData();
    }

    public string Title => appVersionInfo.Title;

    public string InputText { get => inputText; set => SetProperty(ref inputText, value); }

    public AsyncRelayCommand SendRequestCommand => new (async () =>
    {
        Console.WriteLine("コマンドが実行されました");

        try
        {
            await requestDispatcher.SendRequest(InputText);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    });

    [Conditional("DEBUG")]
    private void DebugDummyData()
    {
        InputText = "Hello";
    }
}