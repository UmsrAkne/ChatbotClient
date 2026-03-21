using System;

namespace ChatbotClient.Models
{
    public class SystemPromptEntry
    {
        public int Id { get; set; }

        public string PromptText { get; set; }

        public string Hash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}