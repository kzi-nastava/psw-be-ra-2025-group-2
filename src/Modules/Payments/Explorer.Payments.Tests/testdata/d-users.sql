DELETE FROM stakeholders."Users";

INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES
(-11, 'autor1@gmail.com', 'autor1', 1, true),
(-12, 'autor2@gmail.com', 'autor2', 1, true),
(-21, 'turista1@gmail.com', 'turista1', 2, true);