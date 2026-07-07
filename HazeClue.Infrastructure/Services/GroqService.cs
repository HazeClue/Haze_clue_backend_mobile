using HazeClue.Core.Domain.Contracts;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace HazeClue.Infrastructure.Services
{
    public class GroqService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GroqService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GeneratePersonalizedTipAsync(string assessmentData, string smartwatchSummary, string focusSummary)
        {
            var apiKey = _configuration["GROQ_API_KEY"] ?? Environment.GetEnvironmentVariable("GROQ_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("GROQ_API_KEY is not configured.");
            }

            var prompt = $@"
You are a personalized AI health and cognitive coach for the 'HazeClue' platform.
Your goal is to provide a single, short, and actionable insight (1-3 sentences) to the user based on their recent data.

User Health Assessment Profile:
{assessmentData}

Recent Smartwatch Data (Last 7 days):
{smartwatchSummary}

Recent Focus Sessions Data (Last 7 days):
{focusSummary}

Based on this data, provide a very brief, encouraging, and actionable health or cognitive tip. 
If they have high stress and low HRV, recommend relaxation or a simulation session. 
If their sleep is bad, recommend better sleep hygiene. 
If they are doing great, encourage them to keep it up.
Do not output anything other than the tip itself.
";

            var requestBody = new
            {
                model = "llama3-8b-8192",
                messages = new[]
                {
                    new { role = "system", content = "You are an AI coach. Output only the short tip." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 150
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
            {
                Content = jsonContent
            };
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Groq API Error ({response.StatusCode}): {errorContent}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            
            var tip = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return tip?.Trim() ?? "Remember to stay hydrated and take short breaks to maintain your focus.";
        }
    }
}
