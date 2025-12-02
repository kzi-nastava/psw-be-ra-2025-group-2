public class Comment
{
    public long UserId { get; private set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public Comment() { }

    public Comment(long userId, string text)
    {
        UserId = userId;
        Text = text;
        CreatedAt = DateTime.UtcNow;
        LastModifiedAt = null;
        Validate();
    }


    // Proverava da li je proslo 15 minuta od kreiranja komentara
    public bool CanEditOrDelete()
    {
        return DateTime.UtcNow <= CreatedAt.AddMinutes(15);
    }

    private void Validate(bool skipUsernameValidation = false)
    {
        if (string.IsNullOrWhiteSpace(Text))
            throw new ArgumentException("Comment text cannot be empty.");

        if (!skipUsernameValidation && UserId <= 0) throw new ArgumentException("Invalid user ID.");
    }

    public void Edit(string newText)
    {
        if (!CanEditOrDelete())
            throw new InvalidOperationException("Comment can only be edited within 15 minutes of creation.");
        Text = newText;
        LastModifiedAt = DateTime.UtcNow;
        Validate();
    }

}
