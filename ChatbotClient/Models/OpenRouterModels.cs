namespace ChatbotClient.Models
{
    public static class OpenRouterModels
    {
        // IDを定数として保持
        public const string Gemini2Flash = "google/gemini-2.0-flash-001";
        public const string Gemini2FlashLite = "google/gemini-2.0-flash-lite-001";

        public const string ChatGpt4O = "openai/gpt-4o-2024-11-20";

        public const string Gemma3Free = "google/gemma-3-27b-it:free";
        public const string Llama3Free = "meta-llama/llama-3.3-70b-instruct:free";

        // EnumからIDを引くためのヘルパー
        public static string GetModelId(AiModelType type) => type switch
        {
            AiModelType.GeminiFlash => Gemini2Flash,
            AiModelType.GeminiFlashLite => Gemini2FlashLite,
            AiModelType.GemmaFree => Gemma3Free,
            AiModelType.LlamaFree => Llama3Free,
            AiModelType.ChatGPT4o => ChatGpt4O,
            _ => Gemini2FlashLite,
        };
    }
}