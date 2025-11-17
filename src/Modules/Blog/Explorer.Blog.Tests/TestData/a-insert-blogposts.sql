INSERT INTO blog."BlogPosts"("Id", "Title", "Description", "CreatedAt", "AuthorId")
VALUES (-2, 'Test Blog With Images', 'Blog with images for testing', NOW(), -1);

INSERT INTO blog."BlogImages" ("Url", "BlogPostId")
VALUES ('https://test.com/image1.jpg', -2),
       ('https://test.com/image2.jpg', -2);
