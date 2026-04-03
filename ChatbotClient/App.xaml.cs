using System;
using System.Windows;
using ChatbotClient.Data;
using ChatbotClient.Utils;
using ChatbotClient.ViewModels;
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
        var path = AppContext.BaseDirectory;
        Logger.Initialize(path);

        // --- データベース初期化セクション ---
        // 【事故メモ】
        // 以前、OnInitialized で EnsureCreated を呼んでいたが、以下の理由でエラー（No such table）が発生した：
        // 1. TalkRepository が DbContext に依存しており、DIコンテナが登録時や初期化のごく初期に
        //    TalkRepository (および DbContext) のインスタンスを生成してしまう。
        // 2. その結果、OnInitialized の実行（テーブル作成）よりも先に DB へのアクセスが走り、
        //    「ファイルはあるがテーブルがない」という不整合でクラッシュした。
        //
        // 【対策】
        // 最も確実な「実行順序」を担保するため、このメソッド内で型の登録と同時に物理的なテーブル作成を強制する。
        Logger.Log("RegisterTypes");
        Logger.Log("RegisterTypes: DB初期化開始");

        containerRegistry.Register<MyDbContext>();

        // OnInitializedよりも先に実行されるため、後続のリポジトリ作成でコケない
        using (var db = new MyDbContext())
        {
            db.Database.EnsureCreated();
        }

        containerRegistry.Register<ITalkRepository, TalkRepository>();
        containerRegistry.Register<SessionListBoxViewModel>();
    }

    protected override void OnInitialized()
    {
        Logger.Log("OnInitialized");
        base.OnInitialized();
    }
}