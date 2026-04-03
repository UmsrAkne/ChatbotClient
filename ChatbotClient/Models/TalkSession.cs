using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Prism.Mvvm;

namespace ChatbotClient.Models
{
    public class TalkSession : BindableBase
    {
        private bool isEditing;
        private string title = string.Empty;

        public int Id { get; set; }

        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Title { get => title; set => SetProperty(ref title, value); }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 1対多のナビゲーションプロパティ
        public ICollection<TalkEntry> Entries { get; set; } = new List<TalkEntry>();

        [NotMapped]
        public bool IsEditing { get => isEditing; set => SetProperty(ref isEditing, value); }
    }
}