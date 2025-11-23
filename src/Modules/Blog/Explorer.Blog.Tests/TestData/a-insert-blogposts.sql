INSERT INTO blog."BlogPosts" ("Id", "AuthorId", "Title", "Description", "CreatedAt") VALUES 
    (-1, -21, 'Test blog for integration', 'Ovo je test blog post za integracione testove.', NOW()),
    (-2, -21, 'Post for Updating and Reading', 'This is the original description for testing updates.', NOW()),
    (-3, -21, 'Third Post by Author 21', 'Another test post.', NOW()),
    (-4, -1, 'Simple Post for Reading', 'A basic post without images.', NOW()); 

INSERT INTO blog."BlogImages" ("Id","Url","BlogPostId") VALUES 
    (-1, 'https://test.com/img_original_1.jpg', -2),
    (-2, 'https://test.com/img_original_2.jpg', -2);