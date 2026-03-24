using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ChatbotClient.Models
{
    public class SystemPromptEntry
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = Guid.NewGuid();

        public string PromptText { get; set; }

        public string Hash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public static string NormalizeAndHash(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return string.Empty;
            }

            // 前後の空白を消し、改行コードを \n に統一
            var normalized = prompt.Trim().Replace("\r\n", "\n").Replace("\r", "\n");

            // 3連続以上の改行を2つに
            normalized = Regex.Replace(normalized, @"\n{3,}", "\n\n");

            var inputBytes = Encoding.UTF8.GetBytes(normalized);
            var hashBytes = SHA256.HashData(inputBytes);
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }
}