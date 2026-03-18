using System.Collections.Generic;
using OpenAI.Chat;

namespace ChatbotClient.Models
{
    public class TalkRequest
    {
        public string Message { get; set; }

        public string ModelName { get; set; }

        public string SystemPrompt { get; set; }

        public List<ChatMessage> GeneratedMessages()
        {
            var result = new List<ChatMessage>
            {
                SystemPrompt,
                Message,
            };

            return result;
        }
    }
}