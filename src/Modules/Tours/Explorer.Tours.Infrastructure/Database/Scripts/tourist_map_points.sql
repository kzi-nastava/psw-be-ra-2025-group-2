INSERT INTO tours."Monument" ("Id", "Name", "Description", "YearOfCreation", "State", "Latitude", "Longitude")
VALUES 
(1, 'Petrovaradin Fortress', 'Historic fortress overlooking the Danube, known as Gibraltar on the Danube', 1692, 0, 45.2517, 19.8658),
(2, 'Dunavska Street', 'Historic pedestrian street in the heart of Novi Sad', 1748, 0, 45.2551, 19.8451),
(3, 'Name of Mary Church', 'Beautiful neo-Gothic Catholic cathedral in the city center', 1895, 0, 45.2549, 19.8447),
(4, 'Novi Sad Synagogue', 'One of the largest synagogues in Europe, architectural masterpiece', 1909, 0, 45.2543, 19.8425),
(5, 'Bishop Palace', 'Baroque palace and residence of Serbian Orthodox bishop', 1741, 0, 45.2562, 19.8453);

INSERT INTO tours."TouristObject" ("Id", "Name", "Latitude", "Longitude", "Category")
VALUES 
(1, 'Museum of Vojvodina', 45.2556, 19.8434, 0),
(2, 'Danube Park', 45.2539, 19.8412, 1),
(3, 'Strand Beach', 45.2389, 19.8567, 2),
(4, 'Serbian National Theatre', 45.2558, 19.8442, 0),
(5, 'Fruška Gora National Park', 45.1547, 19.7103, 3),
(6, 'Spens Sports Center', 45.2472, 19.8328, 4),
(7, 'Fisherman''s Island', 45.2403, 19.8598, 2),
(8, 'Novi Sad Fair', 45.2614, 19.8289, 5),
(9, 'Liberty Square', 45.2569, 19.8451, 6),
(10, 'Rainbow Bridge', 45.2494, 19.8389, 6);