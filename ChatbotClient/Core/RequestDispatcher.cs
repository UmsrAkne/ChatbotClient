using System;
using System.ClientModel;
using System.Linq;
using System.Threading.Tasks;
using ChatbotClient.Models;
using OpenAI;
using OpenAI.Chat;

namespace ChatbotClient.Core
{
    public class RequestDispatcher
    {
        public async Task<TalkEntry> SendRequest(TalkRequest req)
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
            var client = new ChatClient(req.ModelName, new ApiKeyCredential(apiKey), options);

            // まかり間違って異常に大きな出力を引いた時のために、リミットをつける。
            var opt = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 2000, // 日本語では 1文字あたり、 [1.2 - 1.3] トークン程度。
            };

            // 引数にリストを渡す（SystemPrompt や History は呼び出し側で設定）
            ChatCompletion completion = await client.CompleteChatAsync(req.GeneratedMessages(), opt);

            // トークン使用量の取得
            if (completion.Usage != null)
            {
                var inputTokens = completion.Usage.InputTokenCount;   // 投げた量
                var outputTokens = completion.Usage.OutputTokenCount; // 返ってきた量
                var totalTokens = completion.Usage.TotalTokenCount;   // 合計

                Console.WriteLine($"[Usage] Input: {inputTokens}, Output: {outputTokens}, Total: {totalTokens}");
            }

            var text = completion.Content?.FirstOrDefault()?.Text ?? string.Empty;

            // ログとしても出力
            Console.WriteLine(text);
            var entry = new TalkEntry()
            {
                GenerationId = completion.Id,
                Content = text,
                Role = "assistant",
            };

            return entry;
        }
    }
}