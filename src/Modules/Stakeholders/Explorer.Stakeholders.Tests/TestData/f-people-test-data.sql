INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES (-100, 'testuser1@test.com', 'test123', 2, true);

INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES (-101, 'testuser2@test.com', 'test123', 2, true);

INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES (-102, 'testauthor@test.com', 'test123', 1, true);

INSERT INTO stakeholders."People"("Id", "UserId", "Name", "Surname", "Email", "Biography", "Motto", "ProfileImageUrl")
VALUES (-100, -100, 'Test', 'User', 'testuser1@test.com', NULL, NULL, NULL);

INSERT INTO stakeholders."People"("Id", "UserId", "Name", "Surname", "Email", "Biography", "Motto", "ProfileImageUrl")
VALUES (-101, -101, 'Test', 'User2', 'testuser2@test.com', 'Existing bio', 'Old motto', 'http://old.jpg');

INSERT INTO stakeholders."People"("Id", "UserId", "Name", "Surname", "Email", "Biography", "Motto", "ProfileImageUrl")
VALUES (-102, -102, 'Test', 'Author', 'testauthor@test.com', NULL, NULL, NULL);