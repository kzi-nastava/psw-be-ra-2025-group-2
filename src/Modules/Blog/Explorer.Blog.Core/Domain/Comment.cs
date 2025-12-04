public class Comment
{
    public long Id { get; private set; }
    public long BlogPostId { get; private set; }  // Komentar pripada blogu
    public long UserId { get; private set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }

    private Comment() { } // Za EF Core

    public Comment(long blogPostId, long userId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Comment text cannot be empty.");

        BlogPostId = blogPostId;
        UserId = userId;
        Text = text;
        CreatedAt = DateTime.UtcNow;
        Validate();
    }

    private void Validate()
    {
        if (UserId <= 0)
            throw new ArgumentException("Invalid user ID.");

        if (BlogPostId <= 0)
            throw new ArgumentException("Invalid blog post ID.");

        if (string.IsNullOrWhiteSpace(Text))
            throw new ArgumentException("Comment text cannot be empty.");

        if (Text.Length > 1000)
            throw new ArgumentException("Comment text cannot exceed 1000 characters.");
    }

    public void Edit(string newText)
    {
        if (!CanEditOrDelete())
            throw new InvalidOperationException("Edit window has expired (15 minutes).");

        if (string.IsNullOrWhiteSpace(newText))
            throw new ArgumentException("Comment text cannot be empty.");

        Text = newText;
        LastModifiedAt = DateTime.UtcNow;
    }

    public bool CanEditOrDelete()
    {
        return DateTime.UtcNow - CreatedAt <= TimeSpan.FromMinutes(15);


    }

    public void UpdateText(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
            throw new ArgumentException("Comment text cannot be empty.");

        if (newText.Length > 1000)
            throw new ArgumentException("Comment text cannot exceed 1000 characters.");

        Text = newText;
        LastModifiedAt = DateTime.UtcNow; // Ovo setuje UpdatedAt
    }
}