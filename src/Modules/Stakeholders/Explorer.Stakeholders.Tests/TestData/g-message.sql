INSERT INTO stakeholders."Messages"
("Id", "SenderId", "ReceiverId", "Content", "CreatedAt")
VALUES
(-1, 1, 2, 'Hello Bob', now()),
(-2, 2, 1, 'Hi Alice', now()),
(-3, 1, 3, 'Admin hello', now());
