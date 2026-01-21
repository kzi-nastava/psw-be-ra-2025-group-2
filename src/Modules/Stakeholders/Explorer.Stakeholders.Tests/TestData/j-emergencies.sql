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
