using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AlemarBudgetelHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ChatbotController> _logger;

        public ChatbotController(
            IConfiguration configuration, 
            IHttpClientFactory httpClientFactory,
            ILogger<ChatbotController> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Message))
                {
                    return BadRequest(new { error = "Message is required" });
                }

                _logger.LogInformation("Chatbot question: {Message}", request.Message);

                // Get API key from configuration (secure)
                var apiKey = _configuration["Anthropic:ApiKey"];
                
                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("Anthropic API key not configured");
                    return Ok(new ChatResponse 
                    { 
                        Response = GetFallbackResponse(request.Message),
                        Source = "fallback"
                    });
                }

                // Call Anthropic API
                var httpClient = _httpClientFactory.CreateClient();
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, 
                    "https://api.anthropic.com/v1/messages");
                
                httpRequest.Headers.Add("x-api-key", apiKey);
                httpRequest.Headers.Add("anthropic-version", "2023-06-01");
                
                var requestBody = new
                {
                    model = "claude-3-haiku-20240307",
                    max_tokens = 300,
                    messages = new[] 
                    {
                        new 
                        { 
                            role = "user", 
                            content = BuildPrompt(request.Message) 
                        }
                    },
                    system = "You are Alemar Assistant, a helpful hotel chatbot. Keep responses short, friendly, and use emojis where appropriate."
                };
                
                var json = JsonSerializer.Serialize(requestBody);
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(httpRequest);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API request failed with status: {Status}", response.StatusCode);
                    return Ok(new ChatResponse 
                    { 
                        Response = GetFallbackResponse(request.Message),
                        Source = "fallback"
                    });
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<AnthropicResponse>(responseContent);
                
                var botResponse = apiResponse?.Content?[0]?.Text ?? GetFallbackResponse(request.Message);
                
                _logger.LogInformation("Successfully got AI response");
                
                return Ok(new ChatResponse 
                { 
                    Response = botResponse,
                    Source = "ai"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in chatbot API");
                return Ok(new ChatResponse 
                { 
                    Response = GetFallbackResponse(request?.Message ?? ""),
                    Source = "fallback"
                });
            }
        }

        private string BuildPrompt(string userMessage)
        {
            return $@"You are Alemar Assistant for Alemar Budgetel Hotel.

HOTEL INFORMATION:
Rooms: Single, Double, Standard, Deluxe, Super Deluxe, Super Duper
Pricing: 
  - 3 hours: ₱300-₱2,000
  - 12 hours: ₱800-₱5,000  
  - 24 hours: ₱1,200-₱7,000
Payment: GCash, Cash, Credit/Debit Card
Booking: Browse → Select → Choose Duration → Payment → Confirm

CUSTOMER QUESTION: {userMessage}

Provide a helpful, friendly response in 2-3 sentences max. Use emojis.";
        }

        private string GetFallbackResponse(string message)
        {
            var lowerMessage = message.ToLower();

            if (lowerMessage.Contains("room") || lowerMessage.Contains("type"))
            {
                return "We have 6 room types: Single, Double, Standard, Deluxe, Super Deluxe, and Super Duper! 🛏️ Each offers different comfort levels to match your needs.";
            }
            
            if (lowerMessage.Contains("price") || lowerMessage.Contains("cost") || lowerMessage.Contains("much"))
            {
                return "Our flexible pricing starts at ₱300 for 3 hours, ₱800 for 12 hours, or ₱1,200 for 24 hours! 💰 Prices vary by room type.";
            }
            
            if (lowerMessage.Contains("book") || lowerMessage.Contains("reserve"))
            {
                return "Booking is easy! Just browse rooms, select your favorite, choose duration (3/12/24 hours), pick payment method, and confirm! ✅";
            }
            
            if (lowerMessage.Contains("pay") || lowerMessage.Contains("payment"))
            {
                return "We accept GCash 📱, Cash 💵, and Credit/Debit Cards 💳 for your convenience!";
            }
            
            if (lowerMessage.Contains("hello") || lowerMessage.Contains("hi"))
            {
                return "Hello! 👋 I'm here to help you find the perfect room. What would you like to know?";
            }

            return "I can help you with room types, pricing, booking process, and payment methods! What would you like to know? 😊";
        }
    }

    // Request/Response models
    public class ChatRequest
    {
        public string Message { get; set; }
    }

    public class ChatResponse
    {
        public string Response { get; set; }
        public string Source { get; set; } // "ai" or "fallback"
    }

    // Anthropic API response model
    public class AnthropicResponse
    {
        public List<ContentItem> Content { get; set; }
    }

    public class ContentItem
    {
        public string Type { get; set; }
        public string Text { get; set; }
    }
}
