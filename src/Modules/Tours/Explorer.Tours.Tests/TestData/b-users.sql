DELETE FROM stakeholders."Users";

INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES
(-1, 'admin@gmail.com', 'admin', 0, true),
(-11, 'autor1@gmail.com', 'autor1', 1, true),
(-12, 'autor2@gmail.com', 'autor2', 1, true),
(-13, 'autor3@gmail.com', 'autor3', 1, true),
(-21, 'turista1@gmail.com', 'turista1', 2, true),
(-22, 'turista2@gmail.com', 'turista2', 2, true),
(-23, 'turista3@gmail.com', 'turista3', 2, true);
