using Prism.Mvvm;

namespace ChatbotClient.Models
{
    public class AttachedFile : BindableBase
    {
        public string FileName { get; set; }

        public string FullPath { get; set; }

        // 送信直前にこれを呼ぶ
        public string GetLatestContent()
        {
            if (System.IO.File.Exists(FullPath))
            {
                return System.IO.File.ReadAllText(FullPath);
            }

            return string.Empty;
        }
    }
}