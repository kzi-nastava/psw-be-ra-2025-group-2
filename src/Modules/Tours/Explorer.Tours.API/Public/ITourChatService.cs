using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Execution
{
    public interface ITourChatService
    {
        Task<TourChatResponseDto> GetChatResponseAsync(long userId, TourChatRequestDto request);
    }
}