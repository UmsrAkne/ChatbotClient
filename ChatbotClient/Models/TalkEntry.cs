using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using ChatbotClient.Utils;
using CommunityToolkit.Mvvm.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace ChatbotClient.Models
{
    public class TalkEntry : BindableBase
    {
        private FlowDocument displayDocument;
        private AsyncRelayCommand copyToClipboardCommand;
        private bool nowCopy;

        public TalkEntry()
        {
        }

        public TalkEntry(string content, bool isUserTalk)
            : this()
        {
            Content = content;
            Role = isUserTalk ? "User" : "Assistant";
            DisplayDocument = RichTextBoxHelper.ConvertMarkdown(content);
        }

        public int Id { get; set; }

        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Content { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty; // 発言者種別

        public DateTime Timestamp { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));

        // 外部キー
        public Guid TalkSessionGuid { get; set; }

        public SystemPromptEntry SystemPrompt { get; set; }

        // ナビゲーションプロパティ（多対1）
        public TalkSession TalkSession { get; set; }

        public AiModelType AiModelType { get; set; } = AiModelType.None;

        public Guid? SystemPromptGuid { get; set; }

        public int Index { get; set; }

        public bool IsFavorite { get; set; }

        public string GenerationId { get; set; }

        [NotMapped]
        public FlowDocument DisplayDocument
        {
            get
            {
                // 参照されたときだけ生成する。
                if (displayDocument == null && !string.IsNullOrEmpty(Content))
                {
                    displayDocument = RichTextBoxHelper.ConvertMarkdown(Content);
                }

                return displayDocument;
            }
            set => displayDocument = value;
        }

        [NotMapped]
        public string DisplayName => AiModelType == AiModelType.None ? Role : AiModelType.ToString();

        [NotMapped]
        public bool NowCopy { get => nowCopy; set => SetProperty(ref nowCopy, value); }

        [NotMapped]
        public AsyncRelayCommand CopyToClipboardAsyncCommand =>
        copyToClipboardCommand ??= new AsyncRelayCommand(async () =>
        {
            try
            {
                Clipboard.SetText(Content ?? string.Empty);
                NowCopy = true;

                // 数秒後にチェックマークを元に戻す
                await Task.Delay(3000);
            }
            catch (Exception ex)
            {
                Logger.Log($"Copy to clipboard failed: {ex.Message}");
            }
            finally
            {
                NowCopy = false;
            }
        });
    }
}