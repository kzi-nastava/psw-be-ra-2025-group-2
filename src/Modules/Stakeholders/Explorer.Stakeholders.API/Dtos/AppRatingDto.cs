namespace Explorer.Stakeholders.API.Dtos
{
    public class AppRatingDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}