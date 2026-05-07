using System;
using System.Security.Cryptography;
using System.Text;

namespace ChatbotClient.Models
{
    public class AiModel
    {
        private string modelName = string.Empty;

        public Guid Id { get; set; }

        public string ModelName
        {
            get => modelName;
            set
            {
                modelName = value;
                Id = GenerateGuidFromName(value);
            }
        }

        private static Guid GenerateGuidFromName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Guid.Empty;
            }

            // 文字列をハッシュ化 (SHA256)
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(name));

            // Guid は 16バイト必要なので、ハッシュ結果の先頭 16バイトを使用
            var guidBytes = new byte[16];
            Array.Copy(hashBytes, guidBytes, 16);

            return new Guid(guidBytes);
        }
    }
}