using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Documents;
using ChatbotClient.Utils;

namespace ChatbotClient.Models
{
    public class TalkEntry
    {
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

        public AiModelType AiModelType { get; set; }

        public Guid? SystemPromptGuid { get; set; }

        public int Index { get; set; }

        public bool IsFavorite { get; set; }

        public string GenerationId { get; set; }

        [NotMapped]
        public FlowDocument DisplayDocument { get; set; }
    }
}