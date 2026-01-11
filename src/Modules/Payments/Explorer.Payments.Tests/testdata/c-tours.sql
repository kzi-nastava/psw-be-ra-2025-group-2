DELETE FROM tours."Tours";

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "AuthorId", "Tags", "Status", "Price", "LengthKm")
VALUES
(-1, 'Test Tura', 'Ovo je tura za testiranje', 1, -11, '{{"test"}}', 1, 0, 10),
(-2, 'Novi Sad City Tour', 'Obilazak Petrovaradinske tvrdjave', 1, -11, '{{"grad","kultura"}}', 1, 15.99, 5),
(-11, 'Fruska Gora Adventure', 'Lagana planinska tura', 3, -11, '{{"priroda"}}', 0, 0, 10),
(-13, 'Carpathian Adventure', 'Avantura u planinama', 4, -12, '{{"planina"}}', 0, 0, 20),
(-14, 'Dunav Kayaking', 'Veslanje Dunavom', 4, -12, '{{"voda"}}', 0, 0, 15),
(-15, 'Stara Planina Extreme', 'Ekstremna avantura', 5, -12, '{{"planina"}}', 0, 0, 30);