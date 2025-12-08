-- Obriši postojeće podatke
DELETE FROM tours."KeyPoint";
DELETE FROM tours."Tours";

-- Insertuj Tours
INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "AuthorId", "Tags", "Status", "Price")
VALUES
(-11, 'Fruska Gora Adventure', 'Lagana planinska tura', 3, -11, '{{"priroda","planinarenje","avantura"}}', 0, 0),
(-2, 'Novi Sad City Tour', 'Obilazak Petrovaradinske tvrdjave i centra grada', 1, -11, '{{"grad","kultura","istorija"}}', 1, 15.99),
(-13, 'Carpathian Adventure', 'Avantura u planinama', 4, -12, '{{"planina","avantura","priroda"}}', 0, 0),
(-14, 'Dunav Kayaking', 'Veslanje Dunavom', 4, -12, '{{"voda","sport","avantura"}}', 0, 0),
(-15, 'Stara Planina Extreme', 'Ekstremna avantura na Staroj Planini', 5, -12, '{{"planina","ekstremno","avantura"}}', 0, 0),
(7, 'Belgrade Tour', 'Obilazak Beograda', 2, 11, '{{"grad","kultura"}}', 1, 20.00);

-- Insertuj KeyPoints za turu -11 (za Author testove)
INSERT INTO tours."KeyPoint"
("TourId", "OrdinalNo", "Name", "Description", "SecretText", "ImageUrl", "Latitude", "Longitude")
VALUES
(-11, 1, 'Vidikovac', 'Prelep pogled na Dunav', 'Tajni tekst 1', 'https://example.com/img1.jpg', 45.2517, 19.8369),
(-11, 2, 'Manastir Krusedol', 'Istorijski manastir', 'Tajni tekst 2', 'https://example.com/img2.jpg', 45.1450, 19.7828),
(-11, 3, 'Strazilovo', 'Planinski vrh', 'Tajni tekst 3', 'https://example.com/img3.jpg', 45.1500, 19.7500),
(-11, 4, 'Letenka', 'Šumska staza', 'Tajni tekst 4', 'https://example.com/img4.jpg', 45.1600, 19.7600),
(-11, 5, 'Iriski Venac', 'Vikend naselje', 'Tajni tekst 5', 'https://example.com/img5.jpg', 45.1700, 19.7700);

-- Insertuj KeyPoints za turu 7 (za Admin testove)
INSERT INTO tours."KeyPoint"
("TourId", "OrdinalNo", "Name", "Description", "SecretText", "ImageUrl", "Latitude", "Longitude")
VALUES
(7, 1, 'Kalemegdan', 'Beogradska tvrdjava', 'Tajni kod 1', 'https://example.com/kalemegdan.jpg', 44.8225, 20.4514),
(7, 2, 'Knez Mihailova', 'Glavna setalisna ulica', 'Tajni kod 2', 'https://example.com/knez.jpg', 44.8176, 20.4573),
(7, 3, 'Hram Svetog Save', 'Najveca pravoslavna crkva', 'Tajni kod 3', 'https://example.com/hram.jpg', 44.7985, 20.4687),
(7, 4, 'Skadarlija', 'Bohemska cetvrt', 'Tajni kod 4', 'https://example.com/skadarlija.jpg', 44.8198, 20.4612),
(7, 5, 'Ada Ciganlija', 'Beogradsko more', 'Tajni kod 5', 'https://example.com/ada.jpg', 44.7894, 20.4078),
(7, 6, 'Zemun Kej', 'Obala Dunava', 'Tajni kod 6', 'https://example.com/zemun.jpg', 44.8433, 20.4089),
(7, 7, 'Avala Toranj', 'TV toranj', 'Tajni kod 7', 'https://example.com/avala.jpg', 44.6925, 20.5144),
(7, 8, 'Topcider Park', 'Istorijski park', 'Tajni kod 8', 'https://example.com/topcider.jpg', 44.7833, 20.4500);