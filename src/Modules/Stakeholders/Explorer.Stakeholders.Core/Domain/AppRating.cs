using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class AppRating : Entity
    {
        public long UserId { get; private set; }
        public int Score { get; private set; }
        public string? Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; } 


        public AppRating(long userId, int score, string? comment)
        {
            UserId = userId;
            Score = score;
            Comment = comment;
            CreatedAt = DateTime.UtcNow;

            Validate();
        }

        public void SetUpdatedAt()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(int score, string? comment)
        {
            Score = score;
            Comment = comment;

            Validate();
        }

        private void Validate()
        {
            if (UserId == 0)
                throw new ArgumentException("Invalid UserId");

            if (Score < 1 || Score > 5)
                throw new ArgumentException("Score must be from 1 to 5");
        }
    }
}