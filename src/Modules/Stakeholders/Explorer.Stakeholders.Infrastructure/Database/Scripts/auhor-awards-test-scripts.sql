DELETE FROM stakeholders."People";
DELETE FROM stakeholders."Users";
DELETE FROM stakeholders."AuthorAwards";



INSERT INTO stakeholders."Users"(
    "Id", "Username", "Password", "Role", "IsActive")
VALUES (-1, 'admin@gmail.com', 'admin', 0, true);



INSERT INTO stakeholders."Users"(
    "Id", "Username", "Password", "Role", "IsActive")
VALUES (-11, 'autor1@gmail.com', 'autor1', 1, true);
INSERT INTO stakeholders."Users"(
    "Id", "Username", "Password", "Role", "IsActive")
VALUES (-12, 'autor2@gmail.com', 'autor2', 1, true);
INSERT INTO stakeholders."Users"(
    "Id", "Username", "Password", "Role", "IsActive")
VALUES (-13, 'autor3@gmail.com', 'autor3', 1, true);

INSERT INTO stakeholders."Users"(
    "Id", "Username", "Password", "Role", "IsActive")
VALUES (-21, 'turista1@gmail.com', 'turista1', 2, true);
INSERT INTO stakeholders."Users"(
    "Id", "Username", "Password", "Role", "IsActive")
VALUES (-22, 'turista2@gmail.com', 'turista2', 2, true);
INSERT INTO stakeholders."Users"(
    "Id", "Username", "Password", "Role", "IsActive")
VALUES (-23, 'turista3@gmail.com', 'turista3', 2, true);



INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-11, -11, 'Ana', 'Anić', 'autor1@gmail.com');
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-12, -12, 'Lena', 'Lenić', 'autor2@gmail.com');
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-13, -13, 'Sara', 'Sarić', 'autor3@gmail.com');

INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-21, -21, 'Pera', 'Perić', 'turista1@gmail.com');
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-22, -22, 'Mika', 'Mikić', 'turista2@gmail.com');
INSERT INTO stakeholders."People"(
    "Id", "UserId", "Name", "Surname", "Email")
VALUES (-23, -23, 'Steva', 'Stević', 'turista3@gmail.com');



INSERT INTO stakeholders."AuthorAwards"(
    "Id", "Name", "Description", "Year", "State", "VotingStartDate", "VotingEndDate")
VALUES (-1, 'Golden Quill', 'Annual literature award', 2020, 0, '2020-03-01', '2020-03-15');

INSERT INTO stakeholders."AuthorAwards"(
    "Id", "Name", "Description", "Year", "State", "VotingStartDate", "VotingEndDate")
VALUES (-2, 'Silver Pen', NULL, 2021, 0, '2021-04-10', '2021-04-20');

INSERT INTO stakeholders."AuthorAwards"(
    "Id", "Name", "Description", "Year", "State", "VotingStartDate", "VotingEndDate")
VALUES (-3, 'Author Spotlight', 'Recognizes emerging authors', 2022, 0, '2022-05-05', '2022-05-25');

INSERT INTO stakeholders."AuthorAwards"(
    "Id", "Name", "Description", "Year", "State", "VotingStartDate", "VotingEndDate")
VALUES (-4, 'Readers Choice', NULL, 2023, 0, '2023-06-01', '2023-06-18');

INSERT INTO stakeholders."AuthorAwards"(
    "Id", "Name", "Description", "Year", "State", "VotingStartDate", "VotingEndDate")
VALUES (-5, 'Critics Award', 'Selected by critics panel', 2028, 0, '2028-02-01', '2028-02-28');

INSERT INTO stakeholders."AuthorAwards"(
    "Id", "Name", "Description", "Year", "State", "VotingStartDate", "VotingEndDate")
VALUES (-6, 'Heritage Award', NULL, 2029, 0, '2029-01-10', '2029-01-30');
