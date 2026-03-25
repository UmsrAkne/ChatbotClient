using System.Collections.Generic;
using System.Text;
using ChatbotClient.Models;

namespace ChatbotClient.Core
{
    public class SystemPromptFormatter
    {
        public static string BuildSystemPrompt(string systemPromptText, List<AttachedFile> attachedFiles)
        {
            var builder = new StringBuilder();

            builder.AppendLine("# Instructions");
            builder.AppendLine(systemPromptText);

            if (attachedFiles.Count == 0)
            {
                return builder.ToString();
            }

            builder.AppendLine();
            builder.AppendLine("# Attached Context Files");
            builder.AppendLine("Below are the contents of the files referenced for this task.");

            foreach (var file in attachedFiles)
            {
                builder.AppendLine();
                builder.AppendLine($"## [File: {file.FullPath}]");
                builder.AppendLine("```");
                builder.AppendLine(file.GetLatestContent());
                builder.AppendLine("```");
            }

            return builder.ToString();
        }
    }
}