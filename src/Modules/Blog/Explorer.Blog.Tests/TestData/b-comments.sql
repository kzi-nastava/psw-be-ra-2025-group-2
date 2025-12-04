INSERT INTO blog."Comments" ("Id", "UserId", "BlogPostId", "Text", "CreatedAt", "UpdatedAt") VALUES 
    (-1, -21, -1, 'Odlican test blog post! Sve radi kako treba.', NOW(), NULL),
    (-2, -1, -1, 'Slazem se, integration testovi su super implementirani.', NOW(), NULL),
    (-3, -21, -2, 'Originalni opis je vrlo jasan i koncizan.', NOW(), NULL),
    (-4, -1, -2, 'Zanima me kako ce update funkcionisati na ovom postu.', NOW(), NULL),
    (-5, -21, -2, 'Edit: Dodao sam jos razmisljanja o update funkcionalnosti.', NOW(), NOW()),
    (-6, -1, -3, 'Treci post je takodje dobar!', NOW(), NULL),
    (-7, -21, -4, 'Simple ali efikasan post.', NOW(), NULL),
    (-8, -1, -4, 'Bas mi se svidjaa minimalizam ovog posta.', NOW(), NULL);