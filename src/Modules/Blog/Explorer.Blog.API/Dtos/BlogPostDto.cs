namespace Explorer.Blog.API.Dtos
{
    public class BlogPostDto
    {
        public long Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; } // Markdown text
        public DateTime CreatedAt { get; set; }
        public long AuthorId { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public int State { get; set; }

        public int Score { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
        public int? UserVote {  get; set; }

    }
}
