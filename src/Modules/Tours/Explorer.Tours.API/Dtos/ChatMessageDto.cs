// Explorer.Tours.API/Dtos/TourChatDto.cs
namespace Explorer.Tours.API.Dtos
{
    public class TourChatRequestDto
    {
        public long ExecutionId { get; set; }
        public string Message { get; set; }
        public List<ChatMessageDto> ConversationHistory { get; set; } = new();
    }

    public class ChatMessageDto
    {
        public string Role { get; set; } // "user" or "assistant"
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class TourChatResponseDto
    {
        public string Response { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}