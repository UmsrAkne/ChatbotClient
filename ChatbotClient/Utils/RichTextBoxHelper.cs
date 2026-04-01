using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
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
                    // Style を消去する前に、コードブロックっぽいかどうか判定する
                    var isCodeBlock = IsCodeElement(fce);
                    var currentFont = fce.GetValue(TextElement.FontFamilyProperty) as FontFamily;
                    var originalFontSize = fce.GetValue(TextElement.FontSizeProperty);

                    // 1. 要素に勝手に割り当てられた Style を消去
                    fce.ClearValue(FrameworkContentElement.StyleProperty);

                    // 2. その上で、個別に書き込まれた背景色があればそれも消す
                    fce.ClearValue(TextElement.BackgroundProperty);

                    // 前処理でコードブロック判定されていればスタイルを再適用
                    if (isCodeBlock)
                    {
                        ApplyCodeBlockTheme(fce, currentFont);
                    }

                    if (originalFontSize != null)
                    {
                        fce.SetValue(TextElement.FontSizeProperty, originalFontSize);
                    }
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

        // 判定ロジック：等幅フォントが含まれているか
        private static bool IsCodeElement(FrameworkContentElement fce)
        {
            if (fce is TextElement te)
            {
                var fontName = te.FontFamily.Source.ToLower();
                return fontName.Contains("consolas") || fontName.Contains("courier") || fontName.Contains("mono");
            }

            return false;
        }

        // コードブロック専用の見た目をセット
        private static void ApplyCodeBlockTheme(FrameworkContentElement fce, FontFamily currentFont)
        {
            // 背景色（ダークテーマなら濃いグレーなど）
            fce.SetValue(
                TextElement.BackgroundProperty,
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D") !));

            // 文字色（読みやすいように明るく）
            fce.SetValue(TextElement.ForegroundProperty, Brushes.LightGray);

            // 段落（Paragraph）なら余白やインデントを調整
            if (fce is Paragraph para)
            {
                para.Margin = new Thickness(5, 5, 5, 5);
                para.Padding = new Thickness(10);
            }

            if (currentFont != null)
            {
                fce.SetValue(TextElement.FontFamilyProperty, currentFont);
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