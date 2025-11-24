DELETE FROM tours."Tours";
INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "AuthorId", "Tags", "Status", "Price")
VALUES
(-11, 'Fruska Gora Adventure', 'Lagana planinska tura', 3, -11, '{{"priroda","planinarenje","avantura"}}', 0, 0),
(-2, 'Novi Sad City Tour', 'Obilazak Petrovaradinske tvrdjave i centra grada', 1, -11, '{{"grad","kultura","istorija"}}', 1, 15.99),
(-13, 'Carpathian Adventure', 'Avantura u planinama', 4, -12, '{{"planina","avantura","priroda"}}', 0, 0),
(-14, 'Dunav Kayaking', 'Veslanje Dunavom', 4, -12, '{{"voda","sport","avantura"}}', 0, 0),
(-15, 'Stara Planina Extreme', 'Ekstremna avantura na Staroj Planini', 5, -12, '{{"planina","ekstremno","avantura"}}', 0, 0);