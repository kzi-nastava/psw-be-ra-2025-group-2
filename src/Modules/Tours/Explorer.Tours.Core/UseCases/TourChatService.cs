using System.Text;
using System.Text.Json;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Execution;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;

namespace Explorer.Tours.Core.UseCases.Execution
{
    public class TourChatService : ITourChatService
    {
        private readonly ITourExecutionRepository _executionRepository;
        private readonly ITourRepository _tourRepository;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public TourChatService(
            ITourExecutionRepository executionRepository,
            ITourRepository tourRepository,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _executionRepository = executionRepository;
            _tourRepository = tourRepository;
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["LLM:GroqApiKey"];

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("Groq API key is not configured!");
            }
            Console.WriteLine($"API Key loaded: {_apiKey.Substring(0, 10)}..."); // Debug log
        }


        public async Task<TourChatResponseDto> GetChatResponseAsync(long userId, TourChatRequestDto request)
        {
            try
            {
                // 1. Get execution and verify ownership
                var execution = _executionRepository.Get(request.ExecutionId);
                if (execution == null || execution.TouristId != userId)
                {
                    return new TourChatResponseDto
                    {
                        Success = false,
                        Error = "Tour execution not found or unauthorized",
                        Timestamp = DateTime.UtcNow
                    };
                }

                // 2. Get tour details
                var tour = await _tourRepository.GetByIdAsync(execution.TourId);
                if (tour == null)
                {
                    return new TourChatResponseDto
                    {
                        Success = false,
                        Error = "Tour not found",
                        Timestamp = DateTime.UtcNow
                    };
                }

                // 3. Build context
                var tourContext = BuildTourContext(tour, execution);

                // 4. Call LLM API
                var llmResponse = await CallGroqApiAsync(tourContext, request);

                return new TourChatResponseDto
                {
                    Response = llmResponse,
                    Success = true,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new TourChatResponseDto
                {
                    Success = false,
                    Error = $"An error occurred: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
            }
        }

        private string BuildTourContext(Tour tour, TourExecution execution)
        {
            var context = new StringBuilder();

            context.AppendLine($"Tour Name: {tour.Name}");
            context.AppendLine($"Description: {tour.Description}");
            context.AppendLine($"Difficulty Level: {tour.Difficulty}/5");
            context.AppendLine($"Length: {tour.LengthKm} km");
            context.AppendLine($"Tags: {string.Join(", ", tour.Tags)}");
            context.AppendLine();

            // Get visited key points from KeyPointVisits
            var visitedOrdinals = execution.KeyPointVisits
                .Select(v => v.KeyPointOrdinal)
                .ToHashSet();

            context.AppendLine("Key Points:");
            foreach (var kp in tour.KeyPoints.OrderBy(k => k.OrdinalNo))
            {
                var isVisited = visitedOrdinals.Contains(kp.OrdinalNo);
                var status = isVisited ? "✓ Completed" : "○ Upcoming";

                context.AppendLine($"{kp.OrdinalNo}. {kp.Name} [{status}]");
                context.AppendLine($"   Description: {kp.Description}");
                context.AppendLine($"   Location: {kp.Latitude}, {kp.Longitude}");

                // Only show secret text for visited key points
                if (isVisited && !string.IsNullOrEmpty(kp.SecretText))
                {
                    context.AppendLine($"   Secret Info: {kp.SecretText}");
                }
                context.AppendLine();
            }

            context.AppendLine($"Current Execution Status: {execution.State}");
            context.AppendLine($"Progress: {execution.KeyPointVisits.Count}/{execution.KeyPointsCount} key points completed");
            context.AppendLine($"Completion Percentage: {execution.GetPercentageCompleted():F1}%");

            if (tour.Equipment?.Any() == true)
            {
                context.AppendLine();
                context.AppendLine("Required Equipment:");
                foreach (var eq in tour.Equipment)
                {
                    context.AppendLine($"- {eq.Name}: {eq.Description}");
                }
            }

            return context.ToString();
        }

        private async Task<string> CallGroqApiAsync(string tourContext, TourChatRequestDto request)
        {
            var messages = new List<object>();

            // System message with tour context
            messages.Add(new
            {
                role = "system",
                content = $@"You are a helpful tour guide assistant. You have access to the following tour information:

{tourContext}

Your role is to:
- Answer questions about the tour, its key points, and requirements
- Provide helpful tips and guidance during the tour
- Help users navigate to key points
- Share interesting information about locations
- Assist with any tour-related queries
- Provide directions and distance information when asked

Keep your responses friendly, concise, and helpful. If users ask about key points they haven't reached yet, avoid spoiling secret information. Be encouraging and supportive throughout their tour experience."
            });

            // Add conversation history (limit to last 10 messages to save tokens)
            foreach (var msg in request.ConversationHistory.TakeLast(10))
            {
                messages.Add(new
                {
                    role = msg.Role,
                    content = msg.Content
                });
            }

            // Add current user message
            messages.Add(new
            {
                role = "user",
                content = request.Message
            });

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = messages,
                temperature = 0.7,
                max_completion_tokens = 1024,
                top_p = 1,
                stream = false
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync(
                "https://api.groq.com/openai/v1/chat/completions",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Groq API error: {response.StatusCode} - {errorContent}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            // Add logging to see what we get back
            Console.WriteLine($"Groq response: {responseBody}");

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

            return jsonResponse
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
    }
}