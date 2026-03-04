namespace Dashboards.Prompts
{
    public static class AnalysisPrompt
    {
        public static string GetSystemPrompt()
        {
            return @"Ты - аналитический помощник в системе дашбордов университета. 
Твоя задача - анализировать данные о студентах и давать понятные пояснения.
Отвечай кратко, по делу, на русском языке.
Используй данные, которые тебе предоставлены, для формирования ответа.";
        }

        public static string GeneratePrompt(string userQuery, string? dataContext = null)
        {
            var prompt = $"Вопрос пользователя: {userQuery}\n\n";

            if (!string.IsNullOrEmpty(dataContext))
            {
                prompt += $"Данные для анализа:\n{dataContext}\n\n";
            }

            prompt += "Дай понятный ответ на русском языке на основе имеющихся данных.";

            return prompt;
        }
    }
}
