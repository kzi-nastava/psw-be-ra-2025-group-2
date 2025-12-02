-- Blogovi za autora -21 (sve Draft osim jednog Published)
INSERT INTO blog."BlogPosts" ("Id", "AuthorId", "Title", "Description", "CreatedAt", "State") VALUES 
    (-1, -21, 'Test blog for integration', 'Ovo je test blog post za integracione testove.', NOW(), 0),
    (-2, -21, 'Post for Updating and Reading', 'This is the original description for testing updates.', NOW(), 0),
    (-3, -21, 'Third Post by Author 21', 'Another test post.', NOW(), 0),
    (-4, -21, 'Fourth Post by Author 21', 'Post for publish tests.', NOW(), 0);

-- Blog za drugog autora (ako treba testirati "tuđi blog")
INSERT INTO blog."BlogPosts" ("Id", "AuthorId", "Title", "Description", "CreatedAt", "State") VALUES 
    (-5, -22, 'Blog of Another Author', 'This blog belongs to author -22.', NOW(), 0);

-- Slike za blog -2
INSERT INTO blog."BlogImages" ("Id","Url","BlogPostId") VALUES 
    (-1, 'https://test.com/img_original_1.jpg', -2),
    (-2, 'https://test.com/img_original_2.jpg', -2);