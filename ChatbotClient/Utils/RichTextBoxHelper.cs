using System;
using System.Windows;
using System.Windows.Documents;
using Markdig;

namespace ChatbotClient.Utils
{
    public static class RichTextBoxHelper
    {
        public readonly static DependencyProperty DocumentBindingProperty =
            DependencyProperty.RegisterAttached(
                "DocumentBinding",
                typeof(FlowDocument),
                typeof(RichTextBoxHelper),
                new PropertyMetadata(null, OnDocumentBindingChanged));

        public static FlowDocument GetDocumentBinding(DependencyObject obj) => (FlowDocument)obj.GetValue(DocumentBindingProperty);

        public static void SetDocumentBinding(DependencyObject obj, FlowDocument value) => obj.SetValue(DocumentBindingProperty, value);

        public static FlowDocument ConvertMarkdown(string markdown)
        {
            // パイプライン（絵文字や高度な機能を有効化）を構築
            var pipeline = new MarkdownPipelineBuilder()
                .UseEmojiAndSmiley()
                .UseAdvancedExtensions()
                .Build();

            // Markdown文字列を FlowDocument に変換
            var doc = Markdig.Wpf.Markdown.ToFlowDocument(markdown, pipeline);

            ForceApplyTheme(doc);
            return doc;
        }

        private static void ForceApplyTheme(DependencyObject obj)
        {
            try
            {
                if (obj == null)
                {
                    return;
                }

                // 背景色とスタイルを持つ可能性のある全要素をチェック
                if (obj is FrameworkContentElement fce)
                {
                    // 1. まず、要素に勝手に割り当てられた Style を消去
                    fce.ClearValue(FrameworkContentElement.StyleProperty);

                    // 2. その上で、個別に書き込まれた背景色があればそれも消す
                    fce.ClearValue(TextElement.BackgroundProperty);
                }

                // 子要素を再帰的に走査（LogicalTreeを掘る）
                foreach (var child in LogicalTreeHelper.GetChildren(obj))
                {
                    if (child is not DependencyObject d)
                    {
                        continue;
                    }

                    ForceApplyTheme(d);
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
                throw;
            }
        }

        private static void OnDocumentBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // ターゲットが Emoji.Wpf.RichTextBox なら Document を差し替える
            if (d is Emoji.Wpf.RichTextBox rtb)
            {
                rtb.Document = (FlowDocument)e.NewValue ?? new FlowDocument();
            }
        }
    }
}