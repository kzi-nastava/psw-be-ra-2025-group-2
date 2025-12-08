DELETE FROM tours."PublicKeyPointRequests" WHERE "Id" < 0;
DELETE FROM tours."PublicKeyPoints" WHERE "Id" < 0;
DELETE FROM tours."Equipment" WHERE "Id" < 0;
DELETE FROM tours."KeyPoint" WHERE "TourId" < 0;
DELETE FROM tours."Tours" WHERE "Id" < 0;

INSERT INTO tours."Tours" 
("Id", "Name", "Description", "Difficulty", "AuthorId", "Status", "Price", "Tags")
VALUES 
(-11, 'Test Tour for Public KeyPoints', 'Test tour', 2, -11, 1, 100.0, '{"test", "public"}'), 
(-12, 'Another Test Tour', 'Second tour', 1, -11, 1, 50.0, '{"test"}'),
(-13, 'Other Author Tour', 'Other tour', 1, -99, 1, 75.0, '{"test"}');

INSERT INTO tours."KeyPoint" 
("TourId", "OrdinalNo", "Name", "Description", "SecretText", "ImageUrl", "Latitude", "Longitude")
VALUES 
(-11, 1, 'Start Point', 'Desc', 'SECRET_START', '', 45.25, 19.84),
(-11, 2, 'Middle Point', 'Desc', 'SECRET_MIDDLE', '', 45.26, 19.85),
(-11, 3, 'Forest Path', 'Desc', 'FOREST789', '', 45.255, 19.845),
(-11, 4, 'Mountain View', 'Desc', 'SECRET123', 'https://example.com/image.jpg', 45.3, 19.85),
(-11, 5, 'River View', 'Desc', 'RIVER_SECRET', 'https://example.com/river.jpg', 45.26, 19.82),
(-12, 1, 'Park Entrance', 'Desc', 'PARK_ENTRY', '', 45.27, 19.83);

INSERT INTO tours."PublicKeyPoints" 
("Id", "Name", "Description", "SecretText", "ImageUrl", "Latitude", "Longitude", "AuthorId", "CreatedAt", "SourceTourId", "SourceOrdinalNo")
VALUES 
(-1, 'Test Public Point 1', 'Desc', 'PUBLIC1', '', 45.24, 19.83, -11, CURRENT_TIMESTAMP, -11, 1),
(-2, 'Test Public Point 2', 'Desc', 'PUBLIC2', '', 45.25, 19.84, -11, CURRENT_TIMESTAMP, -11, 2);

INSERT INTO tours."PublicKeyPointRequests" 
("Id", "PublicKeyPointId", "AuthorId", "Status", "CreatedAt", "ProcessedAt", "ProcessedByAdminId", "RejectionReason")
VALUES 
(-1, -1, -11, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 1, NULL),
(-2, -2, -11, 0, CURRENT_TIMESTAMP, NULL, NULL, NULL);

INSERT INTO tours."Equipment" ("Id", "Name", "Description") VALUES 
(-1, 'Stapovi', 'Opis'), (-2, 'Kaciga', 'Opis'), (-3, 'Uze', 'Opis');