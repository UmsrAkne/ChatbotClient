using System;

namespace ChatbotClient.Models
{
    public class TalkEntry
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty; // 発言者種別

        public DateTime Timestamp { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));

        // 外部キー
        public int TalkSessionId { get; set; }

        // ナビゲーションプロパティ（多対1）
        public TalkSession TalkSession { get; set; }

        public AiModelType AiModelType { get; set; }

        public int SystemPromptId { get; set; }

        public int Index { get; set; }

        public bool IsFavorite { get; set; }

        public string GenerationId { get; set; }
    }
}