
-- 1. PRVO brišemo zavisne tabele (one koje sadrže TourId ili TouristId)
DELETE FROM tours."KeyPointVisit";
DELETE FROM tours."TourReviews";
DELETE FROM tours."TourProblems";
DELETE FROM tours."TourExecutions";
DELETE FROM tours."TouristEquipment";
DELETE FROM tours."KeyPoint"; -- Ako su tačke u posebnoj tabeli

DELETE FROM tours."Bundles";

-- 2. TEK ONDA smemo da obrišemo Ture (jer više niko ne zavisi od njih)
DELETE FROM tours."Tours";

-- 3. Brišemo ostale nezavisne entitete
DELETE FROM tours."Equipment"; -- Imao si ovo dva puta, dovoljno je jednom
DELETE FROM tours."TouristObject";
DELETE FROM tours."Monument";

-- 4. NA KRAJU brišemo korisnike (jer Ture i Recenzije zavise od njih)
DELETE FROM stakeholders."People"; -- Obavezno prvo People ako postoji
DELETE FROM stakeholders."Users";

DELETE FROM tours."TourReports";