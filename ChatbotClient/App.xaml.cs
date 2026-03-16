using System;
using System.Windows;
using ChatbotClient.Data;
using ChatbotClient.Views;
using Prism.Ioc;

namespace ChatbotClient;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        Console.WriteLine("RegisterTypes");
        containerRegistry.Register<MyDbContext>();
        containerRegistry.Register<ITalkRepository, TalkRepository>();
    }

    protected override void OnInitialized()
    {
        Console.WriteLine("OnInitialized");
        base.OnInitialized();
        var db = Container.Resolve<MyDbContext>();
        db.Database.EnsureCreated();
    }
}