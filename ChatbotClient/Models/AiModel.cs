using System;

namespace ChatbotClient.Models
{
    public class AiModel
    {
        public Guid Id { get; set; }

        public string ModelName { get; set; } = string.Empty;
    }
}