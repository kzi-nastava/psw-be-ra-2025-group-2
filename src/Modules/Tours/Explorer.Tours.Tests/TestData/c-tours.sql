DELETE FROM tours."Tours";

-- DODATO: Tura -1 koja se koristi u testovima za Recenzije i Sesije
INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "AuthorId", "Tags", "Status", "Price", "ArchivedAt")
VALUES
(-1, 'Test Tura', 'Ovo je tura za testiranje', 1, -11, '{{"test"}}', 1, 0, NULL), 
(-11, 'Fruska Gora Adventure', 'Lagana planinska tura', 3, -11, '{{"priroda","planinarenje","avantura"}}', 0, 0, NULL),
(-2, 'Novi Sad City Tour', 'Obilazak Petrovaradinske tvrdjave i centra grada', 1, -11, '{{"grad","kultura","istorija"}}', 1, 15.99, NULL),
(-13, 'Carpathian Adventure', 'Avantura u planinama', 4, -12, '{{"planina","avantura","priroda"}}', 0, 0, NULL),
(-14, 'Dunav Kayaking', 'Veslanje Dunavom', 4, -12, '{{"voda","sport","avantura"}}', 0, 0, NULL),
(-15, 'Stara Planina Extreme', 'Ekstremna avantura na Staroj Planini', 5, -12, '{{"planina","ekstremno","avantura"}}', 0, 0, NULL);

