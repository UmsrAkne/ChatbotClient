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
            return Markdig.Wpf.Markdown.ToFlowDocument(markdown, pipeline);
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