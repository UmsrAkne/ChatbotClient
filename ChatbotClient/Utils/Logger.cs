using System;
using System.IO;

namespace ChatbotClient.Utils
{
    public static class Logger
    {
        private static string logFilePath;

        public static void Initialize(string logPath)
        {
            logFilePath = Path.Combine(logPath, "log.txt");
        }

        public static void Log(string message)
        {
            if (string.IsNullOrEmpty(logFilePath))
            {
                // DB のマイグレーション作成時等、logFilePath が入っていない状態で呼び出される可能性がある。
                // 末初期化の場合は Console にログを出力して終了する。
                Console.WriteLine("Logger not initialized.");
                return;
            }

            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}";

            Console.WriteLine(line);
            File.AppendAllText(logFilePath, line + Environment.NewLine);
        }
    }
}