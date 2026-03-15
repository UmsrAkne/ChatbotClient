using System;
using System.ClientModel;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;

namespace ChatbotClient.Core
{
    public class RequestDispatcher
    {
        public async Task<string> SendRequest(string message, string modelName)
        {
            // 環境変数から API キーを取得
            var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");

            // キーが取れなかった時のエラーハンドリング
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("環境変数 'OPENROUTER_API_KEY' が設定されていません。");
            }

            var options = new OpenAIClientOptions
            {
                Endpoint = new Uri("https://openrouter.ai/api/v1"),
            };

            // 取得した apiKey を渡す
            var client = new ChatClient(modelName, new ApiKeyCredential(apiKey), options);

            ChatCompletion completion = await client.CompleteChatAsync(message);

            // トークン使用量の取得
            if (completion.Usage != null)
            {
                var inputTokens = completion.Usage.InputTokenCount;   // 投げた量
                var outputTokens = completion.Usage.OutputTokenCount; // 返ってきた量
                var totalTokens = completion.Usage.TotalTokenCount;   // 合計

                Console.WriteLine($"[Usage] Input: {inputTokens}, Output: {outputTokens}, Total: {totalTokens}");
            }

            var text = completion?.Content?.Count > 0 ? completion.Content[0].Text : string.Empty;

            // ログとしても出力
            Console.WriteLine(text);
            return text;
        }
    }
}