using System;
using System.Collections.Generic;
using System.Linq;
using OpenAI.Chat;

namespace ChatbotClient.Models
{
    public class TalkRequest
    {
        // 今回のユーザー発話
        public string Message { get; set; } = string.Empty;

        public string ModelName { get; set; } = string.Empty;

        public string SystemPrompt { get; set; } = string.Empty;

        /// <summary>
        /// リクエストに含める履歴の件数です。ただし、システムメッセージは件数に含まれません。
        /// </summary>
        public int MessageLimit { get; set; } = 10;

        // 過去履歴（現在のセッション内の発言一覧）
        public IReadOnlyList<TalkEntry> History { get; set; } = new List<TalkEntry>();

        public List<ChatMessage> GeneratedMessages()
        {
            var result = new List<ChatMessage>();

            // 1) system
            if (!string.IsNullOrWhiteSpace(SystemPrompt))
            {
                result.Add(ChatMessage.CreateSystemMessage(SystemPrompt));
            }

            // 2) 履歴（古い順）を整備し、systemを除いた直近maxTurns件にトリム
            var ordered = History
                .OrderBy(h => h.Timestamp)
                .ToList();

            var nonSystemHistory = ordered
                .Where(h => !string.Equals(h.Role, "system", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (nonSystemHistory.Count > MessageLimit)
            {
                // 残す先頭のエントリ（非system基準）
                var keepFirst = nonSystemHistory[nonSystemHistory.Count - MessageLimit];
                var startIndex = ordered.IndexOf(keepFirst);
                if (startIndex > 0)
                {
                    ordered = ordered.Skip(startIndex).ToList();
                }
            }

            Console.WriteLine($"Context length: {MessageLimit}");
            Console.WriteLine($"History: {ordered.Count} entries");

            // 過去の TalkEntry → ChatMessage（role 変換）
            foreach (var h in ordered)
            {
                var role = (h.Role ?? string.Empty).ToLowerInvariant();
                var content = h.Content ?? string.Empty;
                switch (role)
                {
                    case "user":
                        result.Add(ChatMessage.CreateUserMessage(content));
                        break;
                    case "assistant":
                        result.Add(ChatMessage.CreateAssistantMessage(content));
                        break;
                    case "system":
                        // 履歴内の system も必要なら追加
                        result.Add(ChatMessage.CreateSystemMessage(content));
                        break;
                    default:
                        result.Add(ChatMessage.CreateUserMessage(content));
                        break;
                }
            }

            // 3) 今回のユーザー発話（末尾）
            if (!string.IsNullOrWhiteSpace(Message))
            {
                result.Add(ChatMessage.CreateUserMessage(Message));
            }

            return result;
        }
    }
}