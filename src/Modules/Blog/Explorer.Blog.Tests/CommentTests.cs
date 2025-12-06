using Explorer.Blog.Core.Domain;
using Shouldly;

namespace Explorer.Blog.Tests.Unit.Domain
{
    public class CommentTests
    {
        [Fact]
        public void Create_comment_succeeds()
        {
            // Arrange & Act
            var comment = new Comment(-1, -21, "Ovo je test komentar.");

            // Assert
            comment.BlogPostId.ShouldBe(-1);
            comment.UserId.ShouldBe(-21);
            comment.Text.ShouldBe("Ovo je test komentar.");
            comment.CreatedAt.ShouldNotBe(default(DateTime));
            comment.LastModifiedAt.ShouldBeNull();
        }

        [Fact]
        public void Create_comment_fails_when_text_empty()
        {
            // Arrange & Act & Assert
            Should.Throw<ArgumentException>(() => new Comment(-1, -21, ""));
        }

        [Fact]
        public void Create_comment_fails_when_text_null()
        {
            // Arrange & Act & Assert
            Should.Throw<ArgumentException>(() => new Comment(-1, -21, null));
        }

        [Fact]
        public void Create_comment_fails_when_text_whitespace()
        {
            // Arrange & Act & Assert
            Should.Throw<ArgumentException>(() => new Comment(-1, -21, "   "));
        }

        [Fact]
        public void Create_comment_fails_when_userId_zero()
        {
            // Arrange & Act & Assert
            Should.Throw<ArgumentException>(() => new Comment(-1, 0, "Test text"));
        }

        [Fact]
        public void Create_comment_succeeds_when_userId_negative()
        {
            // Arrange & Act
            var comment = new Comment(-1, -21, "Test text");

            // Assert
            comment.UserId.ShouldBe(-21);
        }

        [Fact]
        public void Create_comment_fails_when_blogPostId_zero()
        {
            // Arrange & Act & Assert
            Should.Throw<ArgumentException>(() => new Comment(0, -21, "Test text"));
        }

        [Fact]
        public void Create_comment_succeeds_when_blogPostId_negative()
        {
            // Arrange & Act
            var comment = new Comment(-10, -21, "Test text");

            // Assert
            comment.BlogPostId.ShouldBe(-10);
        }

        [Fact]
        public void Create_comment_fails_when_text_exceeds_1000_characters()
        {
            // Arrange
            var longText = new string('a', 1001);

            // Act & Assert
            Should.Throw<ArgumentException>(() => new Comment(-1, -21, longText));
        }

        [Fact]
        public void Create_comment_succeeds_when_text_exactly_1000_characters()
        {
            // Arrange
            var maxText = new string('a', 1000);

            // Act
            var comment = new Comment(-1, -21, maxText);

            // Assert
            comment.Text.Length.ShouldBe(1000);
        }

        [Fact]
        public void Edit_comment_succeeds()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Originalni tekst.");

            // Act
            comment.Edit("Ažurirani tekst.");

            // Assert
            comment.Text.ShouldBe("Ažurirani tekst.");
            comment.LastModifiedAt.ShouldNotBeNull();
        }

        [Fact]
        public void Edit_comment_fails_when_text_empty()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Originalni tekst.");

            // Act & Assert
            Should.Throw<ArgumentException>(() => comment.Edit(""));
        }

        [Fact]
        public void Edit_comment_fails_when_text_null()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Originalni tekst.");

            // Act & Assert
            Should.Throw<ArgumentException>(() => comment.Edit(null));
        }

        [Fact]
        public void Edit_comment_fails_when_text_whitespace()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Originalni tekst.");

            // Act & Assert
            Should.Throw<ArgumentException>(() => comment.Edit("   "));
        }

        [Fact]
        public void Edit_comment_fails_when_edit_window_expired()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Originalni tekst.");

            // Simulacija starog komentara (stariji od 15 minuta)
            var createdAtField = typeof(Comment).GetField("<CreatedAt>k__BackingField",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            createdAtField?.SetValue(comment, DateTime.UtcNow.AddMinutes(-20));

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => comment.Edit("Novi tekst."));
        }

        [Fact]
        public void CanEditOrDelete_returns_true_when_within_15_minutes()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Test komentar.");

            // Act
            var result = comment.CanEditOrDelete();

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void CanEditOrDelete_returns_false_when_older_than_15_minutes()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Test komentar.");

            // Simulacija starog komentara
            var createdAtField = typeof(Comment).GetField("<CreatedAt>k__BackingField",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            createdAtField?.SetValue(comment, DateTime.UtcNow.AddMinutes(-20));

            // Act
            var result = comment.CanEditOrDelete();

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void CanEditOrDelete_returns_true_when_exactly_15_minutes()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Test komentar.");

            // Tačno 15 minuta minus 1 sekunda (da bude sigurno unutar prozora)
            var createdAtField = typeof(Comment).GetField("<CreatedAt>k__BackingField",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            createdAtField?.SetValue(comment, DateTime.UtcNow.AddMinutes(-15).AddSeconds(1));

            // Act
            var result = comment.CanEditOrDelete();

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void UpdateText_succeeds()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Originalni tekst.");

            // Act
            comment.UpdateText("Ažurirani tekst bez provere vremena.");

            // Assert
            comment.Text.ShouldBe("Ažurirani tekst bez provere vremena.");
            comment.LastModifiedAt.ShouldNotBeNull();
        }

        [Fact]
        public void UpdateText_succeeds_even_when_edit_window_expired()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Originalni tekst.");

            // Simulacija starog komentara
            var createdAtField = typeof(Comment).GetField("<CreatedAt>k__BackingField",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            createdAtField?.SetValue(comment, DateTime.UtcNow.AddMinutes(-20));

            // Act
            comment.UpdateText("Novi tekst.");

            // Assert
            comment.Text.ShouldBe("Novi tekst.");
            comment.LastModifiedAt.ShouldNotBeNull();
        }

        [Fact]
        public void UpdateText_fails_when_text_empty()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Originalni tekst.");

            // Act & Assert
            Should.Throw<ArgumentException>(() => comment.UpdateText(""));
        }

        [Fact]
        public void UpdateText_fails_when_text_null()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Originalni tekst.");

            // Act & Assert
            Should.Throw<ArgumentException>(() => comment.UpdateText(null));
        }

        [Fact]
        public void UpdateText_fails_when_text_whitespace()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Originalni tekst.");

            // Act & Assert
            Should.Throw<ArgumentException>(() => comment.UpdateText("   "));
        }

        [Fact]
        public void UpdateText_fails_when_text_exceeds_1000_characters()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Originalni tekst.");
            var longText = new string('a', 1001);

            // Act & Assert
            Should.Throw<ArgumentException>(() => comment.UpdateText(longText));
        }

        [Fact]
        public void Multiple_edits_update_LastModifiedAt()
        {
            // Arrange
            var comment = new Comment(-1, -21, "Originalni tekst.");

            // Act
            comment.Edit("Prva izmena.");
            var firstModified = comment.LastModifiedAt;

            System.Threading.Thread.Sleep(100); // Mali delay

            comment.Edit("Druga izmena.");
            var secondModified = comment.LastModifiedAt;

            // Assert
            firstModified.ShouldNotBeNull();
            secondModified.ShouldNotBeNull();
            
        }
    }
}