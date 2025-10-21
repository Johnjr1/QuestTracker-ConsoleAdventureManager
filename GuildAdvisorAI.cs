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

    public static async Task GuildAdvisor()
    {
        // Ladda .env
        DotEnv.Load();

        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.Error.WriteLine("Missing OPENAI_API_KEY.");
            return;
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        Console.WriteLine("\n=== GUILD ADVISOR ===");
        Console.Write("Skriv titeln på ditt uppdrag för att skapa en beskrivning eller skriv 'exit' för att avsluta: ");
        string userInput = Console.ReadLine() ?? "";

        if (userInput.ToLower() == "exit") return;

        var requestBody = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = "Du är en hjälpsam gille-rådgivare i ett fantasy-uppdragssystem. Du svarar alltid på svenska, med en hjältemodig och berättande ton. Du ska skapa en kortfattad beskrivning på ett uppdrag från titeln som användaren skriver. Du ska också föreslå en deadline t.ex 2025-10-21 du ska även föreslå prioritet (Low. Medium. High) beroende på deadline" },
                new { role = "user", content = userInput }
            }
        };

        string json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var responseString = await response.Content.ReadAsStringAsync();

        using (JsonDocument doc = JsonDocument.Parse(responseString))
        {
            JsonElement root = doc.RootElement;

            // Check if the API returned an error
            if (root.TryGetProperty("error", out JsonElement error))
            {
                Console.WriteLine($"API Error: {error.GetProperty("message").GetString()}");
                return;
            }

            // Safely extract the content if present
            if (root.TryGetProperty("choices", out JsonElement choices)
                && choices.GetArrayLength() > 0
                && choices[0].TryGetProperty("message", out JsonElement message)
                && message.TryGetProperty("content", out JsonElement contentElement))
            {
                string aiResponse = contentElement.GetString() ?? "(tomt svar)";
                Console.WriteLine($"\nGuild Advisor säger:\n{aiResponse}\n");
            }
            else
            {
                Console.WriteLine("Oväntat svar från API:t. Kontrollera API-nyckeln eller begäran:");
                Console.WriteLine(responseString);
            }
        }
    }
}