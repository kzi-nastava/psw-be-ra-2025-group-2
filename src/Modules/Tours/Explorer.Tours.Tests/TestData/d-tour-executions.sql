-- 1. UBACIVANJE SESIJA (EXECUTIONS)
-- Napomena: Koristimo CURRENT_TIMESTAMP da bi LastActivity uvek bio "svež" (danas), 
-- kako bi prošao validaciju "aktivnost u poslednjih 7 dana".

INSERT INTO tours."TourExecutions"(
	"Id", "TouristId", "TourId", "State", "LastActivityTimestamp", "CompletionTimestamp", "KeyPointsCount")
VALUES 
-- Turista -21: Završio turu skroz (4/4 tačke). Ovo koristimo za postojeću recenziju.
(-1, -21, -1, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 4),

-- Turista -23: Prešao 50% (2/4 tačke). OVO JE ZA TVOJ TEST "Creates". 
-- Mora imati > 35% da bi prošao validaciju.
(-2, -23, -1, 1, CURRENT_TIMESTAMP, NULL, 4),

-- Turista -22: Prešao 25% (1/4 tačke). 
-- Ovo možeš koristiti da testiraš da li sistem ZABRANJUJE ocenjivanje (jer je < 35%).
(-3, -22, -1, 1, CURRENT_TIMESTAMP, NULL, 4);


-- 2. UBACIVANJE POSETA (KEY POINT VISITS)
-- Ovo je neophodno da bi metoda "GetPercentageCompleted()" vratila tačan broj.

-- Za Sesiju -1 (Turista -21) -> 100% (Sve 4 tačke)
INSERT INTO tours."KeyPointVisit"("Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp") 
VALUES 
(-100, -1, 1, CURRENT_TIMESTAMP),
(-101, -1, 2, CURRENT_TIMESTAMP),
(-102, -1, 3, CURRENT_TIMESTAMP),
(-103, -1, 4, CURRENT_TIMESTAMP);

-- Za Sesiju -2 (Turista -23) -> 50% (Prve 2 tačke) -> DOVOLJNO ZA TEST
INSERT INTO tours."KeyPointVisit"("Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp") 
VALUES 
(-200, -2, 1, CURRENT_TIMESTAMP),
(-201, -2, 2, CURRENT_TIMESTAMP);

-- Za Sesiju -3 (Turista -22) -> 25% (Samo 1 tačka) -> NIJE DOVOLJNO
INSERT INTO tours."KeyPointVisit"("Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp") 
VALUES 
(-300, -3, 1, CURRENT_TIMESTAMP);