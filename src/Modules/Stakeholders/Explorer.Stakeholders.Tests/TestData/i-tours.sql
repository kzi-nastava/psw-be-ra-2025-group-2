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
(0, 100, -9), (2, 60, -9),
(2, 180, -10), (0, 240, -10),
(0, 60, -11), (2, 30, -11),
(0, 480, -12);


-- Key Points
INSERT INTO tours."KeyPoint" ("TourId", "OrdinalNo", "Name", "Description", "SecretText", "ImageUrl", "Latitude", "Longitude", "AuthorId", "IsPublic", "EncounterId") VALUES
(-1, 1, 'Kalemegdan Entrance', 'Start your journey at the main entrance of Kalemegdan Fortress, the historical heart of Belgrade.', 'The fortress was built in the 3rd century BC', 'https://picsum.photos/seed/kp1/800/600', 44.8225, 20.4508, 2, false, NULL),
(-1, 2, 'Victor Monument', 'The iconic Pobednik (Victor) statue overlooking the confluence of Sava and Danube rivers.', 'Erected in 1928 to commemorate Serbian victories', 'https://picsum.photos/seed/kp2/800/600', 44.8244, 20.4491, 2, false, NULL),
(-1, 3, 'Knez Mihailova Street', 'Walk down the main pedestrian street, full of shops, cafes, and street performers.', 'Named after Prince Mihailo Obrenović', 'https://picsum.photos/seed/kp3/800/600', 44.8176, 20.4573, 2, false, NULL),
(-1, 4, 'Republic Square', 'The central square featuring the National Museum and National Theatre.', 'Meeting point of Belgrade since 1869', 'https://picsum.photos/seed/kp4/800/600', 44.8163, 20.4603, 2, false, NULL),
(-2, 1, 'Bajina Bašta Starting Point', 'Begin your adventure at this charming riverside town.', 'Gateway to Tara National Park', 'https://picsum.photos/seed/kp5/800/600', 43.9703, 19.5672, 8, false, NULL),
(-2, 2, 'Banjska Stena Viewpoint', 'Breathtaking viewpoint over the Drina river canyon - one of the most photographed spots in Serbia.', 'Height of 1,100 meters above sea level', 'https://picsum.photos/seed/kp6/800/600', 43.9134, 19.4556, 8, false, 1),
(-2, 3, 'Perućac Lake', 'Crystal clear lake surrounded by dense forests. Perfect spot for a break.', 'Created by dam in 1966', 'https://picsum.photos/seed/kp7/800/600', 43.8792, 19.4256, 8, false, NULL),
(-2, 4, 'Drina River House', 'Famous tiny house built on a rock in the middle of the Drina river.', 'Built in 1968 by local teenagers', 'https://picsum.photos/seed/kp8/800/600', 43.9845, 19.5234, 8, false, NULL),
(-3, 1, 'Niš Fortress', 'Start at the impressive 18th-century Ottoman fortress in the city center.', 'Built on Roman foundations', 'https://picsum.photos/seed/kp9/800/600', 43.3209, 21.8961, 6, false, NULL),
(-3, 2, 'Tinkers Alley', 'Historic cobblestone street with traditional craft shops and authentic Serbian restaurants.', 'Dating back to the 18th century', 'https://picsum.photos/seed/kp10/800/600', 43.3195, 21.8972, 6, false, NULL),
(-3, 3, 'Central Market', 'Bustling local market where you can taste fresh produce and traditional delicacies.', 'Open since 1935', 'https://picsum.photos/seed/kp11/800/600', 43.3201, 21.8956, 6, false, NULL),
(-4, 1, 'Krušedol Monastery', 'Beautiful Orthodox monastery from the 16th century, famous for its frescoes.', 'Burial place of Serbian kings', 'https://picsum.photos/seed/kp12/800/600', 45.1428, 19.8078, 3, true, NULL),
(-4, 2, 'Vrdnik Monastery', 'Peaceful monastery surrounded by healing thermal springs.', 'Founded in the 16th century', 'https://picsum.photos/seed/kp13/800/600', 45.1347, 19.7856, 3, true, NULL),
(-4, 3, 'Šumarice Winery', 'Family winery producing excellent local wines. Includes tasting session.', 'Traditional Bermet wine specialty', 'https://picsum.photos/seed/kp14/800/600', 45.1523, 19.8234, 3, false, NULL),
(-6, 1, 'Belgrade Waterfront Start', 'Begin cycling along the newly developed waterfront promenade.', 'Modern development on the Sava river', 'https://picsum.photos/seed/kp15/800/600', 44.8081, 20.4473, 2, false, NULL),
(-6, 2, 'Vinča Archaeological Site', 'Ancient settlement from 5700 BC, one of Europes oldest.', 'Discovered in 1908', 'https://picsum.photos/seed/kp16/800/600', 44.7556, 20.6547, 2, false, NULL),
(-6, 3, 'Smederevo Fortress', 'Massive medieval fortress on the Danube, largest lowland fortress in Europe.', 'Built 1428-1430', 'https://picsum.photos/seed/kp17/800/600', 44.6614, 20.9292, 2, false, NULL),
(-7, 1, 'Gardoš Tower', 'Historic tower offering panoramic views of Zemun and the Danube.', 'Millennium Tower built in 1896', 'https://picsum.photos/seed/kp18/800/600', 44.8492, 20.4056, 3, false, NULL),
(-7, 2, 'Zemun Quay', 'Charming riverside promenade with cafes and fish restaurants.', 'Former border between Austria and Ottoman Empire', 'https://picsum.photos/seed/kp19/800/600', 44.8431, 20.4023, 3, false, NULL),
(-7, 3, 'Madlenianum Opera', 'Beautiful private opera house and art gallery.', 'Opened in 1999', 'https://picsum.photos/seed/kp20/800/600', 44.8503, 20.4089, 3, false, NULL),
(-9, 1, 'Petrovaradin Fortress', 'Massive fortress known as "Gibraltar on the Danube", home to Exit festival.', 'Built 1692-1780', 'https://picsum.photos/seed/kp21/800/600', 45.2525, 19.8653, 2, false, NULL),
(-9, 2, 'Dunavska Street', 'Colorful pedestrian street with galleries, cafes, and shops.', 'Historic heart of Novi Sad', 'https://picsum.photos/seed/kp22/800/600', 45.2551, 19.8447, 2, false, NULL),
(-9, 3, 'Freedom Square', 'Main city square featuring impressive Catholic and Orthodox buildings.', 'City center since 1748', 'https://picsum.photos/seed/kp23/800/600', 45.2558, 19.8447, 2, false, NULL),
(-11, 1, 'Savamala Hub', 'Start at the creative district known for street art and galleries.', 'Cultural renaissance of Belgrade', 'https://picsum.photos/seed/kp24/800/600', 45.2558, 19.8447, 3, false, NULL),
(-11, 2, 'Cetinjska Street Murals', 'Amazing collection of large-scale murals by international artists.', 'Belgrade Street Art Festival location', 'https://picsum.photos/seed/kp25/800/600', 44.8092, 20.4478, 3, false, NULL),
(-11, 3, 'KC Grad', 'Cultural center and gallery space in renovated industrial building.', 'Hub for contemporary art', 'https://picsum.photos/seed/kp26/800/600', 44.8125, 20.4534, 3, false, NULL),
(-12, 1, 'Rtanj Base', 'Starting point at the mountain base. Prepare for steep ascent!', 'Mysterious pyramid-shaped mountain', 'https://picsum.photos/seed/kp27/800/600', 43.7456, 21.9234, 8, false, NULL),
(-12, 2, 'Mountain Hut', 'Halfway point with shelter and water source. Take a break here.', 'Built by mountaineering club', 'https://picsum.photos/seed/kp28/800/600', 43.7523, 21.9289, 8, false, NULL),
(-12, 3, 'Rtanj Peak', 'Summit at 1,565m with 360-degree views. Feel the unique energy of this mystical mountain!', 'Said to have special electromagnetic properties', 'https://picsum.photos/seed/kp29/800/600', 43.7567, 21.9312, 8, false, 2);