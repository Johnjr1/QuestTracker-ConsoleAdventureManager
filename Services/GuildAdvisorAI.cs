// 4. AI Assistance (Guild Advisor – ChatGPT)
// AI kan hjälpa till med:
// Generera quest descriptions → t.ex. användaren skriver bara titel “Rädda byn” → AI skapar en episk quest-text.
// Föreslå prioritet → baserat på deadline och innehåll.
// Sammanfatta quests → systemet ger en kort heroisk briefing över alla pågående uppdrag.

// Guild Advisor AI integration
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using dotenv.net;
using Twilio.Rest.Serverless.V1.Service.Environment;

public static class GuildAdvisorAI
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<Quest?> GenerateQuestFromTitle(string title)
    {
        DotEnv.Load();
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.Error.WriteLine("Missing OPENAI_API_KEY.");
            return null;
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var requestBody = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = 
                    "Du är en hjälpsam gille-rådgivare i ett fantasy-uppdragssystem. " +
                    "Svara ENBART i JSON-format: {\"title\": \"...\", \"description\": \"...\", \"priority\": \"Low/Medium/High\", \"duedate\": \"YYYY-MM-DD\"}. " +
                    "Gör texten episk men kortfattad. Beskrivningen ska passa ett spel-uppdrag." },
                new { role = "user", content = $"Titel: {title}" }
            }
        };

        string json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var responseString = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseString);
        var root = doc.RootElement;

        if (!root.TryGetProperty("choices", out var choices))
        {
            Console.WriteLine("Fel vid AI-svar.");
            return null;
        }

        var messageContent = choices[0].GetProperty("message").GetProperty("content").GetString();

        try
        {
            var questData = JsonSerializer.Deserialize<QuestAIResponse>(messageContent!);
            if (questData == null) return null;

            return new Quest
            {
                Title = questData.title,
                Description = questData.description,
                Priority = questData.priority,
                DueDate = DateTime.TryParse(questData.duedate, out var d) ? d : DateTime.Now.AddDays(3)
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid tolkning av AI-data: {ex.Message}");
            Console.WriteLine("AI-svar var:");
            Console.WriteLine(messageContent);
            return null;
        }
    }

    private class QuestAIResponse
    {
        public string title { get; set; } = "";
        public string description { get; set; } = "";
        public string priority { get; set; } = "Medium";
        public string duedate { get; set; } = "";
    }
}