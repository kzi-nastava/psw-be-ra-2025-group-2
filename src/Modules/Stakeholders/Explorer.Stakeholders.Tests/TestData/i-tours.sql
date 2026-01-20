INSERT INTO tours."Tours" (
    "Id", "Name", "Description", "Difficulty", "Tags", "Status", "Price", 
    "AuthorId", "LengthKm", "PublishedAt", "ArchivedAt", "EnvironmentType", 
    "FoodTypes", "AdventureLevel", "ActivityTypes", "SuitableForGroups"
) VALUES
(-1, 'Belgrade Historical Walk', 'Explore the rich history of Belgrade from ancient times to modern day. Visit Kalemegdan Fortress, Knez Mihailova Street, and historic landmarks.', 1, '{{history, culture, urban, walking}}', 1, 15.00, -11, 5.5, '2024-03-01 10:00:00+00', NULL, 1, '4,7', 1, '2', '1,2,3,4,5'),
(-2, 'Tara National Park Adventure', 'Full-day hiking experience in one of Serbias most beautiful national parks. Stunning viewpoints and pristine nature.', 4, '{{nature, hiking, mountains, adventure}}', 1, 45.00, -11, 18.5, '2024-02-15 09:00:00+00', NULL, 2, '1,3,4', 2, '1,3', '4,5'),
(-3, 'Niš Food Tour', 'Discover the culinary delights of Niš. Taste traditional Serbian dishes, visit local markets, and learn about food culture.', 2, '{{food, culture, urban}}', 1, 35.00, -11, 3.2, '2024-03-10 12:00:00+00', NULL, 1, '4,7', 1, '2,3', '1,3,4,5'),
(-4, 'Fruška Gora Wine Route', 'Explore the monasteries and wineries of Fruška Gora. Includes wine tasting and traditional lunch.', 2, '{{wine, culture, nature, relaxing}}', 1, 55.00, -11, 45.0, '2024-04-01 11:00:00+00', NULL, 3, '4,6', 1, '2,3', '5'),
(-5, 'Kopaonik Ski Experience', 'Day trip to Kopaonik ski resort. Perfect for winter sports enthusiasts of all levels.', 3, '{{skiing, winter, sports, mountains}}', 1, 75.00, -11, 12.0, '2024-11-01 08:00:00+00', NULL, 2, '5', 2, '1', '4,5'),
(-6, 'Danube Cycling Tour', 'Scenic cycling route along the Danube river from Belgrade to Smederevo.', 3, '{{cycling, nature, river, active}}', 1, 40.00, -11, 62.0, '2024-05-15 07:00:00+00', NULL, 2, '3,4,7', 2, '1,3', '5'),
(-7, 'Zemun Old Town Walk', 'Charming walk through the old streets of Zemun with stunning Danube views and bohemian atmosphere.', 1, '{{culture, history, photography, walking}}', 1, 12.00, -11, 4.0, '2024-03-20 10:00:00+00', NULL, 1, '4,7', 1, '2,3', '1,2,3,4,5'),
(-8, 'Drina River Rafting', 'Exciting white-water rafting adventure on the Drina river. Adrenaline guaranteed!', 4, '{{rafting, adventure, water-sports, adrenaline}}', 1, 65.00, -11, 25.0, '2024-06-01 09:00:00+00', NULL, 2, '5', 3, '1', '5'),
(-9, 'Novi Sad City Tour', 'Comprehensive tour of Novi Sad including Petrovaradin Fortress and city center. Perfect introduction to Serbian Athens.', 1, '{{city, culture, history, walking}}', 1, 18.00, -11, 6.5, '2024-04-10 11:00:00+00', NULL, 1, '4,7', 1, '2', '1,2,3,4,5'),
(-10, 'Zlatibor Nature Retreat', 'Relaxing 2-day retreat in Zlatibor with easy hikes, traditional food, and wellness activities.', 2, '{{nature, wellness, relaxing, mountains}}', 1, 120.00, -11, 15.0, '2024-07-01 10:00:00+00', NULL, 2, '1,2,3,4', 1, '3', '4,5'),
(-11, 'Belgrade Street Art Tour', 'Urban exploration of Belgrades vibrant street art scene. Visit Savamala and hidden murals.', 1, '{{art, urban, culture, photography}}', 1, 20.00, -11, 5.0, '2024-05-01 14:00:00+00', NULL, 1, '5,7', 1, '2', '1,4,5'),
(-12, 'Rtanj Mountain Mystery', 'Challenging hike to the pyramidal peak of Rtanj. Explore the mysteries and legends of this unique mountain.', 5, '{{hiking, mountains, adventure, nature}}', 1, 50.00, -11, 22.0, '2024-06-15 06:00:00+00', NULL, 2, '3,4', 3, '1', '5'),
(-13, 'Old Serbia Tour (Draft)', 'Multi-day tour through southern Serbia - still planning the route and stops.', 3, '{{history, culture, monasteries}}', 0, 0.00, -11, NULL, NULL, NULL, 3, '4', 2, '2,3', '4,5'),
(-14, 'Archived Summer Tour', 'This tour is no longer available.', 2, '{{summer, old}}', 2, 30.00, -11, 10.0, '2023-05-01 10:00:00+00', '2024-12-01 10:00:00+00', 3, '4', 1, '2,3', '1,4,5');

INSERT INTO tours."TourEquipment" ("EquipmentId", "TourId") VALUES
(-1, -2), (-2, -2), (-4, -2), (-5, -2), (-6, -2), (-7, -2), (-8, -2), (-9, -2),
(-1, -8), (-2, -8), (-4, -8), (-5, -8), (-7, -8),
(-1, -6), (-11, -6), (-12, -6), (-4, -6), (-9, -6),
(-1, -12), (-2, -12), (-4, -12), (-5, -12), (-6, -12), (-7, -12), (-8, -12), (-10, -12),
(-3, -1), (-4, -1), (-9, -1),
(-3, -11), (-4, -11);

-- Tour Durations (TransportType: 0=Walking, 1=Bicycle, 2=Car, 3=Boat)
INSERT INTO tours."TourDuration" ("TransportType", "Minutes", "TourId") VALUES
(0, 180, -1), (2, 90, -1),
(0, 360, -2),
(0, 120, -3), (2, 60, -3),
(2, 240, -4), (0, 120, -4),
(2, 180, -5), (0, 240, -5),
(1, 420, -6),
(0, 150, -7), (2, 45, -7),
(3, 180, -8), (2, 120, -8),
(0, 200, -9), (2, 60, -9),
(2, 180, -10), (0, 240, -10),
(0, 120, -11), (2, 30, -11),
(0, 480, -12);
