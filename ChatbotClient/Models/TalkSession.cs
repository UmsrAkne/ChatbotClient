using System;
using System.Collections.Generic;

namespace ChatbotClient.Models
{
    public class TalkSession
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 1対多のナビゲーションプロパティ
        public ICollection<TalkEntry> Entries { get; set; } = new List<TalkEntry>();
    }
}