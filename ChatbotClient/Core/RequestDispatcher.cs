using System;
using System.ClientModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatbotClient.Models;
using ChatbotClient.Utils;
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

            // ストリーミングを監視して、異常な出力がトークン最大数まで完走する事故を防ぐ
            var fullResponse = new StringBuilder();
            var finishReason = "unknown";

            // 逐次出力を受け取る
            var updates = client.CompleteChatStreamingAsync(req.GeneratedMessages(), opt);

            try
            {
                await foreach (var update in updates)
                {
                    foreach (var fragment in update.ContentUpdate)
                    {
                        if (string.IsNullOrEmpty(fragment.Text))
                        {
                            continue;
                        }

                        fullResponse.Append(fragment.Text);

                        // AI側が終了した理由を更新し続ける（最後には Stop や Length が入る）
                        if (update.FinishReason != null)
                        {
                            finishReason = update.FinishReason.ToString();
                        }

                        // 【異常検知ロジック】
                        // 例: 直近の文字列が「同じ文字の連続（10文字以上）」なら即遮断
                        if (IsAbnormalRepetition(fullResponse.ToString()))
                        {
                            // ここでループを抜ければ、それ以降の通信（課金）は発生しません
                            finishReason = "Abnormal Repetition Detected"; // 独自理由をセット
                            Logger.Log("!!! 異常な繰り返しを検知。緊急停止します。 !!!");
                            goto EmergencyStop;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                finishReason = "exception";
                Logger.Log($"通信エラー: {ex.Message}");
            }

            EmergencyStop:
            var text = fullResponse.ToString();

            // ログ出力
            Logger.Log($"[Finish Reason]: {finishReason}");
            Logger.Log($"[Final Output]: {text}");

            string completionId = null;
            var sb = new StringBuilder();

            await foreach (var update in updates)
            {
                // IDなどは各アップデートに含まれている（最初の一回で取ればOK）
                completionId ??= update.CompletionId;

                foreach (var fragment in update.ContentUpdate)
                {
                    sb.Append(fragment.Text);
                }

                // トークン使用量は「最後のパケット」にだけ入ってくる仕様
                if (update.Usage != null)
                {
                    Logger.Log($"[Final Usage] Input: {update.Usage.InputTokenCount}, Output: {update.Usage.OutputTokenCount}");
                }
            }

            var entry = new TalkEntry(text, false)
            {
                GenerationId = completionId,
            };

            return entry;
        }

        private bool IsAbnormalRepetition(string currentText)
        {
            if (currentText.Length < 10)
            {
                return false;
            }

            // 末尾10文字がすべて同じ文字かチェック（例：AAAA....）
            var lastPart = currentText.Substring(currentText.Length - 10);
            return lastPart.All(c => c == lastPart[0]);
        }
    }
}