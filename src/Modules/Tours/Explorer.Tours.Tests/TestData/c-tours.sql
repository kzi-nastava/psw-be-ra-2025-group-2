DELETE FROM tours."Tours";

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "AuthorId", "Tags", "Status", "Price", "ArchivedAt", "LengthKm", "PublishedAt", 
 "EnvironmentType", "FoodTypes", "AdventureLevel", "ActivityTypes", "SuitableForGroups", "purchases_count", "starts_count")
VALUES
-- AUTOR -11: 3 ture (-1, -2, -11)  
-- NAPOMENA: Test Delete_fails_published_tour koristi -1 umesto -12
(-1, 'Test Tura', 'Ovo je tura za testiranje', 1, -11, '{{test}}', 1, 0, NULL, 10, NOW(), 
 NULL, '', NULL, '', '', 0, 0),
(-2, 'Novi Sad City Tour', 'Obilazak Petrovaradinske tvrdjave i centra grada', 1, -11, '{{grad,kultura,istorija}}', 1, 15.99, NULL, 5, NOW(),
 NULL, '', NULL, '', '', 0, 0),
(-11, 'Fruska Gora Adventure', 'Lagana planinska tura', 3, -11, '{{priroda,planinarenje,avantura}}', 0, 0, NULL, 10, NULL,
 NULL, '', NULL, '', '', 0, 0),

-- AUTOR -12: 3 ture (-13, -14, -15)
(-13, 'Carpathian Adventure', 'Avantura u planinama', 4, -12, '{{planina,avantura,priroda}}', 0, 0, NULL, 20, NULL,
 NULL, '', NULL, '', '', 0, 0),
(-14, 'Dunav Kayaking', 'Veslanje Dunavom', 4, -12, '{{voda,sport,avantura}}', 1, 25.00, NULL, 15, NOW(),
 NULL, '', NULL, '', '', 0, 0),
(-15, 'Stara Planina Extreme', 'Ekstremna avantura na Staroj Planini', 5, -12, '{{planina,ekstremno,avantura}}', 1, 30.00, NULL, 30, NOW(),
 NULL, '', NULL, '', '', 0, 0);