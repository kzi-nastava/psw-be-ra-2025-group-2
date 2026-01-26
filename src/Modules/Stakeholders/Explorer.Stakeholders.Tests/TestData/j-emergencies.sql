INSERT INTO stakeholders."EmergencyDirectories"
("Id", "Instructions", "Disclaimer", "CountryCode")
VALUES
(-100, 
 'U hitnim situacijama:
1) Pozovi lokalni broj za hitne slučajeve.
2) Ako si povređen, idi u najbližu bolnicu.
3) U slučaju incidenta, kontaktiraj policiju.',
 'Prikazani podaci su informativnog karaktera i neobavezujući.',
 'RS'
);

INSERT INTO stakeholders."EmergencyPlaces"
("Id", "DirectoryId", "Type", "Name", "Address", "Phone")
VALUES
-- Hospitals (Type = 0)
(-101, -100, 0, 'Urgentni centar KCS', 'Pasterova 2, Beograd', '+381 11 361 7777'),
(-102, -100, 0, 'KBC Zemun', 'Vukova 9, Beograd', '+381 11 377 2222'),
-- Police (Type = 1)
(-103, -100, 1, 'PS Stari Grad', 'Majke Jevrosime 31, Beograd', '+381 11 333 444'),
(-104, -100, 1, 'PS Novi Beograd', 'Bulevar Mihajla Pupina 165, Beograd', '+381 11 222 333');

INSERT INTO stakeholders."Embassies"
("Id", "DirectoryId", "Name", "Address", "Phone", "Email", "Website")
VALUES
(-201, -100, 'Embassy of Germany in Serbia',
 'Kneza Miloša 74-76, Beograd',
 '+381 11 301 8700',
 'info@belgrad.diplo.de',
 'https://belgrad.diplo.de'),

(-202, -100, 'Embassy of France in Serbia',
 'Zmaj Jovina 11, Beograd',
 '+381 11 302 3600',
 'info@ambafrance.rs',
 'https://rs.ambafrance.org');

 INSERT INTO stakeholders."EmergencyPhrases"
("Id", "DirectoryId", "Category", "MyText", "LocalText")
VALUES
-- Medicine
(-301, -100, 0,
 'Treba mi hitna medicinska pomoć.',
 'I need urgent medical help.'),

(-302, -100, 0,
 'Povređen sam i ne mogu da se pomerim.',
 'I am injured and cannot move.'),

(-303, -100, 0,
 'Imam jake bolove u stomaku.',
 'I have severe stomach pain.');

 INSERT INTO stakeholders."EmergencyPhrases"
("Id", "DirectoryId", "Category", "MyText", "LocalText")
VALUES
-- Police
(-304, -100, 1,
 'Opljačkan sam.',
 'I have been robbed.'),

(-305, -100, 1,
 'Izgubio sam dokumenta.',
 'I have lost my documents.'),

(-306, -100, 1,
 'Treba mi pomoć policije.',
 'I need police assistance.');
