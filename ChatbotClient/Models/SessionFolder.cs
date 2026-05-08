using System;

namespace ChatbotClient.Models
{
    public class SessionFolder
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ParentFolderId { get; set; }

        public string Name { get; set; }
    }
}