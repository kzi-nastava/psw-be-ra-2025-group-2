-- Blogovi za autora -21 (sve Draft osim jednog Published)
INSERT INTO blog."BlogPosts" ("Id", "AuthorId", "Title", "Description", "CreatedAt", "State") VALUES 
    (-1, -21, 'Test blog for integration', 'Ovo je test blog post za integracione testove.', NOW(), 0),
    (-2, -21, 'Post for Updating and Reading', 'This is the original description for testing updates.', NOW(), 0),
    (-3, -21, 'Third Post by Author 21', 'Another test post.', NOW(), 0),
    (-4, -21, 'Fourth Post by Author 21', 'Post for publish tests.', NOW(), 0),
    (-6, -21, 'Published Blog for Voting', 'This blog is published and ready for votes.', NOW(), 1),
    (-8, -21, 'Draft Blog for Vote Test', 'Draft blog to test voting restriction.', NOW(), 0),
    (-9, -21, 'Published Blog for Changing Vote', 'Test blog for changing votes.', NOW(), 1);

-- Blog za drugog autora (ako treba testirati "tuđi blog")
INSERT INTO blog."BlogPosts" ("Id", "AuthorId", "Title", "Description", "CreatedAt", "State") VALUES 
    (-5, -22, 'Blog of Another Author', 'This blog belongs to author -22.', NOW(), 0),
    (-7, -22, 'Published Blog by Author 22', 'Published blog for testing.', NOW(), 1); 

-- Slike za blog -2
INSERT INTO blog."BlogImages" ("Id","Url","BlogPostId") VALUES 
    (-1, 'https://test.com/img_original_1.jpg', -2),
    (-2, 'https://test.com/img_original_2.jpg', -2);

-- Postojeći glasovi za blog -6 (Published Blog for Voting)
INSERT INTO blog."BlogPostVotes" ("Id", "BlogPostId", "UserId", "VoteValue", "CreatedAt") VALUES
    (-1, -6, -22, 1, NOW()),   
    (-2, -6, -23, 1, NOW()),   
    (-3, -6, -11, -1, NOW());

-- Dodatni glasovi za blog -9 (Published Blog for Changing Vote)
INSERT INTO blog."BlogPostVotes" ("Id", "BlogPostId", "UserId", "VoteValue", "CreatedAt") VALUES
    (-4, -9, -22, 1, NOW()),   
    (-5, -9, -23, -1, NOW());

-- Opcionalno: dodaj komentare na blog -6 ili -9 ako hoces testirati update status na osnovu commentCount
INSERT INTO blog."Comments" ("Id", "UserId", "BlogPostId", "Text", "CreatedAt", "LastModifiedAt") VALUES 
    (-11, -24, -6, 'Extra comment 1 for scoring', NOW(), NULL),
    (-12, -25, -6, 'Extra comment 2 for scoring', NOW(), NULL),
    (-13, -26, -6, 'Extra comment 3 for scoring', NOW(), NULL);
